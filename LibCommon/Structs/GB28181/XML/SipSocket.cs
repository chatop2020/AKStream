using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LibCommon.Structs.GB28181.XML
{
    [XmlRoot("sipServer")]
    public class SipSocket : XmlHelper<SipSocket>
    {
        private static SipSocket _instance;

        public static SipSocket Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SipSocket();
                }

                return _instance;
            }
        }

        [XmlElement("MonitorLoopbackPort")] public MonitorLoopbackPort MonitorPort { get; set; }

        public override string Save<T>(T t)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            //settings.Encoding = Encoding.GetEncoding("GB2312");
            //settings.Encoding = new UTF8Encoding(false);
            //settings.NewLineOnAttributes = true;
            //settings.OmitXmlDeclaration = false;
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                var xns = new XmlSerializerNamespaces();

                xns.Add(string.Empty, string.Empty);
                //去除默认命名空间
                xs.Serialize(writer, t, xns);
            }

            return Encoding.UTF8.GetString(stream.ToArray()).Replace("\r", "");
        }

        public class MonitorLoopbackPort
        {
            [XmlAttribute("value")] public int value { get; set; }
        }
    }
}