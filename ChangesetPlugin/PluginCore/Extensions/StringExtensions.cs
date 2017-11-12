//using PluginCore.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace PluginCore.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveLastIfLast(this string input, string characters)
        {
            if (input.LastIndexOf(characters) == (input.Length - characters.Length))
                return input.Substring(0, input.Length - characters.Length);
            return input;
        }

        public static bool HasValue(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }

        public static Nullable<int> ToInteger(this string str)
        {
            int result;
            if (int.TryParse(str, out result))
                return result;
            else
                return null;
        }
    }
}
