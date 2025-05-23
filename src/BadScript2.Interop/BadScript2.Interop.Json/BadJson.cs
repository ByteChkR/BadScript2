﻿using System.Dynamic;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

///<summary>
///	Contains JSON Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.Json;

/// <summary>
///     Implements a Json to BadObject Converter
/// </summary>
public static class BadJson
{
    /// <summary>
    ///     Configures the Runtime to use the Json API
    /// </summary>
    /// <param name="runtime">The Runtime</param>
    /// <returns>The Runtime</returns>
    public static BadRuntime UseJsonApi(this BadRuntime runtime)
    {
        return runtime.UseApi(new BadJsonApi(), true);
    }

    /// <summary>
    ///     Converts a JArray to a BadArray
    /// </summary>
    /// <param name="array">Array</param>
    /// <returns>Array</returns>
    private static BadArray ConvertArray(JArray array)
    {
        List<BadObject> a = array.Where(x => x != null)
                                 .Select(ConvertNode)
                                 .ToList();

        return new BadArray(a);
    }

    /// <summary>
    ///     Converts a JObject to a BadTable
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Object</returns>
    private static BadTable ConvertObject(JObject obj)
    {
        Dictionary<string, BadObject> t = new Dictionary<string, BadObject>();

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
        if (value == BadObject.Null)
        {
            return JValue.CreateNull();
        }

        return value switch
        {
            IBadString s          => new JValue(s.Value),
            IBadNumber n          => new JValue(n.Value),
            IBadBoolean b         => new JValue(b.Value),
            BadArray a            => ConvertArray(a),
            BadTable t            => ConvertTable(t),
            BadReflectedObject ro => JToken.FromObject(ro.Instance),
            BadFunction f         => new JValue(f.ToSafeString()),
            _                     => throw new Exception("Unsupported value type: " + value.GetType()),
        };
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

        foreach (KeyValuePair<string, BadObject> keyValuePair in table.InnerTable)
        {
            obj.Add(keyValuePair.Key, ConvertNode(keyValuePair.Value));
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
                if(value.Value is byte b)
                {
                    return b;
                }
                if(value.Value is sbyte sb)
                {
                    return sb;
                }
                if(value.Value is short s)
                {
                    return s;
                }
                if(value.Value is ushort us)
                {
                    return us;
                }
                if(value.Value is uint ui)
                {
                    return ui;
                }
                if(value.Value is long l)
                {
                    return l;
                }
                if(value.Value is ulong ul)
                {
                    return ul;
                }

                return (long)value.Value!;
            case JTokenType.Float:
                return value.Value switch
                {
                    float f   => (decimal)f,
                    decimal d => d,
                    _         => (decimal)(double)value.Value!,
                };

            case JTokenType.String:
                return (string)value.Value!;
            case JTokenType.Boolean:
                return (bool)value.Value!;
            case JTokenType.Guid:
                return value.Value!.ToString();
            case JTokenType.Date:
                return value.Value<DateTime>()
                            .ToString("O");
            case JTokenType.Null:
                return BadObject.Null;
            case JTokenType.None:
            case JTokenType.Object:
            case JTokenType.Array:
            case JTokenType.Constructor:
            case JTokenType.Property:
            case JTokenType.Comment:
            case JTokenType.Undefined:
            case JTokenType.Raw:
            case JTokenType.Bytes:
            case JTokenType.Uri:
            case JTokenType.TimeSpan:
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
        return node switch
        {
            null      => BadObject.Null,
            JArray a  => ConvertArray(a),
            JObject o => ConvertObject(o),
            JValue v  => ConvertValue(v),
            _         => throw new Exception("Unsupported node type: " + node.GetType()),
        };
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
    /// Parses a YAML string to a BadObject
    /// </summary>
    /// <param name="s">YAML String</param>
    /// <returns>BadObject</returns>
    public static BadObject FromYaml(string s)
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithAttemptingUnquotedStringTypeDeserialization()
            .Build();
        var yamlObject = deserializer.Deserialize<ExpandoObject>(s);
        var json = JsonConvert.SerializeObject(yamlObject);
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter)!;

        return ConvertNode(JToken.FromObject(deserializedObject));
    }
    
    /// <summary>
    /// Converts a BadObject to a YAML string
    /// </summary>
    /// <param name="o">Object</param>
    /// <returns>YAML String</returns>
    public static string ToYaml(BadObject o)
    {
        // convert string/file to YAML object
        var json = ToJson(o);
        var expConverter = new ExpandoObjectConverter();
        dynamic deserializedObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter)!;

        var serializer = new YamlDotNet.Serialization.SerializerBuilder()
            .WithQuotingNecessaryStrings(true)
            .Build();
        var yaml = serializer.Serialize(deserializedObject);
        return yaml;
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