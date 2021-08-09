using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace snaps.wms.api.pda.Utils {
    public static class Convertor {
        public static object toJSON(this System.Data.DataTable dataTable) {
            return JValue.Parse(JsonConvert.SerializeObject(dataTable));
        }
        /// <summary>
        /// Convert String to Json format 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string toSuccessMsg(this string message) {
            return "{\"success\":true,\"message\":\"" + message + "\",\"data\":null}";
        }
        /// <summary>
        /// Convert Object List to Json
        /// </summary>
        /// <param name="json">json string object</param>
        /// <returns></returns>
        public static string toSuccess(this object json) {
            return "{\"success\":true,\"message\":\"\",\"data\":" + json + "}";
        }
            /// <summary>
        /// Convert Object List to Json
        /// </summary>
        /// <param name="json">json string object</param>
        /// <returns></returns>
        public static string toSuccessWithParams(this object json,object jparam) {
            return "{\"success\":true,\"message\":\"\",\"data\":" + json + ",\"params\":" + jparam.toJson() + "}";
        }
        /// <summary>
        /// json string object with Single Line
        /// </summary>
        /// <param name="json"></param>
        /// <param name="multiple"></param>
        /// <returns></returns>
        public static string toSuccess(this object json,bool multiple) {
            return "{\"success\":true,\"message\":\"\",\"data\":{" + json + "}}";
        }

        /// <summary>
        /// Convert String ot Error message to String 
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns>Api String</returns>
        public static string toError(this object errorMessage) {
            return "{\"success\":false,\"message\":\""+ errorMessage + "\",\"data\":{}}";
        }
        /// <summary>
        /// Convert object to json list
        /// </summary>
        /// <param name="obj">Collections</param>
        /// <returns>String</returns>
        public static string toJson(this object obj) {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        /// <summary>
        /// Convert DataTable to json list
        /// </summary>
        /// <param name="dt">DataTable </param>
        /// <param name="isarray">true = list , false = single line</param>
        /// <returns>String</returns>
        public static string toJson(this DataTable dt,bool isarray) {
            string json;
            if(isarray) {
                // array object
                json = JsonConvert.SerializeObject(dt, Formatting.None);
            } else {
                // single object
                json = new JObject(dt.Columns.Cast<DataColumn>().Select(c => new JProperty(c.ColumnName, JToken.FromObject(dt.Rows[0][c])))).ToString(Formatting.None);
            }
            return json;
        }

        /// <summary>
        /// Convert DataTable to Json String
        /// </summary>
        /// <param name="dt"> DataTable </param>
        /// <returns></returns>
        public static string toJsons(this DataTable dt) {
            return JsonConvert.SerializeObject(dt, Formatting.None);
        }
        /// <summary>
        /// Convert String to Datetime and Set format
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime toDate(this string dateTime,string format) {
            DateTime outDate;
            DateTime.TryParseExact(dateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate);
            return outDate;
        }
        // function that creates a list of an object from the given data table
        public static T toObj<T>(this DataTable dt) where T : new() {
            return getFromRow<T>(dt.Rows[0]);
        }
        // function that creates a list of an object from the given data table
        public static List<T> toList<T>(this DataTable dt) where T : new() {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach(DataRow r in dt.Rows) {
                // add to the list
                lst.Add(getFromRow<T>(r));
            }

            // return the list
            return lst;
        }
        // function that creates an object from the given data row
        public static T getFromRow<T>(DataRow row) where T : new() {
            // create a new object
            T item = new T();

            // set the item
            setFromRow(item, row);

            // return 
            return item;
        }

        public static void setFromRow<T>(T item, DataRow row) where T : new() {
            // go through each column
            foreach(DataColumn c in row.Table.Columns) {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if(p != null && row[c] != DBNull.Value) {
                    p.SetValue(item, row[c], null);
                }
            }
        }
    }
}
