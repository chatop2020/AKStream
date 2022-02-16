using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// XML操作访问类
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public abstract class XmlHelper<T> where T : class
    {
        private static string m_dir = GCommon.ConfigPath;
        //private static ILog logger = AppState.logger;

        /// <summary>
        /// 文档路径
        /// </summary>
        private string m_xml_path;

        /// <summary>
        /// 存储对象
        /// </summary>
        private T t;

        public XmlHelper()
        {
        }

        /// <summary>
        /// 序列化
        /// </summary>
        private void Serialize(T t)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Encoding = Encoding.GetEncoding("utf-8"),
                Indent = true
            };
            XmlSerializer s = new XmlSerializer(t.GetType());
            var xns = new XmlSerializerNamespaces();
            xns.Add("", "");
            XmlWriter w = XmlWriter.Create(m_xml_path, settings);
            s.Serialize(w, t, xns);
            w.Flush();
            w.Close();
        }


        public virtual string Serialize<T1>(T1 obj)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var xmlSerializer = new XmlSerializer(typeof(T1));
            var stringBuilder = new StringBuilder();
          

            using (var xmlWriter = XmlWriter.Create(new StringWriter(stringBuilder), new XmlWriterSettings
                   {
                       OmitXmlDeclaration = false,
                       Encoding = Encoding.GetEncoding("utf-8"),
                       Indent = true
                   }))
            {
                xmlSerializer.Serialize(xmlWriter, obj, ns);
            }

            return stringBuilder.ToString();
        }


        /*public virtual string Serialize<T1>(T1 obj)
        {
            var stream = new MemoryStream();
            var xml = new XmlSerializer(typeof(T1));
            try
            {
                var xns = new XmlSerializerNamespaces();
                xns.Add("", "");
                //序列化对象
                xml.Serialize(stream, obj, xns);
            }
            catch
            {
                // ignored
            }

            return Encoding.UTF8.GetString(stream.ToArray()); //.Replace("\r", "");
        }*/


        /// <summary>
        /// 反序列
        /// </summary>
        /// <returns></returns>
        private T Deserialize()
        {
            if (File.Exists(m_xml_path))
            {
                TextReader r = new StreamReader(m_xml_path);
                XmlSerializer s = new XmlSerializer(typeof(T));
                object obj;
                try
                {
                    obj = (T) s.Deserialize(r);
                }
                catch (Exception)
                {
                    r.Close();
                    return null;
                }

                if (obj is T)
                    t = obj as T;
                r.Close();
            }

            return t;
        }

        public string ConvertUtf8ToDefault(string message)
        {
            Encoding utf8;
            utf8 = Encoding.GetEncoding("utf-8");
            byte[] array = Encoding.Unicode.GetBytes(message);
            byte[] s4 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), array);
            string str = Encoding.Default.GetString(s4);
            return str;
        }


        /// <summary>
        /// 反序列
        /// </summary>
        /// <returns></returns>
        private T Deserialize(string xmlBody)
        {
            MemoryStream stream = new MemoryStream(Encoding.GetEncoding("utf-8").GetBytes(xmlBody));
            StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

            //TextReader sr = new StringReader(xmlBody);
            XmlSerializer s = new XmlSerializer(typeof(T));
            object obj;
            try
            {
                obj = (T) s.Deserialize(sr);
            }
            catch
            {
                sr.Close();
                return null;
            }

            if (obj is T)
                t = obj as T;
            sr.Close();
            return t;
        }

        /// <summary>
        /// 读取文件并返回并构建成类
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>需要返回的类型格式</returns>
        public virtual T Read(Type type)
        {
            CheckConstructPath(type);
            return this.Deserialize();
        }

        /// <summary>
        /// 读取文件并返回并构建成类
        /// </summary>
        /// <param name="xmlBody">XML文档</param>
        /// <returns>需要返回的类型格式</returns>
        public virtual T Read(string xmlBody)
        {
            return this.Deserialize(xmlBody);
        }

        /// <summary>
        /// //检查并构造路径
        /// </summary>
        /// <param name="type"></param>
        private void CheckConstructPath(Type type)
        {
            //构造路径
            string temppath = m_dir + type.Name + ".xml";

            //如果路径相等则返回
            if (this.m_xml_path == temppath)
                return;

            //是否存在Config目录，不存在则返回
            if (!Directory.Exists(m_dir))
                Directory.CreateDirectory(m_dir);

            this.m_xml_path = temppath;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="t">类型</param>
        public virtual void Save(T t)
        {
            CheckConstructPath(t.GetType());
            Serialize(t);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <typeparam name="T1">类型1</typeparam>
        /// <param name="t">类型实例</param>
        /// <returns></returns>
        public virtual string Save<T1>(T1 t)
        {
            CheckConstructPath(t.GetType());
            return Serialize(t);
        }
    }
}