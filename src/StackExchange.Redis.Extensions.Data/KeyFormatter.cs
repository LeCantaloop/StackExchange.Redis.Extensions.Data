namespace StackExchange.Redis.Extensions.Data
{
    public static class KeyFormatter
    {
        private static string KeyFormatSuffix
        {
            get { return ":{0}"; }
        }

        public static string Format(string key, string property)
        {
            return string.Format(key + KeyFormatSuffix, property);
        }
    }
}