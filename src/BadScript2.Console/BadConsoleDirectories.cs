namespace BadScript2.Console
{
    public static class BadConsoleDirectories
    {
        public static string DataDirectory
        {
            get
            {
                string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

                Directory.CreateDirectory(s);

                return s;
            }
        }

        public static string LogFile => Path.Combine(DataDirectory, "logs.log");
    }
}