using System.Xml;
using SIPSorcery.Sys;

namespace LibCommon.Structs.GB28181.Sys.Auth
{
    public class SIPSorcerySecurityHeader
    {
        private const string SECURITY_NAMESPACE = "http://www.sipsorcery.com/security";
        private const string SECURITY_HEADER_NAME = "Security";
        private const string SECURITY_PREFIX = "sssec";
        private const string AUTHID_ELEMENT_NAME = "AuthID";
        private const string APIKEY_ELEMENT_NAME = "apikey";
        public string APIKey;


        public string AuthID;

        public SIPSorcerySecurityHeader(string authID, string apiKey)
        {
            AuthID = authID;
            APIKey = apiKey;
        }

        public bool MustUnderstand
        {
            get { return true; }
        }

        public string Name
        {
            get { return SECURITY_HEADER_NAME; }
        }

        public string Namespace
        {
            get { return SECURITY_NAMESPACE; }
        }

        protected void OnWriteHeaderContents(XmlDictionaryWriter writer, string messageVersion)
        {
            if (!AuthID.IsNullOrBlank())
            {
                writer.WriteStartElement(SECURITY_PREFIX, AUTHID_ELEMENT_NAME, SECURITY_NAMESPACE);
                writer.WriteString(AuthID);
                writer.WriteEndElement();
            }

            if (!APIKey.IsNullOrBlank())
            {
                writer.WriteStartElement(SECURITY_PREFIX, APIKEY_ELEMENT_NAME, SECURITY_NAMESPACE);
                writer.WriteString(AuthID);
                writer.WriteEndElement();
            }
        }

        protected void OnWriteStartHeader(XmlDictionaryWriter writer, string messageVersion)
        {
            writer.WriteStartElement(SECURITY_PREFIX, this.Name, this.Namespace);
        }

        public static SIPSorcerySecurityHeader ParseHeader( /*OperationContext context*/)
        {
            return null;
        }
    }
}