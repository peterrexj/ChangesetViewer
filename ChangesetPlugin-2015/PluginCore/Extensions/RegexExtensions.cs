using System;
using System.Text.RegularExpressions;

namespace PluginCore.Extensions
{
    public static class RegexEx
    {
        static Func<string, RegexOptions, Regex> _GetOrCreate = FuncExtensions.Create((string pattern, RegexOptions options) => new Regex(pattern, options));
        /// <summary>
        /// <para>(BCR) Returns a Regex instance based on the pattern passed in. Regex may actually be returned as a cached instance.</para>
        /// <para>Created for performance. You need to add RegexOptions.Compiled as an option.</para>
        /// </summary>
        /// <param name="pattern">Regex pattern</param>
        /// <param name="options">Regex options</param>
        /// <returns>a Regex instance based on the pattern passed in. Regex may actually be returned as a cached instance.</returns>
        public static Regex GetOrCreate(string pattern, RegexOptions options) { return _GetOrCreate(pattern, options); }

        /// <summary>
        /// (IS) returns the given text in which [rangeStart - rangeEnd] occurencies are replaced by the given replacement value
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="rangeStart">replacement range start text</param>
        /// <param name="rangeEnd">replacement rang end text</param>
        /// <param name="replacement">replacement value</param>
        /// <returns>the given text in which [rangeStart - rangeEnd] occurencies are replaced by the given replacement value</returns>
        public static string RegexReplace(this string text, string rangeStart, string rangeEnd, string replacement, RegexOptions? regexOptions = null)
        {
            if (!text.HasValue())
                return text;

            var pattern = string.Format(@"{0}(.|\n)*?{1}", rangeStart, rangeEnd);
            regexOptions = regexOptions ?? RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline;
            var regex = GetOrCreate(pattern, regexOptions.Value);
            var res = regex.Replace(text, replacement);
            return res;
        }
    }
}
