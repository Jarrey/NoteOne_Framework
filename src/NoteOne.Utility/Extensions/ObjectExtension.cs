using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NoteOne_Utility.Attributes;

namespace NoteOne_Utility.Extensions
{
    /// <summary>
    ///     Support for Enum to get the description attribute
    /// </summary>
    public static class ObjectExtension
    {
        public static string GetDescription<T>(this T value)
        {
            MemberInfo methodInfo;
            if (value is MemberInfo)
                methodInfo = value as MemberInfo;
            else
                methodInfo = value.GetType().GetTypeInfo().GetMemberInfo(value.ToString(), MemberType.All);
            var attributes =
                (DescriptionAttribute[]) methodInfo.GetCustomAttributes(
                    typeof (DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T Check<T>(this T value, T defaultValue = default(T))
        {
            if (default(T) == null)
            {
                if (value == null) return defaultValue;
            }
            if (value is string)
            {
                if (string.IsNullOrEmpty(value.ToString().Trim())) return defaultValue;
            }
            return value;
        }

        public static T CheckAndThrow<T>(this T value, [CallerMemberName] string callerName = null)
        {
            if (default(T) == null)
            {
                if (value == null) throw new NullReferenceException(callerName);
            }
            if (value is string)
            {
                if (string.IsNullOrEmpty(value.ToString().Trim())) throw new NullReferenceException(callerName);
            }
            return value;
        }

        public static string ToString(this object obj, IFormatProvider format)
        {
            return Convert.ToString(obj, format);
        }
    }
}