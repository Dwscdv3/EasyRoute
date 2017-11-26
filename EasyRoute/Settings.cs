namespace EasyRoute
{
    public static class Settings
    {
        public static object Root { get; set; }
        public static bool RequireAttribute { get; set; }
        public static bool CaseSensitive { get; set; }

        static Settings()
        {
            RequireAttribute = true;
            CaseSensitive = true;
        }
    }
}
