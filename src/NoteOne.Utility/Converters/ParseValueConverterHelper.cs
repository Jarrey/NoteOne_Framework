using System;
using System.Collections.Generic;
using System.Globalization;

#if WEB_SERVICE
using System.Drawing;

namespace Chame.WebService.Helper
{
#else
using Windows.Foundation;
using Windows.UI;

namespace NoteOne_Utility.Converters
{
#endif
    public static class ParseValueConverterHelper
    {
        public static byte StringToByte(this String value)
        {
            byte result = 0;
            byte.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static int StringToInt(this String value)
        {
            int result = 0;
            int.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static uint StringToUInt(this String value)
        {
            uint result = 0;
            uint.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static long StringToLong(this String value)
        {
            long result = 0L;
            long.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static ulong StringToULong(this String value)
        {
            ulong result = 0L;
            ulong.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static double StringToDouble(this String value)
        {
            double result = 0.0;
            double.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static float StringToFloat(this String value)
        {
            float result = 0.0f;
            float.TryParse(value, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static bool StringToBoolean(this string value)
        {
            return (value.ToLower().Trim() == "true" || value.Trim() == "1") ? true : false;
        }

#if WEB_SERVICE
        public static PointF StringToPoint(this string value)
        {
            string[] point = value.Split(new[] { ',', ' ' });
            if (point.Length == 2)
                return new PointF(StringToFloat(point[0]), StringToFloat(point[1]));
            else
                return default(PointF);
        }
#else
        public static Point StringToPoint(this string value)
        {
            string[] point = value.Split(new[] { ',', ' ' });
            if (point.Length == 2)
                return new Point(StringToDouble(point[0]), StringToDouble(point[1]));
            else
                return default(Point);
        }
#endif

        public static Color StringToColor(this string value)
        {
            uint argb = uint.Parse(value.Replace("#", ""), NumberStyles.HexNumber);
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                                  (byte)((argb & 0xff0000) >> 0x10),
                                  (byte)((argb & 0xff00) >> 8),
                                  (byte)(argb & 0xff));
        }

        public static string[] StringToArray(this string value, char split = ' ')
        {
            string[] result;
            if (split == ' ')
                result = value.Split(new[] { ',', ' ', '|' });
            else
                result = value.Split(split);
            if (result != null && result.Length != 0)
                return result;
            else
                return null;
        }

        public static DateTime StringToDateTime(this string value, string format = "yyyyMMdd")
        {
            var result = new DateTime(1970, 1, 1, 0, 0, 0);
            string[] dateTimeFormats;
            if (format == "yyyyMMdd")
                dateTimeFormats = new[] { format };
            else
                dateTimeFormats = new[]
                    {
                        format,
                        "yyyyMMdd",
                        "yyyy-MM-dd",
                        "yyyy/MM/dd",
                        "yyyyMMdd HH:mm:ss.fff",
                        "yyyy-MM-dd HH:mm:ss.fff",
                        "yyyy/MM/dd HH:mm:ss.fff"
                    };
            DateTime.TryParseExact(value, dateTimeFormats, new CultureInfo("en-US"), DateTimeStyles.None, out result);
            return result;
        }

        public static Dictionary<string, string> StringToDictionary(this string value)
        {
            var returnvalue = new Dictionary<string, string>();
            string[] parameters = value.Split('|');
            foreach (string p in parameters)
            {
                string[] values = p.Split(':');
                if (values.Length > 1)
                {
                    returnvalue[values[0]] = values[1];
                }
            }
            return returnvalue;
        }

        public static DateTime JSStringToDateTime(this string value)
        {
            var result = new DateTime(1970, 1, 1, 0, 0, 0);
            long ticks = StringToLong(value);
            if (StringToLong(value) == 0L)
                DateTime.TryParse(value, out result);
            else
                result = result.AddTicks(ticks * 10000000);
            return result;
        }
    }
}