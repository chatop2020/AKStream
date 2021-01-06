namespace LibCommon.Structs.GB28181.Sys.Auth
{
    public class ServiceAuthToken
    {
        public const string AUTH_TOKEN_KEY = "authid";
        public const string API_KEY = "apikey";
        public const string COOKIES_KEY = "Cookie";

        public static string GetAuthId()
        {
            return GetToken(AUTH_TOKEN_KEY);
        }

        public static string GetAPIKey()
        {
            return GetToken(API_KEY);
        }

        private static string GetToken(string tokenName)
        {
            string token = null;
            return token;
        }
    }
}