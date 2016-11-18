using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using SimpleAop.Core;
using SimpleAop.Interface;

namespace SimpleAop.Proxy
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
                if (_enablePreInterception)
                    _interception.PreInvoke((MethodInfo)methodCallMessage.MethodBase, methodCallMessage.Args,
                        _target);

                // 问题1：使用 methodReturnMessage = RemotingServices.ExecuteMessage(_target, methodCallMessage) 方法 不能捕获异常

                Exception methodException = null;
                methodReturnMessage = ExecuteWithLog(methodCallMessage, ref methodException);

                if (methodException != null)
                {
                    _interception.ExceptionHandle((MethodInfo)methodCallMessage.MethodBase, methodCallMessage.Args,
                        _target, methodException);
                }

                if (_enableAfterInterception)
                {
                    _interception.AfterInvoke((MethodInfo)methodCallMessage.MethodBase, methodCallMessage.Args,
                        _target);
                }
            }
            return methodReturnMessage;
        }

        /// <summary>
        /// 执行方法抛出异常时记录异常
        /// </summary>
        /// <param name="methodCallMessage"></param>
        /// <param name="methodException"></param>
        /// <returns></returns>
        private IMethodReturnMessage ExecuteWithLog(IMethodCallMessage methodCallMessage, ref Exception methodException)
        {
            IMethodReturnMessage methodReturnMessage;
            try
            {
                object returnValue = null;

                returnValue = _enableArroundInterception
                    ? _interception.ArroundInvoke(new MethodInvocation((MethodInfo)methodCallMessage.MethodBase, _target, methodCallMessage.Args))
                    : methodCallMessage.MethodBase.Invoke(_target, methodCallMessage.Args);

                methodReturnMessage = new ReturnMessage(returnValue, null, 0, methodCallMessage.LogicalCallContext,
                    methodCallMessage);
            }
            catch (Exception ex)
            {
                //methodReturnMessage = new ReturnMessage(ex.InnerException, methodCallMessage);
                methodReturnMessage = new ReturnMessage(null, null, 0, methodCallMessage.LogicalCallContext,
                    methodCallMessage);
                methodException = ex.InnerException;
            }
            return methodReturnMessage;
        }
    }
}