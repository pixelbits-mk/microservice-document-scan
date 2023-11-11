using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Extensions
{
    public static class StringConversionExtensions
    {
        public static bool GetBooleanOrDefault(this string? value, bool defaultValue = false, string? errorMessage = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ApplicationException($"Failed to parse {value} as a Boolean");
            }


            return defaultValue;
        }

        public static int GetInt32OrDefault(this string? value, int defaultValue = 0, string? errorMessage = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ApplicationException($"Failed to parse {value} as a Int32");
            }


            return defaultValue;
        }

        public static LogLevel GetLogLevelOrDefault(this string? value,  LogLevel defaultValue = LogLevel.Information, string? errorMessage = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (Enum.TryParse(value, out LogLevel result))
            {
                return result;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ApplicationException($"Failed to parse {value} as a LogLevel");
            }


            return defaultValue;
        }
        public static string GetStringDefault(this string? str, string defaultValue = "")
        {
            return str ?? defaultValue;
        }

        public static Guid GetGuidOrDefault(this string? value, Guid defaultValue, string? errorMessage = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (Enum.TryParse(value, out Guid result))
            {
                return result;
            } 
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ApplicationException($"Failed to parse {value} as a Guid");
            }

            return defaultValue;
        }
    }
}
