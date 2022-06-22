using BadScript2.Common.Logging;

using Newtonsoft.Json.Linq;

namespace BadScript2.Settings
{
    public class BadSettingsReader
    {
        private readonly BadSettings m_RootSettings;

        private readonly string[] m_SourceFiles;

        public BadSettingsReader(BadSettings rootSettings, params string[] sourceFiles)
        {
            m_SourceFiles = sourceFiles;
            m_RootSettings = rootSettings;
        }

        public BadSettings ReadSettings()
        {
            List<BadSettings> settings = new List<BadSettings> { m_RootSettings };

            Queue<string> files = new Queue<string>(m_SourceFiles);
            while (files.Count != 0)
            {
                string file = files.Dequeue();
                BadLogger.Log("Reading settings from file: " + file, "SettingsReader");
                settings.Add(CreateSettings(ReadJsonFile(file)));
            }

            BadSettings s = new BadSettings();
            s.Populate(settings.ToArray());

            BadLogger.Log("Resolving environment variables", "SettingsReader");
            BadSettings.ResolveEnvironmentVariables(s);

            ReadDynamicSettings(s);

            return s;
        }

        private JToken ReadJsonFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            JToken token = JToken.Parse(json);

            return token;
        }

        private BadSettings CreateSettings(JToken? token)
        {
            if (token is { Type: JTokenType.Object })
            {
                Dictionary<string, BadSettings> settings = new Dictionary<string, BadSettings>();
                JObject obj = (JObject)token;
                foreach (KeyValuePair<string, JToken?> keyValuePair in obj)
                {
                    settings.Add(keyValuePair.Key, CreateSettings(keyValuePair.Value));
                }

                return new BadSettings(settings);
            }

            return new BadSettings(token);
        }


        private void ReadDynamicSettings(BadSettings settings)
        {
            BadLogger.Log("Processing Dynamic Includes", "SettingsReader");
            BadSettings? elems = settings.FindProperty("SettingsBuilder.Include");
            string[]? includes = elems?.GetValue<string[]>();

            if (includes == null)
            {
                return;
            }

            elems?.SetValue(null);
            do
            {
                List<BadSettings> setting = new List<BadSettings>();
                foreach (string include in includes)
                {
                    if (include.Contains('*'))
                    {
                        bool allDirs = include.Contains("**");
                        string[] parts = include.Split('*', StringSplitOptions.RemoveEmptyEntries);
                        string path = parts[0];
                        string pattern = parts[1];
                        string[] files = Directory.GetFiles(
                            path,
                            $"*{pattern}",
                            allDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                        );
                        setting.AddRange(
                            files.Select(
                                f =>
                                {
                                    BadLogger.Log("Reading settings from file: " + f, "SettingsReader");

                                    return CreateSettings(ReadJsonFile(f));
                                }
                            )
                        );
                    }
                    else
                    {
                        BadLogger.Log("Reading settings from file: " + include, "SettingsReader");
                        setting.Add(CreateSettings(ReadJsonFile(include)));
                    }
                }

                settings.Populate(setting.ToArray());


                BadLogger.Log("Resolving environment variables", "SettingsReader");
                BadSettings.ResolveEnvironmentVariables(settings);

                elems = settings.FindProperty("SettingsBuilder.Include");
                includes = elems?.GetValue<string[]>();
            }
            while (includes != null);

            settings.RemoveProperty("SettingsBuilder");
        }
    }
}