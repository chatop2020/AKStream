using System;

namespace LibCommon.Structs.GB28181.Sys
{
    public class EnvironmentVariables
    {
        private const string MICRO_REGISTRY_ADDRESS = "MICRO_REGISTRY_ADDRESS"; //10.78.115.182:8500
        private const string GB_SERVICE_LOCAL_ID = "GB_SERVICE_LOCAL_ID"; //42010000002100000002
        private const string GB_SERVICE_LOCAL_IP = "GB_SERVICE_LOCAL_IP"; //localhost

        private const string
            DEVICE_MANAGEMENT_SERVICE_ADDRESS = "DEVICE_MANAGEMENT_SERVICE_ADDRESS"; //devicemanagementservice:8080

        private const string
            SYSTEM_CONFIGURATION_SERVICE_ADDRESS =
                "SYSTEM_CONFIGURATION_SERVICE_ADDRESS"; //systemconfigurationservice:8080

        private const string GB_NATS_CHANNEL_ADDRESS = "GB_NATS_CHANNEL_ADDRESS"; //nats://10.78.115.182:4222
        private static string _MICRO_REGISTRY_ADDRESS;
        private static string _GB_SERVICE_LOCAL_ID;
        private static string _GB_SERVICE_LOCAL_IP;

        private static string _DEVICE_MANAGEMENT_SERVICE_ADDRESS;

        private static string _SYSTEM_CONFIGURATION_SERVICE_ADDRESS;
        private static string _GB_NATS_CHANNEL_ADDRESS;

        public static string MicroRegistryAddress
        {
            get { return _MICRO_REGISTRY_ADDRESS ?? Environment.GetEnvironmentVariable(MICRO_REGISTRY_ADDRESS); }
            set { _MICRO_REGISTRY_ADDRESS = value; }
        }

        public static string GbServiceLocalId
        {
            get { return _GB_SERVICE_LOCAL_ID ?? Environment.GetEnvironmentVariable(GB_SERVICE_LOCAL_ID); }
            set { _GB_SERVICE_LOCAL_ID = value; }
        }

        public static string GbServiceLocalIp
        {
            get { return _GB_SERVICE_LOCAL_IP ?? Environment.GetEnvironmentVariable(GB_SERVICE_LOCAL_IP); }
            set { _GB_SERVICE_LOCAL_IP = value; }
        }

        public static string DeviceManagementServiceAddress
        {
            get
            {
                return _DEVICE_MANAGEMENT_SERVICE_ADDRESS ??
                       Environment.GetEnvironmentVariable(DEVICE_MANAGEMENT_SERVICE_ADDRESS);
            }
            set { _DEVICE_MANAGEMENT_SERVICE_ADDRESS = value; }
        }

        public static string SystemConfigurationServiceAddress
        {
            get
            {
                return _SYSTEM_CONFIGURATION_SERVICE_ADDRESS ??
                       Environment.GetEnvironmentVariable(SYSTEM_CONFIGURATION_SERVICE_ADDRESS);
            }
            set { _SYSTEM_CONFIGURATION_SERVICE_ADDRESS = value; }
        }

        public static string GBNatsChannelAddress
        {
            get { return _GB_NATS_CHANNEL_ADDRESS ?? Environment.GetEnvironmentVariable(GB_NATS_CHANNEL_ADDRESS); }
            set { _GB_NATS_CHANNEL_ADDRESS = value; }
        }

        public static int GBServerGrpcPort
        {
            get { return 50051; }
        }

        public static int GbServiceLocalPort //obsolete
        {
            get { return 5061; }
        }
    }
}