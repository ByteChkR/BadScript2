namespace BadScript2.Utility
{
    public static class BadEnum
    {
        public static T Parse<T>(string value, bool ignoreCase) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T Parse<T>(string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}