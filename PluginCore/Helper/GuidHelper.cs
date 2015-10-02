using System;

namespace PluginCore.Helper
{
    public static class GuidHelper
    {
        /// <summary>
        /// Get guid from string, if Guid cannot be created returns empty Guid.
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns>empty guid if not valid</returns>
        /// <remarks></remarks>
        public static Guid Create(string sValue)
        {
            return IsGuid(sValue) ? new Guid(sValue) : Guid.Empty;
        }

        /// <summary>
        /// check if string is in guid format
        /// </summary>
        /// <param name="sGuid"></param>
        /// <returns></returns>
        public static bool IsGuid(string sGuid)
        {
            if (sGuid == null)
            {
                return false;
            }
            else
            {
                Guid guid;
                switch (sGuid.Length)
                {
                    case 36:
                        return Guid.TryParse(sGuid, out guid);
                    case 38:
                        return Guid.TryParse(sGuid.Substring(1, 36), out guid);
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// (IS) converts given string to option Guid
        /// </summary>
        /// <param name="value">string value to convert</param>
        /// <returns>Some(Guid) if the string represents a valid guid, otherwise None</returns>
        public static Guid? TryParse(string value)
        {
            return IsGuid(value) ? Create(value) : Guid.Empty;
        }
    }
}
