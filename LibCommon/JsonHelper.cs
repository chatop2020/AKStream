using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace LibCommon
{
    public class BoolConvert : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(bool) || objectType == typeof(Boolean));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            bool b = (bool)value!;
            if (b)
            {
                writer.WriteValue("1");
            }
            else
            {
                writer.WriteValue("0");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Value<string>().Trim().Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class IpAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            IPAddress ip = (IPAddress)value!;
            writer.WriteValue(ip.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            return IPAddress.Parse(token.Value<string>());
        }
    }

    class IpEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPEndPoint));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            IPEndPoint ep = (IPEndPoint)value!;
            writer.WriteStartObject();
            writer.WritePropertyName("Address");
            serializer.Serialize(writer, ep.Address);
            writer.WritePropertyName("Port");
            writer.WriteValue(ep.Port);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            IPAddress address = jo["Address"]!.ToObject<IPAddress>(serializer)!;
            int port = jo["Port"]!.Value<int>();
            return new IPEndPoint(address, port);
        }
    }


    /// <summary>
    /// json工具类
    /// </summary>
    public static class JsonHelper
    {
        private static string _loggerHead = "JsonHelper";
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
            _jsonSettings.Converters.Add(new IpAddressConverter());
            _jsonSettings.Converters.Add(new IpEndPointConverter());
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
                GCommon.Logger.Error($"[{_loggerHead}]->Json序列化异常->{ex.Message}\r\n{ex.StackTrace}");
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
        /// MissingMemberHandling.Ignore实体类缺少字段时忽略它
        public static T FromJson<T>(this string json, MissingMemberHandling p = MissingMemberHandling.Ignore)
        {
            _jsonSettings.MissingMemberHandling = p;

            try
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings)!;
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error($"[{_loggerHead}]->Json返序列化异常->{ex.Message}\r\n{ex.StackTrace}\r\njson内容：{json}");

                return default(T)!;
            }
        }
    }
}