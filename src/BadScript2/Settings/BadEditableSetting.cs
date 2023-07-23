using BadScript2.Runtime.Error;

using Newtonsoft.Json.Linq;

namespace BadScript2.Settings;

public class BadEditableSetting<T, TValue> where T : BadSettingsProvider<T>, new()
{
	private readonly TValue? m_DefaultValue;
	private readonly string m_Name;
	private BadSettings? m_SettingsObj;

	public BadEditableSetting(string name, TValue? defaultValue = default(TValue?))
	{
		m_Name = name;
		m_DefaultValue = defaultValue;
	}

	public BadSettings? Get()
	{
		if (m_SettingsObj == null &&
		    BadSettingsProvider<T>.Instance.Settings != null)
		{
			if (BadSettingsProvider<T>.Instance.Settings.HasProperty(m_Name))
			{
				m_SettingsObj = BadSettingsProvider<T>.Instance.Settings?.GetProperty(m_Name);
			}
			else
			{
				m_SettingsObj =
					new BadSettings(m_DefaultValue == null ? JValue.CreateNull() : JToken.FromObject(m_DefaultValue));
				BadSettingsProvider<T>.Instance.Settings.SetProperty(m_Name, m_SettingsObj);
			}
		}

		return m_SettingsObj;
	}

	public TValue? GetValue()
	{
		BadSettings? setting = Get();

		if (setting == null)
		{
			return m_DefaultValue;
		}

		TValue? value = setting.GetValue<TValue>();

		return value ?? m_DefaultValue;
	}

	public void Set(TValue? value)
	{
		BadSettings? settings = Get();

		if (settings == null)
		{
			throw new BadRuntimeException($"Settings Object for {typeof(T).Name}.{m_Name} is null");
		}

		settings.SetValue(value == null ? JValue.CreateNull() : JToken.FromObject(value));
	}
}
