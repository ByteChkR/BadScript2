using BadScript2.Runtime.Error;

namespace BadScript2.Settings
{
    public static class BadSettingsProvider
    {
        private static BadSettings? s_RootSettings;

        public static bool HasRootSettings => s_RootSettings != null;

        public static BadSettings RootSettings => s_RootSettings ??
                                                  throw new BadRuntimeException(
                                                      "BadSettingsProvider.RootSettings is not initialized"
                                                  );

        public static void SetRootSettings(BadSettings settings)
        {
            s_RootSettings = settings;
        }
    }

    public abstract class BadSettingsProvider<T> where T : BadSettingsProvider<T>, new()
    {
        private static T? s_Instance;
        private readonly string m_Path;

        protected BadSettingsProvider(string path)
        {
            m_Path = path;
        }

        protected BadSettings? Settings => BadSettingsProvider.HasRootSettings
            ? BadSettingsProvider.RootSettings.FindProperty(m_Path)
            : null;

        public static T Instance => s_Instance ??= new T();
    }
}