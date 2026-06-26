using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LibSystemInfo
{
    /// <summary>
    /// json工具类
    /// </summary>
    public static class JsonHelper
    {
        private static JsonSerializerSettings _jsonSettings;

        static JsonHelper()
        {
            IsoDateTimeConverter datetimeConverter = new IsoDateTimeConverter();
            datetimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.MissingMemberHandling = MissingMemberHandling.Error;
            _jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            _jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            _jsonSettings.Converters.Add(datetimeConverter);
        }


        //格式化json字符串
        public static string ConvertJsonString(string str)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object? obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


        /// <summary>
        /// 将指定的对象序列化成 JSON 数据。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="formatting"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Formatting formatting = Formatting.None,
            MissingMemberHandling p = MissingMemberHandling.Error)
        {
            _jsonSettings.MissingMemberHandling = p;
            try
            {
                return JsonConvert.SerializeObject(obj, formatting, _jsonSettings);
            }
            catch (Exception ex)
            {
                return null!;
            }
        }


        /// <summary>
        /// 将指定的对象序列化成 JSON 数据。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, MissingMemberHandling p = MissingMemberHandling.Error)
        {
            _jsonSettings.MissingMemberHandling = p;
            try
            {
                if (null == obj)
                    return null!;

                return JsonConvert.SerializeObject(obj, Formatting.None, _jsonSettings);
            }
            catch
            {
                return null!;
            }
        }

        /// <summary>
        /// 将指定的 JSON 数据反序列化成指定对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON 数据。</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json, MissingMemberHandling p = MissingMemberHandling.Error)
        {
            _jsonSettings.MissingMemberHandling = p;
            try
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings)!;
            }
            catch
            {
                return default(T)!;
            }
        }
    }
}