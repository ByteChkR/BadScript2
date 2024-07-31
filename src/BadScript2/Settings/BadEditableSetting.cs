using BadScript2.Runtime.Error;

using Newtonsoft.Json.Linq;

/// <summary>
/// Contains the Settings Implementation
/// </summary>
namespace BadScript2.Settings;

/// <summary>
///     Implements an Editable Setting
/// </summary>
/// <typeparam name="T">The Type that has the setting</typeparam>
/// <typeparam name="TValue">The Type of the value of the setting</typeparam>
public class BadEditableSetting<T, TValue> where T : BadSettingsProvider<T>, new()
{
	/// <summary>
	///     The Default Value of the Setting
	/// </summary>
	private readonly TValue? m_DefaultValue;

	/// <summary>
	///     The Name of the Setting
	/// </summary>
	private readonly string m_Name;

	/// <summary>
	///     The Settings Object
	/// </summary>
	private BadSettings? m_SettingsObj;

	/// <summary>
	///     Creates a new Editable Setting
	/// </summary>
	/// <param name="name">The Name of the Setting</param>
	/// <param name="defaultValue">The Default Value of the Setting</param>
	public BadEditableSetting(string name, TValue? defaultValue = default)
    {
        m_Name = name;
        m_DefaultValue = defaultValue;
    }

	/// <summary>
	///     Returns the settings object of the Editable Setting
	/// </summary>
	/// <returns>The settings object of the Editable Setting</returns>
	public BadSettings? Get()
    {
        if (m_SettingsObj != null ||
            BadSettingsProvider<T>.Instance.Settings == null)
        {
            return m_SettingsObj;
        }

        if (BadSettingsProvider<T>.Instance.Settings.HasProperty(m_Name))
        {
            m_SettingsObj = BadSettingsProvider<T>.Instance.Settings?.GetProperty(m_Name);
        }
        else
        {
            m_SettingsObj =
                new BadSettings(m_DefaultValue == null ? JValue.CreateNull() : JToken.FromObject(m_DefaultValue),
                                string.Empty
                               );
            BadSettingsProvider<T>.Instance.Settings.SetProperty(m_Name, m_SettingsObj);
        }

        return m_SettingsObj;
    }

	/// <summary>
	///     Returns the value of the Editable Setting
	/// </summary>
	/// <returns>The value of the Editable Setting</returns>
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

	/// <summary>
	///     Sets the value of the Editable Setting
	/// </summary>
	/// <param name="value">The value to set</param>
	/// <exception cref="BadRuntimeException">If the Settings Object is null</exception>
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