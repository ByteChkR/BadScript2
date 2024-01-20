using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

using Newtonsoft.Json.Linq;

namespace BadScript2.Interop.Json;

/// <summary>
///     Implements a Json to BadObject Converter
/// </summary>
public static class BadJson
{
    public static BadRuntime UseJsonApi(this BadRuntime runtime)
    {
        runtime.ConfigureContextOptions(opts => opts.AddOrReplaceApi(new BadJsonApi()));

        return runtime;
    }

    /// <summary>
    ///     Converts a JArray to a BadArray
    /// </summary>
    /// <param name="array">Array</param>
    /// <returns>Array</returns>
    private static BadArray ConvertArray(JArray array)
    {
        List<BadObject> a = new List<BadObject>();

        foreach (JToken? node in array)
        {
            if (node == null)
            {
                continue;
            }

            a.Add(ConvertNode(node));
        }

        return new BadArray(a);
    }

    /// <summary>
    ///     Converts a JObject to a BadTable
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Object</returns>
    private static BadTable ConvertObject(JObject obj)
    {
        Dictionary<BadObject, BadObject> t = new Dictionary<BadObject, BadObject>();

        foreach (KeyValuePair<string, JToken?> keyValuePair in obj)
        {
            t.Add(keyValuePair.Key, keyValuePair.Value == null ? BadObject.Null : ConvertNode(keyValuePair.Value));
        }

        return new BadTable(t);
    }

    /// <summary>
    ///     Converts a BadObject to a JToken
    /// </summary>
    /// <param name="value">Object</param>
    /// <returns>Object</returns>
    /// <exception cref="Exception">Gets Raised if the type is not supported</exception>
    public static JToken ConvertNode(BadObject value)
    {
        if (value is IBadString s)
        {
            return new JValue(s.Value);
        }

        if (value is IBadNumber n)
        {
            return new JValue(n.Value);
        }

        if (value is IBadBoolean b)
        {
            return new JValue(b.Value);
        }

        if (value == BadObject.Null)
        {
            return JValue.CreateNull();
        }

        if (value is BadArray a)
        {
            return ConvertArray(a);
        }

        if (value is BadTable t)
        {
            return ConvertTable(t);
        }


        if (value is BadReflectedObject ro)
        {
            return JToken.FromObject(ro.Instance);
        }

        throw new Exception("Unsupported value type: " + value.GetType());
    }

    /// <summary>
    ///     Converts a BadTable to a JObject
    /// </summary>
    /// <param name="table">Object</param>
    /// <returns>Object</returns>
    /// <exception cref="Exception">Get raised if any of the table keys is not a string</exception>
    private static JObject ConvertTable(BadTable table)
    {
        JObject obj = new JObject();

        foreach (KeyValuePair<BadObject, BadObject> keyValuePair in table.InnerTable)
        {
            if (keyValuePair.Key is not IBadString key)
            {
                throw new Exception("Key is not a string");
            }

            obj.Add(key.Value, ConvertNode(keyValuePair.Value));
        }

        return obj;
    }

    /// <summary>
    ///     Converts a BadArray to a JArray
    /// </summary>
    /// <param name="value">Array</param>
    /// <returns>Array</returns>
    private static JArray ConvertArray(BadArray value)
    {
        JArray array = new JArray();

        foreach (BadObject node in value.InnerArray)
        {
            array.Add(ConvertNode(node));
        }

        return array;
    }

    /// <summary>
    ///     Converts a JValue to a BadObject
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Value</returns>
    /// <exception cref="Exception">Gets Raised if the type is not supported</exception>
    private static BadObject ConvertValue(JValue value)
    {
        switch (value.Type)
        {
            case JTokenType.Integer:
                if (value.Value is int i)
                {
                    return i;
                }

                return (decimal)(long)value.Value!;
            case JTokenType.Float:
                if (value.Value is float f)
                {
                    return (decimal)f;
                }

                if (value.Value is decimal d)
                {
                    return d;
                }

                return (decimal)(double)value.Value!;
            case JTokenType.String:
                return (string)value.Value!;
            case JTokenType.Boolean:
                return (bool)value.Value!;
            case JTokenType.Guid:
                return value.Value!.ToString();
            case JTokenType.Date:
                return value.Value<DateTime>().ToString("O");
            case JTokenType.Null:
                return BadObject.Null;
            default:
                throw new Exception("Unsupported Json type: " + value.Type);
        }
    }

    /// <summary>
    ///     Converts a JToken to a BadObject
    /// </summary>
    /// <param name="node">Node</param>
    /// <returns>Object</returns>
    /// <exception cref="Exception">Gets Raised if the type is not supported</exception>
    public static BadObject ConvertNode(JToken? node)
    {
        if (node is null)
        {
            return BadObject.Null;
        }

        if (node is JArray a)
        {
            return ConvertArray(a);
        }

        if (node is JObject o)
        {
            return ConvertObject(o);
        }

        if (node is JValue v)
        {
            return ConvertValue(v);
        }

        throw new Exception("Unsupported node type: " + node.GetType());
    }

    /// <summary>
    ///     Converts a Json string to a BadObject
    /// </summary>
    /// <param name="s">String</param>
    /// <returns>Bad Object</returns>
    public static BadObject FromJson(string s)
    {
        JToken o = JToken.Parse(s);

        return ConvertNode(o);
    }

    /// <summary>
    ///     Converts a BadObject to a Json string
    /// </summary>
    /// <param name="o">Object</param>
    /// <returns>JSON String</returns>
    public static string ToJson(BadObject o)
    {
        JToken token = ConvertNode(o);

        return token.ToString();
    }
}