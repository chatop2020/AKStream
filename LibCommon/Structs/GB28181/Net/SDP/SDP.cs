using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace LibCommon.Structs.GB28181.Net.SDP
{
    public class SDP
    {
        public const string CRLF = "\r\n";
        public const string SDP_MIME_CONTENTTYPE = "application/sdp";
        public const decimal SDP_PROTOCOL_VERSION = 0M;
        public const string ICE_UFRAG_ATTRIBUTE_PREFIX = "ice-ufrag";
        public const string ICE_PWD_ATTRIBUTE_PREFIX = "ice-pwd";
        public const string ICE_CANDIDATE_ATTRIBUTE_PREFIX = "candidate";

        public string
            Address; // IP Address of the machine that created the session, either FQDN or dotted quad or textual for IPv6.

        public string AddressType = "IP4"; // Address type, typically IP4 or IP6.

        public int
            AnnouncementVersion =
                0; // Version number for each announcement, number must be increased for each subsequent SDP modification.

        public List<string> BandwidthAttributes = new List<string>();

        public SDPConnectionInformation Connection;

        public List<string> ExtraAttributes = new List<string>(); // Attributes that were not recognised.
        public List<string> IceCandidates;
        public string IcePwd; // If ICE is being used the password for the STUN requests.
        public string IceUfrag; // If ICE is being used the username for the STUN requests.

        // Media.
        public List<SDPMediaAnnouncement> Media = new List<SDPMediaAnnouncement>();

        public string NetworkType = "IN"; // Type of network, IN = Internet.
        public string[] OriginatorEmailAddresses; // Email addresses for the person responsible for the session.
        public string[] OriginatorPhoneNumbers; // Phone numbers for the person responsible for the session.

        // Optional fields.
        public string SessionDescription;
        public string SessionId = "-"; // Unique Id for the session.

        public string SessionName = "-"; // Common name of the session.
        public string Timing;
        public string URI; // URI for additional information about the session.

        // Owner fields.
        public string Username = "-"; // Username of the session originator.

        public decimal Version = SDP_PROTOCOL_VERSION;

        public SDP()
        {
        }

        public SDP(string address)
        {
            Address = address;
        }

        public string Owner
        {
            get
            {
                return Username + " " + SessionId + " " + AnnouncementVersion + " " + NetworkType + " " + AddressType +
                       " " + Address;
            }
        }

        public void AddExtra(string attribute)
        {
            if (!string.IsNullOrWhiteSpace(attribute))
                ExtraAttributes.Add(attribute);
        }

        public override string ToString()
        {
            string sdp =
                "v=" + SDP_PROTOCOL_VERSION + CRLF +
                "o=" + Owner + CRLF +
                "s=" + SessionName + CRLF;
            sdp += string.IsNullOrWhiteSpace(URI) ? null : "u=" + URI + CRLF;
            sdp += ((Connection != null) ? Connection.ToString() : null);
            foreach (string bandwidth in BandwidthAttributes)
            {
                sdp += "b=" + bandwidth + CRLF;
            }

            sdp += "t=" + Timing + CRLF;

            sdp += !string.IsNullOrWhiteSpace(IceUfrag)
                ? "a=" + ICE_UFRAG_ATTRIBUTE_PREFIX + ":" + IceUfrag + CRLF
                : null;
            sdp += !string.IsNullOrWhiteSpace(IcePwd) ? "a=" + ICE_PWD_ATTRIBUTE_PREFIX + ":" + IcePwd + CRLF : null;
            sdp += string.IsNullOrWhiteSpace(SessionDescription) ? null : "i=" + SessionDescription + CRLF;

            if (OriginatorEmailAddresses != null && OriginatorEmailAddresses.Length > 0)
            {
                foreach (string originatorAddress in OriginatorEmailAddresses)
                {
                    sdp += string.IsNullOrWhiteSpace(originatorAddress) ? null : "e=" + originatorAddress + CRLF;
                }
            }

            if (OriginatorPhoneNumbers != null && OriginatorPhoneNumbers.Length > 0)
            {
                foreach (string originatorNumber in OriginatorPhoneNumbers)
                {
                    sdp += string.IsNullOrWhiteSpace(originatorNumber) ? null : "p=" + originatorNumber + CRLF;
                }
            }

            foreach (string extra in ExtraAttributes)
            {
                sdp += string.IsNullOrWhiteSpace(extra) ? null : extra + CRLF;
            }

            foreach (SDPMediaAnnouncement media in Media)
            {
                sdp += (media == null) ? null : media.ToString();
            }

            return sdp;
        }

        public static IPEndPoint GetSDPRTPEndPoint(string sdpMessage)
        {
            // Process the SDP payload.
            Match portMatch = Regex.Match(sdpMessage, @"m=audio (?<port>\d+)", RegexOptions.Singleline);
            if (portMatch.Success)
            {
                int rtpServerPort = Convert.ToInt32(portMatch.Result("${port}"));

                Match serverMatch = Regex.Match(sdpMessage, @"c=IN IP4 (?<ipaddress>(\d+\.){3}\d+)",
                    RegexOptions.Singleline);
                if (serverMatch.Success)
                {
                    string rtpServerAddress = serverMatch.Result("${ipaddress}");
                    IPAddress ipAddress = null;

                    if (IPAddress.TryParse(rtpServerAddress, out ipAddress))
                    {
                        IPEndPoint serverEndPoint = new IPEndPoint(ipAddress, rtpServerPort);
                        return serverEndPoint;
                    }
                }
            }

            return null;
        }
    }
}