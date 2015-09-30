using PluginCore.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace PluginCore.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string input)
        {
            decimal value;
            return decimal.TryParse(input, out value);
        }

        /// <summary>
        /// Determines whether the specified string is an integer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if the specified input is an integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(this string input)
        {
            int value;
            return int.TryParse(input, out value);
        }

        /// <summary>
        /// If a string match can be made, only change the first occurrence of the removeValue with newValue.
        /// </summary>
        /// <param name="input">The string that will be changed</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace with</param>
        /// <returns>String</returns>
        /// <remarks>"Pass" replace s with X. Result: "PaXs"</remarks>
        public static string ReplaceFirstOccurrence(this string input, string oldValue, string newValue)
        {
            if (!input.HasValue() || !oldValue.HasValue())
                return input;

            int pos = input.IndexOf(oldValue);
            if (pos < 0)
                return input;

            return input.Remove(pos, oldValue.Length).Insert(pos, newValue);
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="input">The string that will be changed</param>
        /// <param name="oldValue">The string to be replaced</param>
        /// <param name="newValue">The string to replace with</param>
        /// <param name="comparison">The StringComparison</param>
        /// <returns>String</returns>
        /// <remarks>Can be used to do a case insensitive replace. Didn't use RegEx as there may be characters that might interfere with the RegEx.</remarks>
        public static string Replace(this string input, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (!input.HasValue() || !oldValue.HasValue())
                return input;

            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = input.IndexOf(oldValue, comparisonType);
            while (index != -1)
            {
                sb.Append(input.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = input.IndexOf(oldValue, index, comparisonType);
            }
            sb.Append(input.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// Given an existing string, append a string. "a".append("z") returns "az"
        /// </summary>
        /// <param name="input">original string</param>
        /// <param name="value">string to be appended to the end of the original string</param>
        /// <returns>Concatenated string</returns>
        /// <remarks></remarks>
        public static string Append(this string input, string value)
        {
            return string.Concat(input, value);
        }

        /// <summary>
        /// Get the QueryString part of a URL. http://microsoft.com?v=100 will return v=100
        /// Supports schemed, schemeless or relative paths. (Doesn't use new URI())
        /// </summary>
        public static string QueryPart(this string url)
        {
            if (!url.HasValue())
                return String.Empty;

            var queryStart = url.IndexOf('?');
            if (queryStart > -1 && queryStart != url.Length)
                return url.Substring(queryStart + 1);

            return String.Empty;
        }

        /// <summary>
        /// Get the left part of a URL removing the QueryString part. http://microsoft.com?v=100 will http://microsoft.com.
        /// Supports schemed, schemeless or relative paths. (Doesn't use new URI())
        /// </summary>
        public static string StripQueryPart(this string url)
        {
            if (!url.HasValue())
                return String.Empty;

            var queryStart = url.IndexOf('?');
            if (queryStart > -1)
                return url.Substring(0, queryStart);

            return url;
        }

        /// <summary>
        /// http://something.com AppendUrlPart
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AppendUrlPart(this string input, string value)
        {
            if (!input.HasValue() && !value.HasValue())
                return string.Empty;

            if (!input.HasValue())
                return value;

            if (!value.HasValue())
                return input;

            string result = String.Concat(VirtualPathUtility.AppendTrailingSlash(input), (value.StartsWith("/") ? value.TrimStart('/') : value));
            return result;
        }

        /// <summary>
        /// http://something.com AppendQueryPart adds "?value" or "&value"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AppendQueryPart(this string input, string value)
        {
            if (!value.HasValue())
                return input;

            if (value.StartsWith("?"))
                value = value.TrimStart('?');

            string result = (input.IndexOf("?") > -1)
                ? String.Format("{0}&{1}", input, value)
                : String.Format("{0}?{1}", input, value);
            return result;
        }

        /// <summary>
        /// http://something.com AppendQueryPart adds "?value" or "&value"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AppendQueryPart(this string input, string key, string value)
        {
            string queryPart = String.Format("{0}={1}", key, value);
            return input.AppendQueryPart(queryPart);
        }

        /// <summary>
        /// Remove a subdomain from a URL. Has TestCoverage RemoveSubdomainPart_Should_Remove_Subdomain().
        /// Supports Schemeless URLs like //www.microsoft.com
        /// Converts 'scheme://www.microsoft.com' -> 'scheme://microsoft.com'
        /// </summary>
        public static string UrlRemoveSubdomainPart(this string url)
        {
            // BCR this is janky
            int skip = (url.IndexOf("//") > -1)
                ? url.IndexOf("//") + 2
                : 0;

            if (url.IndexOf('.') > -1)
                return url.Remove(skip, url.IndexOf('.') - skip + 1); // scheme://[remove].something.com
            else
                return url;
        }

        /// <summary>
        /// Remove scheme from a URL. Has TestCoverage RemoveSubdomainPart_Should_Remove_Subdomain().
        /// Converts 'scheme://www.microsoft.com' -> '//www.microsoft.com'
        /// </summary>
        public static string UrlRemoveScheme(this string url)
        {
            if (url.IndexOf("http://", StringComparison.InvariantCultureIgnoreCase) == -1 &&
                url.IndexOf("https://", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                return url;
            }

            string result = url.Replace("http://", "//")
                .Replace("https://", "//");
            return result;
        }

        /// <summary>
        /// Will remove a part of a string from the first occurrence of the value string.
        /// </summary>
        /// <example>
        /// Implementation: "Sample".RemoveFrom("m");
        /// Output: "Sa"
        /// </example>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveFrom(this string input, string value)
        {
            int indexOfValue = input.IndexOf(value);
            if (indexOfValue <= -1)
            {
                return input;
            }
            return input.Remove(indexOfValue);
        }

        /// <summary>
        /// Will find all occurrences of removeValue and remove them.
        /// </summary>
        /// <example>
        /// Implementation: "catt".Remove("t");
        /// Output: "ca"
        /// </example>
        /// <param name="input">The string to manipulate.</param>
        /// <param name="removeValue">The string value to be removed</param>
        /// <returns></returns>
        public static string Remove(this string input, string removeValue)
        {
            return input.Replace(removeValue, String.Empty);
        }

        /// <summary>
        /// Will find the last chars in string and if it matches the "characters" param then it removes it
        /// </summary>
        /// <example>
        /// Implementation: "catt".Remove("t");
        /// Output: "ca"
        /// </example>
        /// <param name="input">The string to manipulate.</param>
        /// <param name="removeValue">The string value to be removed</param>
        /// <returns></returns>
        public static string RemoveLastIfLast(this string input, string characters)
        {
            if (input.LastIndexOf(characters) == (input.Length - characters.Length))
                return input.Substring(0, input.Length - characters.Length);
            return input;
        }

        public static string RemoveBetween(this string input, string from, string to)
        {
            string result = input.RemoveEx(input.IndexOf(from) + from.Length,
                input.IndexOf(to) - (input.IndexOf(from) + from.Length));
            return result;
        }

        public static string RemoveBetweenInclusive(this string input, string from, string to)
        {
            string result = input.RemoveEx(input.IndexOf(from),
                (input.IndexOf(to) + to.Length) - (input.IndexOf(from)));
            return result;
        }

        /// <summary>
        /// (BCR) Uses a RegularExpression to pattern match that the supplied string is a valid email address.
        /// </summary>
        /// <remarks>Also see http://www.regular-expressions.info/email.html for information on email address validation with regex.</remarks>
        /// <see cref="http://blog.sinjakli.co.uk/2011/02/13/email-address-validation-please-stop/"/>
        /// <seealso cref="http://www.regular-expressions.info/email.html"/>

        public static bool IsValidEmailAddress(this string input)
        {
            if (!input.HasValue())
                return false;

            var regex = RegexEx.GetOrCreate(Consts.EmailAddressRegPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// (DS) Uses a regular expression to pattern match that the supplied string is a valid identifier - (no spaces, special characters, only allows - _ and alphanumeric characters)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValidIdentifier(this string input)
        {
            var regex = RegexEx.GetOrCreate(Consts.IdentiferRegPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        /// <example>
        /// Implementation: "cat".Truncate(1);
        /// Output: "c"
        /// </example>
        public static string Truncate(this string source, int length)
        {
            return source.Length <= length ? source : source.Substring(0, length);
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        /// <example>
        /// Implementation: "cat".Truncate(1);
        /// Output: "c"
        /// </example>
        public static string Elipsis(this string source, int length)
        {
            return source.EmptyIfNull().Length <= length ? source : source.Substring(0, length) + "...";
        }

        //from Newtonsoft.Json.Utilities.StringUtilities
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                bool hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || char.IsUpper(s[i + 1]))
                {
                    char lowerCase;
#if !(NETFX_CORE || PORTABLE)
                    lowerCase = char.ToLower(s[i], System.Globalization.CultureInfo.InvariantCulture);
#else
                    lowerCase = char.ToLower(s[i]);
#endif

                    sb.Append(lowerCase);
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Write a string to a file. (UTF8 encoded)
        /// </summary>
        /// <param name="s">The string to write.</param>
        /// <param name="file">Full Path of the file to write to.</param>
        public static void ToFile(this string s, FileInfo file)
        {
            File.WriteAllText(file.FullName, s, Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes a string to an instance of T. Alias of DeserializeToObject.
        /// </summary>
        /// <typeparam name="T">The type of the object you're dealing with.</typeparam>
        /// <param name="s">String to deserialize.</param>
        /// <returns>An instance of T</returns>
        public static T Deserialize<T>(this string s) where T : class
        {
            return (T)DeserializeToObject(s, typeof(T));
        }

        /// <summary>
        /// Deserializes to a Single item from a String.
        /// </summary>
        /// <typeparam name="T">The type of the object you're dealing with.</typeparam>
        /// <param name="s">String to deserialize.</param>
        /// <returns>A object of T</returns>
        public static T DeserializeToObject<T>(this string s) where T : class
        {
            return (T)DeserializeToObject(s, typeof(T));
        }

        /// <summary>
        /// Deserializes to a Single item from a String.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object DeserializeToObject(this string s, Type objectType)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return null;
            }
            var serializer = new XmlSerializer(objectType);
            using (var stream = new StringReader(s))
            {
                return serializer.Deserialize(stream);
            }
        }


        /// <summary>
        /// Deserializes to a List from an XML string.
        /// If string is empty no deserialization will be attempted and null will be returned.
        /// </summary>
        /// <typeparam name="T">Type of a List Item</typeparam>
        /// <param name="items">The list of to deserialize</param>
        /// <param name="s">The string to deserialize from.</param>
        /// <returns></returns>
        public static IList<T> DeserializeToList<T>(this string s) where T : class
        {
            if (String.IsNullOrEmpty(s)) return null;

            var items = new List<T>();
            var serializer = new XmlSerializer(items.GetType());
            using (var reader = new StringReader(s))
            {
                items = (List<T>)serializer.Deserialize(reader);
            }
            return items;
        }

        /// <summary>
        /// HTML decode.
        /// </summary>
        /// <example>
        /// Input: "&lt;div ID=&quot;ctrl1&quot; runat=&quot;server&quot;/&gt;".HtmlDecode
        /// Output: "<div ID=\"ctrl1\" runat=\"server\"/>"
        /// </example>
        /// <param name="target">The string to HTML decode.</param>
        /// <returns></returns>
        public static string HtmlDecode(this string target)
        {
            return HttpUtility.HtmlDecode(target);
        }

        /// <summary>
        /// HTML encode.
        /// </summary>
        /// <example>
        /// Input: "<div ID=\"ctrl1\" runat=\"server\"/>".HtmlEncode
        /// Output: "&lt;div ID=&quot;ctrl1&quot; runat=&quot;server&quot;/&gt;"
        /// </example>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static string HtmlEncode(this string target)
        {
            return HttpUtility.HtmlEncode(target);
        }

        /// <summary>
        /// Check if string is Guid string
        /// </summary>
        /// <example>
        /// Input: "F7139E92-91B7-49DD-8607-87602427763B".ToGuid()
        /// Output: true
        /// </example>
        /// <param name="guidStr">string to detect</param>
        /// <returns></returns>
        public static bool IsGuidString(this string guidStr)
        {
            Guid guid = Guid.Empty;
            return Guid.TryParse(guidStr.Trim(new char[] { '{', '}' }), out guid);
        }

        /// <summary>
        /// Convert string to Guid
        /// </summary>
        /// <example>
        /// Input: "F7139E92-91B7-49DD-8607-87602427763B".ToGuid()
        /// </example>
        /// <param name="guidStr">string to convert</param>
        /// <returns></returns>
        public static Guid ToGuid(this string guidStr)
        {
            // had to fix a bug for a case when guidStr was null
            return GuidEx.From(guidStr).GetValueOrDefault();
        }

        public static Guid? AsGuid(this string guidStr)
        {
            var guidOpt = GuidEx.From(guidStr);
            return guidOpt.IsEmpty() ? guidOpt.Value : Guid.Empty;
        }

        /// <summary>
        /// Determines whether a string is composed of two or more guids
        /// </summary>
        /// <param name="input"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static bool IsGuidList(this string input, params char[] delimiter)
        {
            Guid _;
            if (delimiter.IsEmpty()) delimiter = new[] { ',' };
            return input.Split(delimiter).All(v => Guid.TryParse(v, out _));
        }

        /// <summary>
        /// return guid from doc reference string
        /// </summary>
        /// <param name="docRef"></param>
        /// <returns></returns>
        public static Guid? ToDocRefGuid(this string docRef)
        {
            if (!docRef.HasValue())
            {
                return null;
            }
            var refParts = docRef.Split('_');
            return refParts.Length == 2 ?
                refParts[1].ToGuid() :
                (Guid?)null;

        }

        public static string Truncate(this string s, int length, bool atWord = true, bool addEllipsis = true)
        {
            // Return if the string is less than or equal to the truncation length 
            if (s == null || s.Length <= length)
                return s;

            // Do a simple tuncation at the desired length 
            string s2 = s.Substring(0, length);

            // Truncate the string at the word
            if (atWord)
            {
                // List of characters that denote the start or a new word (add to or remove more as necessary) 
                List<char> alternativeCutOffs = new List<char>() { ' ', ',', '.', '?', '/', ':', ';', '\'', '\"', '\'', '-' };

                // Get the index of the last space in the truncated string 
                int lastSpace = s2.LastIndexOf(' ');

                // If the last space index isn't -1 and also the next character in the original 
                // string isn't contained in the alternativeCutOffs List (which means the previous
                // truncation actually truncated at the end of a word),then shorten string to the last space
                if (lastSpace != -1 && (s.Length >= length + 1 && !alternativeCutOffs.Contains(s.ToCharArray()[length])))
                    s2 = s2.Remove(lastSpace);
            }

            // Add Ellipsis if desired
            if (addEllipsis)
                s2 += "...";
            return s2;
        }

        /// <summary>
        ///  returns inner text of html
        /// </summary>
        /// <param name="value">html to get inner text of</param>
        /// <returns>inner text of html</returns>
        public static string InnerTextOfHtml(this string value)
        {
            var rege = RegexEx.GetOrCreate(@"<[^>]*>", System.Text.RegularExpressions.RegexOptions.Compiled);
            return rege.Replace(value.EmptyIfNull(), string.Empty);
        }

        /// <summary>
        /// replicates microsoft get field functionality
        /// </summary>
        /// <param name="value"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetField(this string value, string id)
        {
            string sId = null;
            string sFieldValue = string.Empty;
            int i;
            int j;
            sId = string.Format("*~-{0}-~*", id.ToLower());
            i = value.ToLower().IndexOf(sId);
            if (i > -1)
            {
                j = value.IndexOf("*~-", (i + 3));
                if (j == -1)
                {
                    j = value.Length - i - sId.Length;
                }
                else
                {
                    j = j - i - sId.Length;
                }
                sFieldValue = value.Substring(i + sId.Length, j);
            }

            return sFieldValue;
        }

        public static int GetFieldAsInt(this string value, string id)
        {
            try
            {
                string sVal = value.GetField(id);
                return !string.IsNullOrEmpty(sVal) ? Convert.ToInt32(sVal) : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string SetField(this string source, string key, string value)
        {
            int i = 0;
            int j = 0;
            string sId = string.Format("*~-{0}-~*", key);
            i = source.ToLower().IndexOf(sId.ToLower());
            if (i > -1)
            {
                j = source.IndexOf("*~-", (i + 3));
                if (j == -1)
                {
                    source = source.Insert(i + sId.Length, value);
                }
                else
                {
                    source = source.Remove(i + sId.Length, (j - (i + sId.Length)));
                    source = source.Insert(i + sId.Length, value);
                }
            }
            else
            {
                source = string.Format("{0}{1}{2}", source, sId, value);
            }
            return source;
        }

        public static MemoryStream ToStream(this string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        /// <summary>
        ///  safe (null aware) version of String.Length
        /// </summary>
        /// <param name="value">string to calculate the length of</param>
        /// <returns>number of characters in the given string</returns>
        public static int LengthEx(this string value)
        {
            return value == null ? 0 : value.Length;
        }

        /// <summary>
        ///  safe version of String.Remove
        /// </summary>
        /// <param name="value">string to remove from</param>
        /// <param name="startIndex">start index of the range to remove</param>
        /// <param name="count">number of characters in the range to remove</param>
        /// <returns>string with the section [startIndex, startIndex + count[ range removed</returns>
        public static string RemoveEx(this string value, int startIndex, int count)
        {
            if (!startIndex.BetweenEx(0, value.LengthEx() - 1))
                return value;

            count = Math.Min(count, value.LengthEx() - startIndex);
            if (count <= 0)
                return value;

            return value.Remove(startIndex, count);
        }

        /// <summary>
        ///  removes the strToTrim string from the lhs of the given string; the value is returned untouched if it doesnt start with the strToTrim string.
        /// </summary>
        /// <param name="value">string to trim on the left</param>
        /// <param name="strToTrim">string to trim</param>
        public static string TrimLeft(this string value, string strToTrim, bool ignoreCase = false)
        {
            if (!value.StartsWithEx(strToTrim, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                return value;

            return value.RemoveEx(0, strToTrim.LengthEx());
        }

        /// <summary>
        ///  removes the strToTrim string from the rhs of the given string; the value is returned untouched if it doesnt end with the strToTrim string.
        /// </summary>
        /// <param name="value">string to trim on the right</param>
        /// <param name="strToTrim">string to trim</param>
        public static string TrimRight(this string value, string strToTrim, bool ignoreCase = false)
        {
            if (!value.EndsWithEx(strToTrim, ignoreCase))
                return value;

            return value.RemoveEx(value.LengthEx() - strToTrim.LengthEx(), Int32.MaxValue);
        }

        /// <summary>
        ///  null-aware version of ToString()
        /// </summary>
        public static string ToStringEx(this object o)
        {
            return o == null ? null : o.ToString();
        }

        /// <summary>
        ///  safe (null-aware) version of String.Replace
        /// </summary>
        /// <param name="value">target string</param>
        /// <param name="oldValue">sub-string occurencies of which to replace</param>
        /// <param name="newValue">sub-string to replace found occurencies by</param>
        /// <returns>A string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.</returns>
        public static string ReplaceEx(this string value, string oldValue, string newValue)
        {
            if (!oldValue.HasValue())
                return value;

            return value.EmptyIfNull().Replace(oldValue, newValue.EmptyIfNull());
        }

        /// <summary>
        /// removes spaces from the start end the end of the string; null-aware
        /// </summary>
        /// <param name="v">string to trim</param>
        public static string TrimEx(this string v)
        {
            return v == null ? null : v.Trim();
        }

        /// <summary>
        ///  null-aware String.StartsWith()
        /// </summary>
        /// <param name="thisValue">"this" string</param>
        /// <param name="otherValue">string to compare to</param>
        /// <returns>true if "thisValue" string starts with "otherValue" string</returns>
        public static bool StartsWithEx(this string thisValue, string otherValue)
        {
            return thisValue.EmptyIfNull().StartsWith(otherValue.EmptyIfNull());
        }

        /// <summary>
        ///  null-aware String.StartsWith()
        /// </summary>
        /// <param name="thisValue">"this" string</param>
        /// <param name="otherValue">string to compare to</param>
        /// <returns>true if "thisValue" string starts with "otherValue" string</returns>
        public static bool StartsWithEx(this string thisValue, string otherValue, StringComparison comparison)
        {
            return thisValue.EmptyIfNull().StartsWith(otherValue.EmptyIfNull(), comparison);
        }

        /// <summary>
        ///  null-aware String.EndsWith()
        /// </summary>
        /// <param name="thisValue">"this" string</param>
        /// <param name="otherValue">string to compare to</param>
        /// <returns>true if "thisValue" string ends with "otherValue" string</returns>
        public static bool EndsWithEx(this string thisValue, string otherValue, bool ignoreCase = false)
        {
            return thisValue.EmptyIfNull().EndsWith(otherValue.EmptyIfNull(), ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
        }

        /// <summary>
        /// <para> Shortcut for !String.IsNullOrEmpty()</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="value">string to check</param>
        /// <returns>true if the string is neither null or ""</returns>
        public static bool HasValue(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// <para> Concatenates a and b using the separator in between if both a and b arent empty; otherwise just the result of a + b</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="a">first string for concatenation</param>
        /// <param name="b">second string for concatenation</param>
        /// <param name="separator">separator to be inserted between a and b if both a and b arent empty</param>
        /// <returns>a + separator + b if a and b arent empty; otherwise - a + b</returns>
        public static string ConcatWithSeparator(this string a, string b, string separator)
        {
            return a.HasValue() && b.HasValue() ? a + separator + b : a + b;
        }

        /// <summary>
        ///  Returns string.Empty if this string is null
        /// </summary>
        /// <param name="value">this string</param>
        /// <returns>given value if not null, otherwise ""</returns>
        public static string EmptyIfNull(this string value)
        {
            return value ?? String.Empty;
        }

        /// <summary>
        ///  Returns null if the value is an empty string or null, otherwise - the given value
        /// </summary>
        /// <param name="value">this string</param>
        /// <returns>null if the value is an empty string or null, otherwise - the given value</returns>
        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// <para> checks if this string has a value, and if so returns this string value, otherwise - the alternative altValue</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="altValue">alternative value to return if this string is null or empty</param>
        /// <returns>this string value if it is not empty, otherwise - the alternative altValue</returns>
        public static string Alt(this string value, string altValue)
        {
            return value.HasValue() ? value : altValue;
        }

        /// <summary>
        ///  checks if this string has a value, and if so returns this string value, otherwise - the alternative altValue
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="altValue">alternative value to return if this string is null or empty</param>
        /// <returns>this string value if it is not empty, otherwise - the alternative altValue</returns>
        public static string Alt(this string value, Func<string> altValue)
        {
            return value.HasValue() ? value : altValue();
        }

        /// <summary>
        ///  Returns true if this string matches the other - case-insensitive
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="other">other string</param>
        public static bool EqTxt(this string value, string other)
        {
            return value.EmptyIfNull().Equals(other, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///  Returns parts of the given value separated with the given separator. Empty items are excluded if excludeEmptyParts is true.
        /// </summary>
        /// <param name="value">the value for which the parts are calculated</param>
        /// <param name="separator">separator which separates the parts in the given value</param>
        /// <param name="excludeEmptyParts">true to exclude empty parts</param>
        /// <returns>seq of parts within the given value</returns>
        public static IEnumerable<string> GetParts(this string value, string separator, bool excludeEmptyParts)
        {
            if (!(value.HasValue() && separator.HasValue()))
                return Enumerable.Empty<string>();

            var options = excludeEmptyParts ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            var res = value.Split(new[] { separator }, options);
            return res;
        }

        /// <summary>
        ///  Returns parts of the given value separated with the given separator. Empty items are excluded.
        /// </summary>
        /// <param name="value">the value for which the parts are calculated</param>
        /// <param name="separator">separator which separates the parts in the given value</param>
        /// <returns>seq of parts within the given value</returns>
        public static IEnumerable<string> GetParts(this string value, string separator)
        {
            return value.GetParts(separator, true);
        }

        /// <summary>
        ///  null-aware version of String.Contains(string)
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="subString">subString to find in this string</param>
        /// <returns>true if the specified subString occurs within this string.</returns>
        public static bool ContainsEx(this string value, string subString)
        {
            if (!subString.HasValue())
                return false;

            return value.EmptyIfNull().Contains(subString);
        }

        /// <summary>
        ///  Returns a value indicating whether the specified System.String object occurs within this string.
        /// </summary>
        /// <param name="value">this string</param>
        /// <param name="subString">subString to find in this string</param>
        /// <param name="comparison">comparison to use when looking for the sub-string</param>
        /// <returns>true if the specified subString occurs within this string.</returns>
        public static bool ContainsEx(this string value, string subString, StringComparison comparison)
        {
            if (!subString.HasValue())
                return false;

            return value.EmptyIfNull().IndexOf(subString, comparison) >= 0;
        }

        /// <summary>
        /// Retrieves a text range from this instance which starts at a specified character position and has a specified length.
        /// </summary>
        /// <param name="value">this string instance</param>
        /// <param name="startIndex">0-based index of the first character in the range</param>
        /// <param name="length">number of characters in the range</param>
        /// <returns>text range from this instance which starts at a specified character position and has a specified length.</returns>
        public static string SubstringEx(this string value, int startIndex, int length)
        {
            if (startIndex >= value.LengthEx() || length <= 0)
                return null;

            length = Math.Min(length, value.LengthEx() - startIndex);
            if (length <= 0)
                return null;

            return value.Substring(startIndex, length);
        }

        /// <summary>
        /// Retrieves a text range from this instance which starts at a specified character position and ends at the end of the string - last character is included.
        /// </summary>
        /// <param name="value">this string instance</param>
        /// <param name="startIndex">0-based index of the first character in the range</param>
        /// <returns>text range from this instance which starts at a specified character position and ends at the end of the string - last character is included.</returns>
        public static string SubstringEx(this string value, int startIndex)
        {
            return value.SubstringEx(startIndex, Int32.MaxValue);
        }

        /// <summary>
        /// Returns a copy of this string converted to uppercase.
        /// </summary>
        /// <param name="value">this</param>
        /// <returns>The uppercase equivalent of the current string.</returns>
        public static string ToUpperEx(this string value)
        {
            return value == null ? value : value.ToUpper();
        }

        /// <summary>
        /// Returns a copy of this string converted to lowercase.
        /// </summary>
        /// <param name="value">this</param>
        /// <returns>The lowercase equivalent of the current string.</returns>
        public static string ToLowerEx(this string value)
        {
            return value == null ? null : value.ToLower();
        }

        /// <summary>
        ///  converts ABC to aBC - or original value if its length is less than 1
        /// </summary>
        /// <param name="value">value first character of which is to be converted to lower case</param>
        /// <returns>given ABC returns aBC - or original value if its length is less than 1</returns>
        public static string WithFirstCharInLowerCase(this string value)
        {
            return value.LengthEx() == 0 ? value : char.ToLower(value[0]) + value.SubstringEx(1);
        }

        /// <summary>
        /// <para>Replaces each sourceCharMap[i] character found in the string with corresponding destCharMap[i] character.</para>
        /// <para>If i >= destCharMap.Length, uses last character in destCharMap.</para>
        /// <para>If destCharMap is empty, removes the character</para>
        /// </summary>
        public static string Translate(this string text, string sourceCharMap, string destCharMap)
        {
            text = text.EmptyIfNull();
            sourceCharMap = sourceCharMap.EmptyIfNull();
            destCharMap = destCharMap.EmptyIfNull();

            var stringBuilder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                int p = sourceCharMap.IndexOf(text[i]);
                if (p == -1)
                    stringBuilder.Append(text[i]); //no translation
                else if (destCharMap.Length != 0) //translate to mathing or last index; remove if dest map is empty
                    stringBuilder.Append(destCharMap[Math.Min(p, destCharMap.Length - 1)]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// <para>Converts this string in notation form (identifier) to display form. The process consists of the following steps:</para>
        /// <para>1. Following characters are replaced with one white space character: "_?!.,()[]{}\n"</para>
        /// <para>2. # gets replaced by "No"</para>
        /// <para>3. / or \ get replaced by " or "</para>
        /// <para>4. the string is trimmed</para>
        /// <para>5. Double spaces are replaced with a single space</para>
        /// <para>6. If all string is in lower case, the first character gets upper-cased</para>
        /// <para>7. if whole string is in upper case, and dontChangeCaseIfAllUpperCase is false, the result is similar to this conversion: "IS DG HERE BTW" => "Is Dg Here Btw"</para>
        /// <para>8. if the string  contains low case characters, or dontChangeCaseIfAllUpperCase is true, the result is similar to this conversion: "IsDGHereBTW" => "Is DG Here BTW"</para>
        /// <para>9. Finally, if aAllowSpaces is false, it removes following characters: " .&amp;' and replaces "-" with "_".</para>
        /// </summary>
        public static string NotationForm2DisplayForm(this string aText, bool aAllowSpaces, bool dontChangeCaseIfAllUpperCase)
        {
            var res =
                aText
                .Translate("_?!.,()[]{}\n", " ")
                .ReplaceEx("#", "No")
                .ReplaceEx("/", " or ")
                .ReplaceEx("\\", " or ")
                .TrimEx()
                .EmptyIfNull();

            while (res.IndexOf("  ") != -1)
                res = res.ReplaceEx("  ", " ");

            if (!res.HasValue())
                return res;

            if (res.ToLower() == res)
                res = res.Substring(0, 1).ToUpper() + res.Substring(1);

            if (res == res.ToUpper())
            {
                //all uppercase, something like: "IS DG HERE BTW", convert to "Is Dg Here Btw"
                if (!dontChangeCaseIfAllUpperCase)
                {
                    res = res.ToLower();
                    var aBuilder = new StringBuilder(res);

                    for (int k = aBuilder.Length - 2; k >= 0; k--)
                        if (aBuilder[k] == ' ')
                            aBuilder[k + 1] = Char.ToUpper(aBuilder[k + 1]);

                    if (aBuilder.Length > 0)
                        aBuilder[0] = Char.ToUpper(aBuilder[0]);

                    res = aBuilder.ToString();
                }
            }
            else
            {
                //something like "IsDGHereBTW", convert to "Is DG Here BTW"
                var i = res.Length - 1;
                do
                {
                    /* find char at i in different case than i - 1 */
                    while (i > 1 && !(Char.IsLetterOrDigit(res[i]) && Char.IsLetterOrDigit(res[i - 1]) && Char.IsUpper(res[i]) != Char.IsUpper(res[i - 1])))
                        i--;

                    if (i <= 1)
                        break;

                    /* insert space in fron of the capital char */
                    res = res.Insert(Char.IsUpper(res[i]) ? i : i - 1, " ");

                    i--;
                }
                while (true);
            }

            if (!aAllowSpaces)
                res =
                    res
                    .ReplaceEx(" ", "")
                    .ReplaceEx("-", "_")
                    .ReplaceEx(".", "")
                    .ReplaceEx("&", "");

            return res;
        }

        public static string NotationForm2DisplayForm(this string value)
        {
            return value.NotationForm2DisplayForm(true, true);
        }

        /// <summary>
        /// <para> surrounds the given value with 'left' value on the left and 'right' value on the right, if the value isnt blank, in which case it returns the empty value</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="value">value to surrond</param>
        /// <param name="left">prefix</param>
        /// <param name="right">postfix</param>
        /// <returns>value surrouned with 'left' value on the left and 'right' value on the right, if the value isnt blank, in which case it returns the empty value</returns>
        public static string Surround(this string value, string left, string right)
        {
            return value.HasValue() ? left + value + right : value;
        }

        /// <summary>
        /// <para> returns true if the character at the given position in the string is a number</para>
        /// <para>*expandable</para>
        /// </summary>
        /// <param name="value">the string to check for a number character in</param>
        /// <param name="index">index (0-based) of the character in the string to check if it is a number </param>
        /// <returns>true if the character at the given position in the string is a number</returns>
        public static bool IsNumberCharAt(this string value, int index)
        {
            return value.LengthEx() > index && char.IsNumber(value[index]);
        }

        /// <summary>
        /// Determines if the string is made up of alpha numeric characters only (Spaces are allowed)
        /// </summary>
        /// <param name="s">The string to test for alpha numeric only characters</param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string value)
        {
            if (string.IsNullOrEmpty(value)) return true;

            var r = new Regex("^(\\w| )+$");
            return r.IsMatch(value);
        }

        /// <summary>
        /// Determines if the string is made up of alpha numeric characters only (Spaces are allowed)
        /// </summary>
        /// <param name="s">The string to test for alpha numeric only characters</param>
        /// <returns></returns>
        public static bool IsValidFirstLastName(this string value)
        {
            if (string.IsNullOrEmpty(value)) return true;

            var r = new Regex("^(\\w|[- '\\(\\)])+$");
            return r.IsMatch(value);
        }

        /// <summary>
        /// Determines if the string is made up of ASCII only characters
        /// </summary>
        /// <param name="s">The string to test for ASCII only characters</param>
        /// <returns></returns>
        public static bool IsValidAscii(this string s)
        {
            for (var i = (s == null ? 0 : s.Length) - 1; i >= 0; i--)
            {
                var c = s[i];

                // have to consider \r and \n as they are part of the 'valid' input
                switch (c)
                {
                    case '\n':
                    case '\r':
                    case '\t':
                        continue;
                }

                var inValidRange = ((int)c).BetweenEx(32, 126);
                if (!inValidRange)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// (TB) convert string to an HTML safe identifier (url friendly format, no special characters or spaces)
        /// </summary>
        /// <param name="baseId"></param>
        /// <returns></returns>
        public static string ToIdentifier(this string src)
        {
            // handling blank baseId parameter
            if (!src.HasValue())
                return src;

            src = src.ToLower();

            //Replace contiguous invalid chars with hyphen
            src = Regex.Replace(src, @"[^a-z0-9_]+", "-", RegexOptions.None);

            //Remove leading/trailing hypen
            src = Regex.Replace(src, @"^[-_]+|[-_]+$", "");

            //remove special characters, leaving only alphanumeric characters
            src = Regex.Replace(src, @"[^a-z0-9-]+", "", RegexOptions.Compiled);

            //Remove leading numbers
            //(TB): Removed because we want to allow leading numbers in identifiers,
            //(TB): despite this producing invalid html identifiers
            //src = Regex.Replace(src, @"^[0-9]+\-*", "");

            return src;
        }

        /// <summary>
        /// Transforms a string into a JavaScript safe function name
        /// </summary>
        public static string ToJsFunctionName(this string str, bool isConstructor)
        {
            //Remove leading numbers (followed by hypens)
            str = Regex.Replace(str, @"^[0-9]+\-*", "");

            //Split the string into a list of "words"
            var parts = str.Split(new[] { " ", "-", "_", "." }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //If the function name is for a constructor uppercase the all words, 
            //else uppercase from the second word onwards
            var startIndex = (isConstructor) ? 0 : 1;
            for (int i = startIndex; i < parts.Count; i++)
            {
                parts[i] = parts[i].UpperFirstChar();
            }
            return parts.Concatenate();
        }

        public static string UrlEncode(this string s)
        {
            s = HttpUtility.UrlEncode(s).Replace("+", "%20");
            return s;
        }

        public static string UrlDecode(this string s)
        {
            try
            {
                return System.Web.HttpUtility.UrlDecode(s);
            }
            catch
            {

                return s;
            }
        }

        /// <summary>
        ///  changes the first character of the given string to the upper case
        /// </summary>
        /// <param name="value">string to upper-case the first character in</param>
        /// <returns>original string with the first character upper-cased</returns>
        public static string UpperFirstChar(this string value)
        {
            return value.SubstringEx(0, 1).ToUpperEx() + value.SubstringEx(1);
        }

        /// <summary>
        /// BCR: Returns the MD5 hash of a string
        /// </summary>
        public static string ComputeGetMD5Hash(this string input)
        {
            using (var cryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                byte[] bs = Encoding.UTF8.GetBytes(input);
                bs = cryptoServiceProvider.ComputeHash(bs);
                return bs.EmptyIfNull().Select(b => b.ToString("x2")).Concatenate().ToLowerEx();
            }
        }

        /// <summary>
        /// <para> returns hash value for the given string which uniquely identifies it: hashes of two sets of data should match if (and generally but not always only if) the corresponding data also matches.</para>
        /// <para>uses MD5 hash algorithm (which by the look of it is the fastest atm)</para>
        /// <para>expensive, cache the result where possible (can take up to 100ms to calculate a hash for a string 1M characters long)</para>
        /// </summary>
        /// <param name="value">value to calculate the hash value for</param>
        /// <returns>
        /// <para>hash value for the given string which uniquely identifies it: hashes of two sets of data should match if (and generally but not always only if) the corresponding data also matches.</para>
        /// <para>null if the given value is blank</para>
        /// </returns>
        public static string ContentHashValue(this string value)
        {
            if (!value.HasValue())
                return null;

            var hashValue = FuncExtensions.Invoke(() =>
            {
                var content = Encoding.Unicode.GetBytes(value);
                using (var md5 = MD5.Create())
                    return md5.ComputeHash(content);
            });

            var hashValueEncoded = Convert.ToBase64String(hashValue);
            return hashValueEncoded;
        }

        /// <summary>
        /// Encrypts a string and returns a base64 encoded representation of the bytes produced.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="password">The password to encrpyt the string with</param>
        /// <param name="salt">A salt value (can be any string)</param>
        /// <param name="passwordIterations">Can be any number</param>
        /// <param name="initialisationVector">Has to be 16 bytes (16 ASCII chars) for AES.</param>
        /// <param name="keySize">Size of key to generate, e.g. 256, 192, 128</param>
        /// <returns></returns>
        public static string AesEncrypt(this string plainText, string password, string salt, int passwordIterations, string initialisationVector, int keySize)
        {
            // Need byte arrays for crypto classes. Assume these values will be ASCII.
            var initialisationVectorBytes = Encoding.ASCII.GetBytes(initialisationVector);
            var saltBytes = Encoding.ASCII.GetBytes(salt);

            // Convert plaintext to byte array. This might contain non-ascii characters so use UTF8 for the conversion.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //Use crypto libs to create a password.
            using (var passwordDerive = new Rfc2898DeriveBytes(password, saltBytes, passwordIterations))
            // instantiate crypto objects
            using (var symmetricKey = new AesManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                // key is psuedo random bytes derived from password
                var keyBytes = passwordDerive.GetBytes(keySize / 8);

                var encryptor = symmetricKey.CreateEncryptor(keyBytes, initialisationVectorBytes);

                using (var memoryStream = DisposeOnce.Create(() => new MemoryStream()))
                //unencrypted data is sensitive so we put it in a cryptostream which won't keep it in memory
                using (var cryptoStream = new CryptoStream(memoryStream.Inner, encryptor, CryptoStreamMode.Write))
                {
                    memoryStream.DisposableOwnershipIsTakenAway();

                    //encrypt
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    var cipherTextBytes = memoryStream.Inner.ToArray();

                    var cipherText = Convert.ToBase64String(cipherTextBytes);
                    return cipherText;
                }
            }
        }

        /// <summary>
        /// Decrypts a string. Parameters must match those used to encrypt the string.
        /// </summary>
        /// <param name="cipherText">string to decrypt</param>
        /// <param name="password">The password used to encrpyt the string</param>
        /// <param name="salt">A salt value used to encrypt (can be any string)</param>
        /// <param name="passwordIterations">Password iterations used in encryption. Can be any number</param>
        /// <param name="initialisationVector">IV used to encrypt. Has to be 16 bytes (16 ASCII chars) for AES.</param>
        /// <param name="keySize">Size of key to generate, e.g. 256, 192, 128</param>
        /// <returns></returns>
        public static string AesDecrypt(this string cipherText, string password, string salt, int passwordIterations, string initialisationVector, int keySize)
        {
            // Need byte arrays for crypto classes. Assume these values will be ASCII.
            var initialisationVectorBytes = Encoding.ASCII.GetBytes(initialisationVector);
            var saltBytes = Encoding.ASCII.GetBytes(salt);

            // Convert ciphertext to byte array.
            var cipherTextBytes = Convert.FromBase64String(cipherText);

            //Use crypto libs to create a password.
            using (var passwordDerive = new Rfc2898DeriveBytes(password, saltBytes, passwordIterations))
            // instantiate crypto objects
            using (var symmetricKey = new AesManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                // key is psuedo random bytes derived from password
                var keyBytes = passwordDerive.GetBytes(keySize / 8);

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialisationVectorBytes);

                using (var memoryStream = DisposeOnce.Create(() => new MemoryStream(cipherTextBytes)))
                //unencrypted data is sensitive so we put it in a cryptostream which won't keep it in memory
                using (var cryptoStream = new CryptoStream(memoryStream.Inner, decryptor, CryptoStreamMode.Read))
                {

                    //decrypt - ciphertext will be at least as long as plaintext so initialise array that size.
                    var plainTextBytes = new byte[cipherTextBytes.Length];

                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    //Convert data to string. This might contain non-ascii characters so use UTF8 for the conversion.
                    var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    return plainText;
                }
            }
        }

        public static string JavaScriptStringEncode(this string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return message;
            }

            var builder = new StringBuilder();
            var serializer = new JavaScriptSerializer();
            serializer.Serialize(message, builder);
            return builder.ToString(1, builder.Length - 2); // remove first + last quote
        }

        public static T FromJsonString<T>(this String jsonString) where T : class
        {
            var serialiser = new JavaScriptSerializer();
            return serialiser.Deserialize<T>(jsonString);
        }

        public static string Interpolate(this string str, string token, string value, string interpolationFormat = "${0}")
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("The token cannot be null or whitespace", "token");

            if (string.IsNullOrEmpty(value))
                return str;

            token = string.Format(interpolationFormat, token);

            str = Regex.Replace(str, Regex.Escape(token), value,
               RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return str;
        }

        public static Nullable<int> ToInteger(this string str)
        {
            int result;
            if (int.TryParse(str, out result))
                return result;
            else
                return null;
        }

        public static Nullable<double> ToDouble(this string str)
        {
            double result;
            if (double.TryParse(str, out result))
                return result;
            else
                return null;
        }

        public static Nullable<double> ToDoubleInvariant(this string str)
        {
            double result;
            if (double.TryParse(str, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out result))
                return result;
            else
                return null;
        }

        public static bool AsBoolean(this string s)
        {
            if (!s.HasValue())
                return false;

            Boolean result;
            return Boolean.TryParse(s, out result) ? result : false;
        }

        /// <summary>
        /// (AT) Converts size in bytes to human-readable string (e.g. 30MB)
        /// (TB) Changed to use calculation relevant to data transfer/file storage (kB, 1000 bytes)
        ///      as opposed to KiB which is relevant to RAM measurements (1024, due to memory's binary addressing)
        /// (apw) Defaults to 1024 (which is more common) but can use 1000 by setting useBase1000 = true
        /// </summary>
        /// <param name="size"></param>
        /// <param name="useBase1000">Use 1000 as base for calculation instead of 1024</param>
        /// <returns></returns>
        public static string ByteSizeToHumanString(this long size, bool useBase1000 = false)
        {
            var sizes = new[] { "B", "kB", "MB", "GB", "TB" };
            int order = 0;
            double sizeFloat = size;
            var kbase = useBase1000 ? 1000 : 1024;
            while (sizeFloat >= kbase && order + 1 < sizes.Length)
            {
                order++;
                sizeFloat = sizeFloat / kbase;
            }

            string result = String.Format("{0:0.##} {1}", sizeFloat, sizes[order]);
            return result;
        }

        public static string TrimEnd(this string source, string value)
        {
            if (!source.EndsWith(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }

        public static string FormatJson(string str)
        {
            var indentString = "  ";
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).Iter(item => sb.Append(indentString));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).Iter(item => sb.Append(indentString));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).Iter(item => sb.Append(indentString));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string IncrementVersion(this string str, bool Major = false)
        {
            var version = str.Split('.');
            if (Major == false)
            {
                var minor = Convert.ToDecimal("." + version[1]);
                var increment = Convert.ToDecimal("0." + String.Concat(Enumerable.Repeat("0", GetDecimalPlaces(minor))) + "1");
                if ((minor + increment) % 1 == 0)
                    minor = Convert.ToDecimal("0." + String.Concat(Enumerable.Repeat("0", GetDecimalPlaces(minor) + 1)) + "1");
                else
                    minor = minor + increment;

                return version[0] + "." + minor.ToString().Split('.')[1];
            }
            else
            {
                var major = Convert.ToDecimal(version[0]);
                ++major;
                if (version.Length > 1)
                {
                    return major + "." + "00";
                }
                else
                {
                    return major.ToString();
                }
            }
        }

        private static int GetDecimalPlaces(decimal decimalNumber)
        {
            int decimalPlaces = 1;
            decimal powers = 10.0m;
            if (decimalNumber > 0.0m)
            {
                while ((decimalNumber * powers) % 1 != 0.0m)
                {
                    powers *= 10.0m;
                    ++decimalPlaces;
                }
            }
            if (decimalNumber.ToString().EndsWith("0"))
                return decimalPlaces;
            else
                return --decimalPlaces;
        }
    }
}
