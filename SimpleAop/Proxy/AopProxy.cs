using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using AopIntercept.Core;
using AopIntercept.Extension;
using AopIntercept.Interface;

namespace AopIntercept.Proxy
{
    /// <summary>
    /// 代理
    /// </summary>
    public class AopProxy : RealProxy
    {
        private readonly MarshalByRefObject _target;
        private bool _enableAfterInterception;
        private bool _enableArroundInterception;
        private bool _enablePreInterception;
        private IInterception _interception;

        public AopProxy(Type targetType, MarshalByRefObject target) : base(targetType)
        {
            _target = target;
        }

        /// <summary>
        /// 注入拦截器
        /// </summary>
        /// <param name="interception"></param>
        /// <param name="enablePreInterception">是否启用方法执行后拦截</param>
        /// <param name="enableAfterInterception">是否启用方法执行前拦截</param>
        /// <param name="enableAroundInterception"></param>
        public void InjectInterception(IInterception interception, bool enablePreInterception,
            bool enableAfterInterception, bool enableAroundInterception)
        {
            _interception = interception;
            _enablePreInterception = enablePreInterception;
            _enableAfterInterception = enableAfterInterception;
            _enableArroundInterception = enableAroundInterception;
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodReturnMessage methodReturnMessage = null;

            var constructionCallMessage = msg as IConstructionCallMessage;
            if (constructionCallMessage != null) // Constructor Method.
            {
                var defaultProxy = RemotingServices.GetRealProxy(_target);
                defaultProxy.InitializeServerObject(constructionCallMessage);
                methodReturnMessage =
                    EnterpriseServicesHelper.CreateConstructionReturnMessage(constructionCallMessage,
                        (MarshalByRefObject)GetTransparentProxy());
                return methodReturnMessage;
            }

            var methodCallMessage = msg as IMethodCallMessage;
            if (methodCallMessage != null)
            {
                var methodInfo = (MethodInfo)methodCallMessage.MethodBase;

                if (_enablePreInterception)
                    _interception.PreInvoke(methodInfo, methodCallMessage.Args,
                        _target);

                // 问题1：使用 methodReturnMessage = RemotingServices.ExecuteMessage(_target, methodCallMessage) 方法 不能捕获异常

                Exception methodException = null;
                methodReturnMessage = SafeExecute(methodCallMessage, ref methodException);

                if (methodException != null)
                {
                    _interception.ExceptionHandle(methodInfo, methodCallMessage.Args,
                        _target, methodException);
                }

                if (_enableAfterInterception)
                {
                    _interception.AfterInvoke(methodInfo, methodCallMessage.Args,
                        _target);
                }
            }
            return methodReturnMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodCallMessage"></param>
        /// <param name="methodException"></param>
        /// <returns></returns>
        private IMethodReturnMessage SafeExecute(IMethodCallMessage methodCallMessage, ref Exception methodException)
        {
            IMethodReturnMessage methodReturnMessage;
            var methodInfo = (MethodInfo)methodCallMessage.MethodBase;
            try
            {
                object returnValue = null;

                IMethodInvocation methodInvocation = new MethodInvocation(methodInfo, _target, methodCallMessage.Args);

                returnValue =
                    _enableArroundInterception
                        ? _interception.ArroundInvoke(methodInvocation)
                        : methodInvocation.Proceed();

                methodReturnMessage = new ReturnMessage(returnValue, null, 0, methodCallMessage.LogicalCallContext,
                    methodCallMessage);
            }
            catch (Exception ex)
            {
                //methodReturnMessage = new ReturnMessage(ex.InnerException, methodCallMessage);

                var defaultV = methodInfo.ReturnType.GetDefault();

                methodReturnMessage = new ReturnMessage(defaultV, null, 0, methodCallMessage.LogicalCallContext,
                    methodCallMessage);
                methodException = ex.InnerException ?? ex;
            }
            return methodReturnMessage;
        }
    }
}