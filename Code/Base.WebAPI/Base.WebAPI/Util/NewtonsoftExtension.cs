using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.WebAPI
{
    public static class NewtonsoftExtension
    {
        public static IEnumerable<T> ToListCustom<T>(this JToken param)
        {
            try
            {
                if (param == null || !param.HasValues)
                    return null;

                var jsonVal = JArray.Parse(param.ToString());
                IEnumerable<JToken> p = jsonVal.ToArray();
                IEnumerable<T> flows = null;
                if (typeof(T).Name == "String")
                    flows = p.Where(x => !string.IsNullOrEmpty(x.ToString())).Select(x => x.ToString()).Cast<T>().ToList<T>();
                else
                    flows = p.Where(x => !string.IsNullOrEmpty(x.ToString())).Select(x => JsonConvert.DeserializeObject(x.ToString(), typeof(T))).Cast<T>().ToList<T>();

                return flows;
            }
            catch (JsonReaderException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public static T DeserializeObjectCustom<T>(this JToken param)
        => (T)JsonConvert.DeserializeObject(param.ToString(), typeof(T));

    }

    public static class JsonConverter<T> where T : class
    {
        public static IEnumerable<T> ToListCustom(JToken param) => param.ToListCustom<T>();
        public static T DeserializeObjectCustom<T>(JToken para) => para.DeserializeObjectCustom<T>();
    }
}
