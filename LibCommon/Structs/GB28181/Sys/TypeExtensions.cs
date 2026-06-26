namespace LibCommon.Structs.GB28181.Sys
{
    public static class TypeExtensions
    {
        public static readonly char[] codes =
        {
            ',', '\'', ';', ':', '/', '?', '<', '>', '.', '#', '%', '&', '?',
            '^', '\\', '@', '*', '~', '`', '$', '{', '}', '[', ']', '"'
        };

        /// <summary>  
        /// 非法字符转换  
        /// </summary>  
        /// <param name="str">携带(特殊字符)字符串</param>  
        /// <returns></returns>  
        public static string Replace(this string str)
        {
            for (int i = 0; i < codes.Length; i++)
            {
                str = str.Replace(codes[i], char.MinValue);
            }

            return str;
        }
    }
}