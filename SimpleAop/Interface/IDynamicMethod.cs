namespace SimpleAop.Interface
{
    public interface IDynamicMethod
    {
        /// <summary>
        /// Invokes dynamic method on the specified target object.
        /// </summary>
        /// <param name="target">Target object to invoke method on.</param>
        /// <param name="arguments">Method arguments.</param>
        /// <returns></returns>
        object Invoke(object target, params object[] arguments);
    }
}