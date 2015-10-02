using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginCore.Extensions
{
    public static class EnumEx
    {
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
        public static IEnumerable<TEnum?> GetValuesWithNull<TEnum>() where TEnum : struct
        {
            return EnumerableExtensions.Singleton(new Nullable<TEnum>()).Concat(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(i => new Nullable<TEnum>(i)));
        }

        /// <summary>
        ///  attempts to convert the given string to the given TEnum enum type; returns null if the conversion failed
        /// </summary>
        /// <typeparam name="TEnum">enum type</typeparam>
        /// <param name="value">string to convert to the TEnum enum type</param>
        /// <returns>enum value if the conversion succeeded, otherwise null</returns>
        public static TEnum? TryParse<TEnum>(string value) where TEnum : struct
        {
            TEnum res;
            return Enum.TryParse(value, true, out res) ? res : (TEnum?)null;
        }
    }
}
