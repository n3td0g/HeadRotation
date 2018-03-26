using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadRotation.Helpers
{
    /// <summary> Helper class for working with string </summary>
    public static class StringConverter
    {
        public static float ToFloat(string str)
        {
            return (float)ToDouble(str, float.NaN);
        }
        public static float ToFloat(string str, float nanValue)
        {
            return (float)ToDouble(str, nanValue);
        }

        public static double ToDouble(object value)
        {
            return value == null ? double.NaN : ToDouble(value.ToString());
        }
        public static double ToDouble(string str)
        {
            return ToDouble(str, Double.NaN);
        }
        public static double ToDouble(string str, double nanValue)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0) return nanValue;
            var newSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var oldSeparator = newSeparator == "," ? "." : ",";
            var s = str.Replace(oldSeparator, newSeparator).Trim(new[] { ' ', '\t' });

            double result;
            if (double.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out result))
                return result;

            return nanValue;
        }
    }
}
