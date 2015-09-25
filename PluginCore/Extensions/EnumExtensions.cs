using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PluginCore.Extensions
{
    ///// <summary>
    ///// Enum Extensions for general use.
    ///// </summary>
    //public static class EnumExtensions
    //{
    //    static Func<Enum, FieldInfo, bool, IEnumerable<string>>[] EnumDescriptionGetters = new Func<Enum, FieldInfo, bool, IEnumerable<string>>[] {
    //        (val, m, requiresTranslation) => m.Attributes<Janison.Utils.Attributes.JanDisplayNameAttribute>().Select(a => a.GetDisplayName(requiresTranslation)),
    //        (val, m, requiresTranslation) => m.Attributes<DisplayAttribute>().Select(a => a.GetDescription().Alt(() => a.GetName())),
    //        (val, m, requiresTranslation) => m.Attributes<DescriptionAttribute>().Select(a => a.Description),
    //        (val, m, requiresTranslation) => new[] { val.ToString().NotationForm2DisplayForm() }.AsEnumerable()
    //    };

    //    static Func<Type, ILookup<Enum, FieldInfo>> EnumFields =
    //        FuncEx.Create((Type enumType) => Enum.GetValues(enumType).Cast<Enum>().ToLookup(a => a, a => enumType.GetField(a.ToString())))
    //        .Memoize(threadSafe: true);

    //    /// <summary>
    //    ///  Returns the display name for the given enum value
    //    /// </summary>
    //    /// <param name="val">enum value (can be null)</param>
    //    /// <param name="val">by default enum value names are translated to the current culture; when false, the translation doesnt take place and English name is returned</param>
    //    /// <returns>display name for the given enum value</returns>
    //    public static string GetDescription(this Enum val, bool requiresTranslation = true)
    //    {
    //        if (val == null)
    //            return "(not specified)";

    //        var m = EnumFields(val.GetType())[val].First();

    //        return
    //            (
    //                from f in EnumDescriptionGetters
    //                from a in f(val, m, requiresTranslation)
    //                where a.HasValue()
    //                select a
    //            )
    //            .First();
    //    }
    //    /// <summary>
    //    /// (JS) Returns the classname that we use for styling particular enums.
    //    /// </summary>
    //    /// <param name="val">enum value (can be null)</param>
    //    /// <returns>The given enum value as its css class string</returns>
    //    public static string AsCssClass(this Enum val)
    //    {
    //        return val.ToStringEx().ToLowerEx();
    //    }

    //    /// <summary>
    //    /// Checks if an enumerated type contains a value
    //    /// </summary>
    //    public static bool Has<T>(this Enum value, T check)
    //    {
    //        Type type = value.GetType();

    //        //determine the values
    //        _Value parsed = new _Value(check, type);
    //        if (parsed.Signed is long)
    //        {
    //            return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;
    //        }
    //        else if (parsed.Unsigned is ulong)
    //        {
    //            return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    #region Helper Classes

    //    //class to simplfy narrowing values between 
    //    //a ulong and long since either value should
    //    //cover any lesser value
    //    private class _Value
    //    {

    //        //cached comparisons for tye to use
    //        private static Type _UInt64 = typeof(ulong);
    //        private static Type _UInt32 = typeof(long);

    //        public long? Signed;
    //        public ulong? Unsigned;

    //        public _Value(object value, Type type)
    //        {

    //            //make sure it is even an enum to work with
    //            if (!type.IsEnum)
    //            {
    //                throw new
    //        ArgumentException("Value provided is not an enumerated type!");
    //            }

    //            //then check for the enumerated value
    //            Type compare = Enum.GetUnderlyingType(type);

    //            //if this is an unsigned long then the only
    //            //value that can hold it would be a ulong
    //            if (compare.Equals(_Value._UInt32) || compare.Equals(_Value._UInt64))
    //            {
    //                this.Unsigned = Convert.ToUInt64(value);
    //            }
    //            //otherwise, a long should cover anything else
    //            else
    //            {
    //                this.Signed = Convert.ToInt64(value);
    //            }

    //        }

    //    }

    //    #endregion
    //}

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
            return Enum.TryParse<TEnum>(value, true, out res) ? res : (TEnum?)null;
        }
    }
}
