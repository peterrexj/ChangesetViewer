using System;
using System.Linq.Expressions;

namespace PluginCore.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// Determines if the Int input is between two values. 
        /// By default the lower and upper are NOT inclusive.
        /// An optional Inclusive bool is supplied if you would like the lower and upper bounds to be included in the test.
        /// </summary>
        public static bool Between(this int input, int lower, int upper, bool inclusive = false)
        {
            if (inclusive)
            {
                lower--;
                upper++;
            }
            return (input > lower && input < upper);
        }
       
        /// <summary>
        /// shortcut for value &gt;= lo and value &lt;= hi - which is correct behaviour for 'between' (unlike the Between() implementation)
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="lo">inclusive lower bound</param>
        /// <param name="hi">inclusive higher bound</param>
        /// <returns>true if value is between lo and hi, otherwise false</returns>
        public static bool BetweenEx(this int value, int? lo, int? hi)
        {
            return (lo == null || value >= lo) && (hi == null || value <= hi);
        }
        /// <summary>
        /// <para>(IS) returns the result of num / denom, or null if b is 0</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="num">numerator</param>
        /// <param name="denom">denominator</param>
        /// <returns>the result of num / denom, or null if b is 0</returns>
        public static double? SafeDiv(this int num, int denom)
        {
            return denom == 0 ? null : ((double)num / (double)denom).AsNullable();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        static Expression<Func<int, int, double?>> SafeDivExpr = (num, denom) => denom == 0 ? null : ((double?)(num * 100 / denom)) * 0.01;
    }
}
