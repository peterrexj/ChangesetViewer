using System;

namespace PluginCore.Fundamentals
{
    public static class Check
    {
        /// <summary>
        ///  asserts that the value isnt empty (null or "")
        /// </summary>
        /// <param name="value">value to assert that it isnt empty</param>
        /// <param name="parameterName">name of the parameter this value is from</param>
        /// <returns>original value - or exception if the value is empty</returns>
        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(parameterName);

            return value;
        }
        /// <summary>
        ///  asserts that the value isnt null
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="value">value to assert that it isnt null</param>
        /// <param name="parameterName">name of the parameter this value is from</param>
        /// <returns>original value - or exception if the value is null</returns>
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }
        /// <summary>
        ///  asserts that the value isnt null
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="value">value to assert that it isnt null</param>
        /// <param name="parameterName">name of the parameter this value is from</param>
        /// <returns>original value - or exception if the value is null</returns>
        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(parameterName);

            return value;
        }
        public static void IsTrue(bool value, Func<string> message)
        {
            if (value)
                return;

            throw new ArgumentException(message());
        }
        public static T Failed<T>(string message)
        {
            throw new ArgumentException(message);
        }
    }
}
