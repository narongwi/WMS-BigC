using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace Snaps.Helpers.StringExt
{

    public static class AnnotationExt { 
        public static bool AnnotationValidate<T>(this T obj, out List<ValidationResult> results) { 
            results = new List<ValidationResult>(); 
            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true); 
        }
    }
    public static class StringExt
    {
		public static Decimal CDecimal(this string ip)
		{
            Decimal rl;
			return (string.IsNullOrEmpty(ip)) ? 0 : (decimal.TryParse(ip,out rl)) ? Decimal.Parse(ip) : 0;
		}
        public static Double CDouble(this string ip) { 
            Double rl;
            return (string.IsNullOrEmpty(ip)) ? 0 : (Double.TryParse(ip,out rl)) ? Double.Parse(ip) : 0;
        }
        public static Int32 CInt32(this string ip) { 
            Int32 rl;
            return (string.IsNullOrEmpty(ip)) ? 0 : (Int32.TryParse(ip,out rl)) ? Int32.Parse(ip) : 0;
        }
        public static DateTimeOffset? CDateTimeOffset(this DateTime vl) { 
            return DateTime.SpecifyKind(vl,DateTimeKind.Local);
        }
        public static DateTime? CDateTime(this string ip) { 
            DateTime rl;
            return (string.IsNullOrEmpty(ip)) ? null :
                (DateTime.TryParse(ip, new CultureInfo("en-US"),DateTimeStyles.AdjustToUniversal,out rl)) ?
                (DateTime?)DateTime.Parse(ip, new CultureInfo("en-US"), DateTimeStyles.AdjustToUniversal) : null;
        }

        public static Boolean CBoolean(this string o) { return (o=="1") ? true : false; }
        public static Boolean CBoolean(this Int32 o){ return (o==1) ? true : false; }
        public static string ClearReg(this string o){
            return  (o !=null) ? Regex.Replace(o, @"(\s+|@|&|'|\(|\)|<|>|#)", "") : null;
            
        }
        public static string ClearEmail(this string o){
            return Regex.Replace(o, @"(\s+|&|'|\(|\)|<|>|#)", "");
        }
        public static string LPad(this string o, char paddingChar, Int16 length) {
            return o.PadLeft(length,paddingChar);
        }
        public static Boolean isNull(this string o){ 
            return String.IsNullOrEmpty(o);
        }

        public static Boolean notNull(this string o) { return !string.IsNullOrEmpty(o); }
        public static Boolean notNull(this Decimal o){ return !o.Equals(null); }
        public static Boolean notNull(this Double o){ return !o.Equals(null); }
        public static Boolean notNull(this Int32 o) { return !o.Equals(null); }
        public static Boolean notNull(this float o) { return !o.Equals(null); }
        public static Boolean notNull(this DateTime o){ return (o == DateTime.MinValue) ? false : true; }
        public static Boolean notNull(this DateTime? o) { return o.HasValue; }
        public static Boolean notNull(this DateTimeOffset o) { return (o == DateTime.MinValue)  ? false : true; }
        public static Boolean notNUll(this DateTimeOffset? o) { return o.HasValue; }
        public static Boolean notNull(this object o) { return !o.Equals(null); }

        public static String nvl(this String o) { return (o==null) ? "" : o; }
        public static Int32? nvl(this Int32? o) { return (o==null) ? 0 : o; }
        public static Decimal nvl(this Decimal o) { return (o.Equals(null)) ? 0 : o; }
        public static Double nvl(this Double o) { return (o.Equals(null)) ? 0 : o; }
        public static float nvl(this float o) { return (o.Equals(null)) ? 0 : o; }
        
        public static Double getVolume(double length, double width, double height) { 
            return length * width * height;
        }
        public static float changeUnit(this float o, String t){ 
            // Level for input must be milixx only
            if (t == "mm" || t == "mL" || t == "mg" || t == "mm3" ) { return o ; }
            else if ( t == "cm" || t == "cL" || t == "cg" || t == "cm3") { return o * 10; }
            else if ( t == "dm" || t == "dL" || t == "dg" || t == "dm3") { return o * 100; }
            else if ( t == "m" || t == "L" || t == "g" || t == "m3") { return o * 1000; }
            else if ( t == "dam" || t == "daL" || t == "dag" || t == "dam3") { return o * 10000; }
            else if ( t == "hm" || t == "hL" || t == "hg" || t == "hm3") { return o * 100000; }
            else if ( t == "km" || t == "kL" || t == "kg" || t == "km3") { return o * 1000000; }
            else { return 0; }
        }


        //validate email format 
        public static bool IsValidEmail(this String email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                try
                {
                    // Normalize the domain
                    email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                        RegexOptions.None, TimeSpan.FromMilliseconds(200));

                    // Examines the domain part of the email and normalizes it.
                    string DomainMapper(Match match)
                    {
                        // Use IdnMapping class to convert Unicode domain names.
                        var idn = new IdnMapping();

                        // Pull out and process domain name (throws ArgumentException on invalid)
                        var domainName = idn.GetAscii(match.Groups[2].Value);

                        return match.Groups[1].Value + domainName;
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }

                try
                {
                    return Regex.IsMatch(email,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
    }
}
