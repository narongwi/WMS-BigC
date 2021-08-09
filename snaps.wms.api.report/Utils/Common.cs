using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.report.Utils
{
    public static class Common
    {
        public static int toInt(this object Value)
        {
            try
            {
                if (Value == DBNull.Value)
                    return 0;
                if (Value == null)
                    return 0;

                int OutVal;
                int.TryParse(Value.ToString(), out OutVal);
                return OutVal;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static object toDecimal(this object Value)
        {
            try
            {
                if (Value == DBNull.Value)
                    return 0;
                if (Value == null)
                    return 0;
                decimal OutVal;
                decimal.TryParse(Value.ToString(), out OutVal);
                return OutVal;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static double toDouble(this object Value)
        {
            try
            {
                if (Value == DBNull.Value)
                    return 0;

                double OutVal;
                double.TryParse(Value.ToString(), out OutVal);
                return OutVal;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string SetDecimal(this double doubleNumber, int decimalPoint)
        {
            string format = string.Format("F{0}", decimalPoint);
            return doubleNumber.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string LimitString(this string Value, int limitLength)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return "";
            }
            else
            {
                if (Value.Length > limitLength)
                    return Value.Substring(0, limitLength).Replace("'", "").Replace("&", "");

                return Value.Replace("'", "").Replace("&", "");
            }
        }

        public static int GetDecimalPlace(object decimal_number)
        {
            try
            {
                string decimal_places = "";
                string input_decimal_number = decimal_number.ToString();
                var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
                if (regex.IsMatch(input_decimal_number))
                    decimal_places = regex.Match(input_decimal_number).Value;
                return toInt(decimal_places);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
