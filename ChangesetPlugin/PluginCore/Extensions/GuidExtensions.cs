using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore.Extensions
{
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether the specified Guid "value" is actually a valid guid.
        /// In invalid Guid is either null or Guid.Empty
        /// </summary>
        public static bool IsGuid(this Guid? value)
        {
            return value != null && !value.Equals(Guid.Empty);
        }

        /// <summary>
        /// get guid from string
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns>empty guid if not valid</returns>
        /// <remarks></remarks>
        public static Guid Create(string sValue)
        {
            return IsGuidString(sValue) ? new Guid(sValue) : Guid.Empty;
        }

        /// <summary>
        /// check if string is in guid format
        /// </summary>
        /// <param name="sGuid"></param>
        /// <returns></returns>
        public static bool IsGuidString(string sGuid)
        {
            //(IS) removing duplicate code
            return PluginCore.Helper.GuidHelper.IsGuid(sGuid);
        }
    }

    public static class GuidEx
    {
        /// <summary>
        /// (IS) Returns true if the Guid value is not Guid.Empty
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>true if the value isnt Guid.Empty, otherwise false</returns>
        public static bool HasValue(this Guid value)
        {
            return value != Guid.Empty;
        }
        /// <summary>
        /// (IS) Returns true if the Guid value is not null and not Guid.Empty
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>true if the value isnt Guid.Empty, otherwise false</returns>
        public static bool HasValue(this Guid? value)
        {
            return value != null && value != Guid.Empty;
        }
        /// <summary>
        /// (IS) converts the given string to Guid; returns Some(Guid) if the given string is a Guid, otherwise None
        /// </summary>
        public static Guid? From(string value)
        {
            var res = GuidExtensions.Create(value);
            return res.HasValue() ? res : Guid.Empty;
        }
       
        /// <summary>
        /// (TB) Returns true if the Guid value is Guid.Empty
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>true if the value is Guid.Empty, otherwise false</returns>
        public static bool IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }
        /// <summary>
        /// (TB) Returns true if the Guid value is Guid.Empty
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>true if the value is null or Guid.Empty, otherwise false</returns>
        public static bool IsEmpty(this Guid? value)
        {
            return value == Guid.Empty;
        }
        /// <summary>
        /// (TB) Returns true if the Guid value is null or Guid.Empty
        /// </summary>
        /// <param name="value">value to check</param>
        /// <returns>true if the value is null or Guid.Empty, otherwise false</returns>
        public static bool IsNullOrEmpty(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }

    }
}
