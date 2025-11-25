using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProphetAR
{
    public static class DynamicCaster
    {
        private static readonly MethodInfo GenericCastDefinition = typeof(DynamicCaster).GetMethod(nameof(Cast), BindingFlags.NonPublic);
        private static readonly Dictionary<Type, MethodInfo> CastCache = new();

        private static T Cast<T>(object value)
        {
            return (T) value;
        }
        
        public static object DynamicCast(object o, Type toType)
        {
            if (o == null)
            {
                return null;
            }

            if (!CastCache.TryGetValue(toType, out MethodInfo castMethod))
            {
                castMethod = GenericCastDefinition.MakeGenericMethod(toType);
                CastCache.Add(toType, castMethod);
            }

            return castMethod.Invoke(null, new[] { o });
        }
    }
}