using BadScript2.Common.Logging;
using BadScript2.IO;

using Newtonsoft.Json.Linq;

namespace BadScript2.Settings;

/// <summary>
///     Reads a JSON file and returns the resulting BadSettings Object
/// </summary>
public class BadSettingsReader
{
	/// <summary>
	///     The Root Settings Object that all other Settings are added into
	/// </summary>
	private readonly BadSettings m_RootSettings;

	/// <summary>
	///     The Source Files that are used to read the Settings
	/// </summary>
	private readonly string[] m_SourceFiles;

	/// <summary>
	///     Constructs a new BadSettingsReader
	/// </summary>
	/// <param name="rootSettings">Root Settings Object</param>
	/// <param name="sourceFiles">List of Source Files</param>
	public BadSettingsReader(BadSettings rootSettings, params string[] sourceFiles)
	{
		m_SourceFiles = sourceFiles;
		m_RootSettings = rootSettings;
	}

	/// <summary>
	///     Returns a new Instance of BadSettings with all source files loaded
	/// </summary>
	/// <returns>BadSettings Instance</returns>
	public BadSettings ReadSettings()
	{
		List<BadSettings> settings = new List<BadSettings>
		{
			m_RootSettings
		};

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

	/// <summary>
	///     Parses a JSON File and returns the resulting JObject
	/// </summary>
	/// <param name="fileName">Source File</param>
	/// <returns></returns>
	private JToken ReadJsonFile(string fileName)
	{
		string json = BadFileSystem.ReadAllText(fileName);
		JToken token = JToken.Parse(json);

		return token;
	}

	/// <summary>
	///     Creates a Settings Object from a JToken
	/// </summary>
	/// <param name="token">JToken to convert</param>
	/// <returns>BadSettings Instance</returns>
	private BadSettings CreateSettings(JToken? token)
	{
		if (token is
		    {
			    Type: JTokenType.Object
		    })
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


	/// <summary>
	///     Processes Dynamic Include Statements inside a settings object.
	/// </summary>
	/// <param name="settings">Settings object to process</param>
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
					string[] parts = include.Split(new[]
						{
							'*'
						},
						StringSplitOptions.RemoveEmptyEntries);
					string path = parts[0];
					string extension = parts[1];
					IEnumerable<string> files = BadFileSystem.Instance.GetFiles(path,
						extension,
						allDirs);
					setting.AddRange(files.Select(f =>
					{
						BadLogger.Log("Reading settings from file: " + f, "SettingsReader");

						return CreateSettings(ReadJsonFile(f));
					}));
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
