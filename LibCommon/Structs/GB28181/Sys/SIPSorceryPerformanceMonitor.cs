using System.Threading;

namespace LibCommon.Structs.GB28181.Sys
{
    public class SIPSorceryPerformanceMonitor
    {
        private const string PERFORMANCE_COUNTER_CATEGORY_NAME = "SIPSorcery";

        public const string PROXY_PREFIX = "Proxy";
        public const string REGISTRAR_PREFIX = "Registrar";
        public const string REGISTRATION_AGENT_PREFIX = "RegistrationAgent";

        public const string SIP_TRANSPORT_STUN_REQUESTS_PER_SECOND_SUFFIX = "STUNRequestsPerSecond";
        public const string SIP_TRANSPORT_SIP_REQUESTS_PER_SECOND_SUFFIX = "SIPRequestsPerSecond";
        public const string SIP_TRANSPORT_SIP_RESPONSES_PER_SECOND_SUFFIX = "SIPResponsesPerSecond";
        public const string SIP_TRANSPORT_SIP_BAD_MESSAGES_PER_SECOND_SUFFIX = "SIPBadMessagesPerSecond";

        public const string PROXY_STUN_REQUESTS_PER_SECOND =
            PROXY_PREFIX + SIP_TRANSPORT_STUN_REQUESTS_PER_SECOND_SUFFIX;

        public const string PROXY_SIP_REQUESTS_PER_SECOND = PROXY_PREFIX + SIP_TRANSPORT_SIP_REQUESTS_PER_SECOND_SUFFIX;

        public const string PROXY_SIP_RESPONSES_PER_SECOND =
            PROXY_PREFIX + SIP_TRANSPORT_SIP_RESPONSES_PER_SECOND_SUFFIX;

        public const string PROXY_SIP_BAD_MESSAGES_PER_SECOND =
            PROXY_PREFIX + SIP_TRANSPORT_SIP_BAD_MESSAGES_PER_SECOND_SUFFIX;

        public const string REGISTRAR_STUN_REQUESTS_PER_SECOND =
            REGISTRAR_PREFIX + SIP_TRANSPORT_STUN_REQUESTS_PER_SECOND_SUFFIX;

        public const string REGISTRAR_SIP_REQUESTS_PER_SECOND =
            REGISTRAR_PREFIX + SIP_TRANSPORT_SIP_REQUESTS_PER_SECOND_SUFFIX;

        public const string REGISTRAR_SIP_RESPONSES_PER_SECOND =
            REGISTRAR_PREFIX + SIP_TRANSPORT_SIP_RESPONSES_PER_SECOND_SUFFIX;

        public const string REGISTRAR_SIP_BAD_MESSAGES_PER_SECOND =
            REGISTRAR_PREFIX + SIP_TRANSPORT_SIP_BAD_MESSAGES_PER_SECOND_SUFFIX;

        public const string REGISTRAR_REGISTRATION_REQUESTS_PER_SECOND =
            REGISTRAR_PREFIX + "RegistersReceivedPerSecond";

        public const string REGISTRATION_AGENT_STUN_REQUESTS_PER_SECOND =
            REGISTRATION_AGENT_PREFIX + SIP_TRANSPORT_STUN_REQUESTS_PER_SECOND_SUFFIX;

        public const string REGISTRATION_AGENT_SIP_REQUESTS_PER_SECOND =
            REGISTRATION_AGENT_PREFIX + SIP_TRANSPORT_SIP_REQUESTS_PER_SECOND_SUFFIX;

        public const string REGISTRATION_AGENT_SIP_RESPONSES_PER_SECOND =
            REGISTRATION_AGENT_PREFIX + SIP_TRANSPORT_SIP_RESPONSES_PER_SECOND_SUFFIX;

        public const string REGISTRATION_AGENT_SIP_BAD_MESSAGES_PER_SECOND =
            REGISTRATION_AGENT_PREFIX + SIP_TRANSPORT_SIP_BAD_MESSAGES_PER_SECOND_SUFFIX;

        public const string REGISTRATION_AGENT_REGISTRATIONS_PER_SECOND =
            REGISTRATION_AGENT_PREFIX + "RegistrationsPerSecond";

        private static bool m_sipsorceryCategoryReady = false;

        static SIPSorceryPerformanceMonitor()
        {
            ThreadPool.QueueUserWorkItem(delegate { CheckCounters(); });
        }

        public static bool Initialise()
        {
            CheckCounters();
            return m_sipsorceryCategoryReady;
        }

        public static void IncrementCounter(string counterName)
        {
            IncrementCounter(counterName, 1);
        }

        public static void IncrementCounter(string counterName, int incrementBy)
        {
        }

        private static void CheckCounters()
        {
        }

        private static void CreateCategory()
        {
        }
    }
}