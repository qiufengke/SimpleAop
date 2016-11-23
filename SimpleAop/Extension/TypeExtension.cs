using System;

namespace AopIntercept.Extension
{
    public static class TypeExtension
    {
        /// <summary>
        /// 求默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                if (type == typeof(void)) return null;
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}