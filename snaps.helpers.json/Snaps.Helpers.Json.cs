using System;
using Newtonsoft.Json;

namespace Snaps.Helpers.Json
{
    public static class JsonExt
    {
        //decode from object
        public static string toJson(this object value)
        {            
            return JsonConvert.SerializeObject(value, Formatting.None, 
            new JsonSerializerSettings {  ReferenceLoopHandling = ReferenceLoopHandling.Ignore  });
        }
        //decode from exception 
        public static String toJson(this Exception ex){
            return JsonConvert.SerializeObject(ex, Formatting.None, 
            new JsonSerializerSettings {  ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        }
        
        public static object toObject(this string value) { 
            return JsonConvert.DeserializeObject(value);
        }
    }
}
