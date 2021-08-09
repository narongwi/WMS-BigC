using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
namespace Snaps.Helpers.Hash
{
    public static class HashExt
    {
        public static string ToHash(this string rawData) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData)); // ComputeHash - returns byte array
                StringBuilder builder = new StringBuilder(); // Convert byte array to a string
                for (int i = 0; i < bytes.Length; i++) { builder.Append(bytes[i].ToString("x2")); }
                return builder.ToString();
            }
        }
        public static string ToHashwithSalt(this string raw, string salt){ 
            Int32 cntlength = 0;
            cntlength = (int)Math.Ceiling(Convert.ToDecimal(salt.Length/2));
            raw = salt.Substring(cntlength) + raw + salt.Substring(salt.Length-cntlength, cntlength) ;
            return raw.ToHash();
        }

        public static string TodecAuth(this string authCode,string authDigit, string authDate,string authprov = "") { 
            if (authDigit.IndexOf("-") > 0){ 
                if (authprov == "") { authprov = Environment.MachineName; }
                return (authCode+"#Spans#"+authDate+authDigit.Substring(0,authDigit.IndexOf('-'))).ToString().ToHashwithSalt(authprov);
            }else { return ""; }
        }
        public static Boolean TovldAuth(this string authCode,string authDigit, string authDate){
            if (authDigit.IndexOf("-") > 0) {
                string encd = authCode.TodecAuth(authDigit,authDate);
                try { 
                    // string co0 = encd.Substring(4,2);
                    // string co1 = encd.Substring(9,2);
                    // string co2 = encd.Substring(encd.Length-8, 4);
                    // string rsl = authDigit.Substring(0,authDigit.IndexOf('-')).ToString() +"-"+ encd.Substring(4,2)+encd.Substring(9,2)+encd.Substring(encd.Length-8, 4);
                    return (authDigit.Substring(0,authDigit.IndexOf('-')).ToString() +"-"+ encd.Substring(4,2)+encd.Substring(9,2)+encd.Substring(encd.Length-8, 4) == authDigit) ? true : false; 
                }catch (Exception) { return false; }                
            }else { 
                return false;
            }
        }
    }

}
