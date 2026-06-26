using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// sip服务器配置
    /// </summary>
    [XmlRoot("sipaccounts")]
    public class SipServer : XmlHelper<SipServer>
    {
        private static SipServer _instance;
        private string _xml = AppDomain.CurrentDomain.BaseDirectory + "Config\\gb28181.xml";

        public static SipServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SipServer();
                }

                return _instance;
            }
        }

        [XmlElement("sipaccount")] public List<Account> Accounts { get; set; }

        public new void Save<T>(T t)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using var stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";

            using (XmlWriter writer = XmlWriter.Create(_xml, settings))
            {
                var xns = new XmlSerializerNamespaces();

                xns.Add(string.Empty, string.Empty);
                //去除默认命名空间
                xs.Serialize(writer, t, xns);
                writer.Close();
                writer.Dispose();
            }
        }

        /// <summary>
        /// 账户信息
        /// </summary>
        public class Account
        {
            [XmlElement("id")] public Guid id { get; set; }
            [XmlElement("sipusername")] public string sipusername { get; set; }
            [XmlElement("sippassword")] public string sippassword { get; set; }
            [XmlElement("sipdomain")] public string sipdomain { get; set; }
            [XmlElement("owner")] public string owner { get; set; }
            [XmlElement("localID")] public string localID { get; set; }
            [XmlElement("localSocket")] public string localSocket { get; set; }
        }
    }
}