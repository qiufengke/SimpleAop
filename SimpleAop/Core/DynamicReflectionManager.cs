using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

namespace SimpleAop.Core
{
    [Serializable]
    internal class DynamicReflectionManager
    {
        private static readonly OpCode[] LdArgOpCodes = { OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Ldarg_2 };

        private static readonly MethodInfo FnGetTypeFromHandle =
            new GetTypeFromHandleDelegate(Type.GetTypeFromHandle).Method;

        private static readonly MethodInfo FnConvertArgumentIfNecessary =
            new ChangeTypeDelegate(ConvertValueTypeArgumentIfNecessary).Method;

        public static object ConvertValueTypeArgumentIfNecessary(object value, Type targetType, int argIndex)
        {
            if (value == null)
            {
                //if (ReflectionUtils.IsNullableType(targetType))
                //{
                //    return null;
                //}
                throw new InvalidCastException(string.Format(
                    "Cannot convert NULL at position {0} to argument type {1}", argIndex, targetType.FullName));
            }

            var valueType = value.GetType();
#if NET_2_0
            if (ReflectionUtils.IsNullableType(targetType))
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }
#endif
            // no conversion necessary?
            if (valueType == targetType)
            {
                return value;
            }

            if (!valueType.IsValueType)
            {
                // we're facing a reftype/valuetype mix that never can convert
                throw new InvalidCastException(
                    string.Format("Cannot convert value '{0}' of type {1} at position {2} to argument type {3}", value,
                        valueType.FullName, argIndex, targetType.FullName));
            }

            // we're dealing only with ValueType's now - try to convert them
            try
            {
                // TODO: allow widening conversions only
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                    string.Format("Cannot convert value '{0}' of type {1} at position {2} to argument type {3}", value,
                        valueType.FullName, argIndex, targetType.FullName), ex);
            }
        }

        public static SafeDynamicMethod.FunctionDelegate CreateMethod(MethodInfo methodInfo)
        {
            //AssertUtils.ArgumentNotNull(methodInfo, "You cannot create a delegate for a null value.");

            var skipVisibility = true; // !IsPublic(methodInfo);
            var dm = CreateDynamicMethod(methodInfo.Name, typeof(object), new[] { typeof(object), typeof(object[]) },
                methodInfo, skipVisibility);
            var il = dm.GetILGenerator();
            EmitInvokeMethod(il, methodInfo, false);
            return (SafeDynamicMethod.FunctionDelegate)dm.CreateDelegate(typeof(SafeDynamicMethod.FunctionDelegate));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static DynamicMethod CreateDynamicMethod(string methodName, Type returnType, Type[] argumentTypes,
            MemberInfo member, bool skipVisibility)
        {
            DynamicMethod dmGetter = null;
            methodName = "_dynamic_" + member.DeclaringType.FullName + "." + methodName;
            try
            {
                new PermissionSet(PermissionState.Unrestricted).Demand();
                dmGetter = CreateDynamicMethodInternal(methodName, returnType, argumentTypes, member, skipVisibility);
            }
            catch (SecurityException)
            {
                dmGetter = CreateDynamicMethodInternal(methodName, returnType, argumentTypes,
                    MethodBase.GetCurrentMethod(), false);
            }
            return dmGetter;
        }

        private static DynamicMethod CreateDynamicMethodInternal(string methodName, Type returnType,
            Type[] argumentTypes, MemberInfo member, bool skipVisibility)
        {
            DynamicMethod dm;
            dm = new DynamicMethod(methodName, returnType, argumentTypes, member.Module, skipVisibility);
            return dm;
        }

        internal static void EmitInvokeMethod(ILGenerator il, MethodInfo method, bool isInstanceMethod)
        {
            var paramsArrayPosition = (isInstanceMethod) ? 2 : 1;
            var args = method.GetParameters();
            IDictionary outArgs = new Hashtable();
            for (var i = 0; i < args.Length; i++)
            {
                SetupOutputArgument(il, paramsArrayPosition, args[i], outArgs);
            }

            if (!method.IsStatic)
            {
                EmitTarget(il, method.DeclaringType, isInstanceMethod);
            }

            for (var i = 0; i < args.Length; i++)
            {
                SetupMethodArgument(il, paramsArrayPosition, args[i], outArgs);
            }

            EmitCall(il, method);

            for (var i = 0; i < args.Length; i++)
            {
                ProcessOutputArgument(il, paramsArrayPosition, args[i], outArgs);
            }

            EmitMethodReturn(il, method.ReturnType);
        }

        private static void ProcessOutputArgument(ILGenerator il, int paramsArrayPosition, ParameterInfo argInfo,
            IDictionary outArgs)
        {
            if (!IsOutputOrRefArgument(argInfo))
                return;

            var argType = argInfo.ParameterType.GetElementType();

            il.Emit(LdArgOpCodes[paramsArrayPosition]);
            il.Emit(OpCodes.Ldc_I4, argInfo.Position);
            il.Emit(OpCodes.Ldloc, (LocalBuilder)outArgs[argInfo.Position]);
            if (argType.IsValueType)
            {
                il.Emit(OpCodes.Box, argType);
            }
            il.Emit(OpCodes.Stelem_Ref);
        }

        private static bool IsOutputOrRefArgument(ParameterInfo argInfo)
        {
            return argInfo.IsOut || argInfo.ParameterType.Name.EndsWith("&");
        }

        /// <summary>
        /// Generates code to process return value if necessary.
        /// </summary>
        /// <param name="il">IL generator to use.</param>
        /// <param name="returnValueType">Type of the return value.</param>
        private static void EmitMethodReturn(ILGenerator il, Type returnValueType)
        {
            if (returnValueType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (returnValueType.IsValueType)
            {
                il.Emit(OpCodes.Box, returnValueType);
            }
            il.Emit(OpCodes.Ret);
        }

        private static void SetupOutputArgument(ILGenerator il, int paramsArrayPosition, ParameterInfo argInfo,
            IDictionary outArgs)
        {
            if (!IsOutputOrRefArgument(argInfo))
                return;

            var argType = argInfo.ParameterType.GetElementType();

            var lb = il.DeclareLocal(argType);
            if (!argInfo.IsOut)
            {
                PushParamsArgumentValue(il, paramsArrayPosition, argType, argInfo.Position);
                il.Emit(OpCodes.Stloc, lb);
            }
            outArgs[argInfo.Position] = lb;
        }

        private static void PushParamsArgumentValue(ILGenerator il, int paramsArrayPosition, Type argumentType,
            int argumentPosition)
        {
            il.Emit(LdArgOpCodes[paramsArrayPosition]);
            il.Emit(OpCodes.Ldc_I4, argumentPosition);
            il.Emit(OpCodes.Ldelem_Ref);
            if (argumentType.IsValueType)
            {
                // call ConvertArgumentIfNecessary() to convert e.g. int32 to double if necessary
                il.Emit(OpCodes.Ldtoken, argumentType);
                EmitCall(il, FnGetTypeFromHandle);
                il.Emit(OpCodes.Ldc_I4, argumentPosition);
                EmitCall(il, FnConvertArgumentIfNecessary);
                EmitUnbox(il, argumentType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, argumentType);
            }
        }

        private static void EmitUnbox(ILGenerator il, Type argumentType)
        {
#if NET_2_0
            il.Emit(OpCodes.Unbox_Any, argumentType);
#else
            il.Emit(OpCodes.Unbox, argumentType);
            il.Emit(OpCodes.Ldobj, argumentType);
#endif
        }

        private static void EmitTarget(ILGenerator il, Type targetType, bool isInstanceMethod)
        {
            il.Emit((isInstanceMethod) ? OpCodes.Ldarg_1 : OpCodes.Ldarg_0);
            if (targetType.IsValueType)
            {
                var local = il.DeclareLocal(targetType);
                EmitUnbox(il, targetType);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, 0);
            }
            else
            {
                il.Emit(OpCodes.Castclass, targetType);
            }
        }

        private static void SetupMethodArgument(ILGenerator il, int paramsArrayPosition, ParameterInfo argInfo,
            IDictionary outArgs)
        {
            if (IsOutputOrRefArgument(argInfo))
            {
                il.Emit(OpCodes.Ldloca_S, (LocalBuilder)outArgs[argInfo.Position]);
            }
            else
            {
                PushParamsArgumentValue(il, paramsArrayPosition, argInfo.ParameterType, argInfo.Position);
            }
        }

        private static void EmitCall(ILGenerator il, MethodInfo method)
        {
            il.EmitCall((method.IsVirtual) ? OpCodes.Callvirt : OpCodes.Call, method, null);
        }

        private delegate Type GetTypeFromHandleDelegate(RuntimeTypeHandle handle);

        private delegate object ChangeTypeDelegate(object value, Type targetType, int argIndex);
    }
}