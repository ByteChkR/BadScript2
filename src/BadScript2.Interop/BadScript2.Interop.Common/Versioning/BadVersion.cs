using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Versioning;

/// <summary>
///     Implements a Version Object
/// </summary>
public class BadVersion : BadObject, IBadNative
{
	/// <summary>
	///     The Version Class Prototype
	/// </summary>
	public static readonly BadClassPrototype
        Prototype = new BadNativeClassPrototype<BadVersion>("Version", VersionCtor);

    private static readonly BadClassPrototype s_Prototype;

    /// <summary>
    ///     The Change Version Function Reference
    /// </summary>
    private readonly BadObjectReference m_ChangeVersion;

    /// <summary>
    ///     The Inner Version Object
    /// </summary>
    private readonly Version m_Version;

    static BadVersion()
    {
        s_Prototype = new BadNativeClassPrototype<BadVersion>("Version", VersionCtor);
    }

    /// <summary>
    ///     Creates a new Version Object
    /// </summary>
    /// <param name="version">Version Object</param>
    public BadVersion(Version version)
    {
        m_Version = version;
        m_ChangeVersion = BadObjectReference.Make(
            "Version.ChangeVersion",
            () => new BadDynamicInteropFunction<string>(
                "ChangeVersion",
                (ctx, s) => new BadVersion(m_Version.ChangeVersion(s))
            )
        );
    }

    public bool Equals(IBadNative other)
    {
        return other is BadVersion v && m_Version.Equals(v.m_Version);
    }

    public object Value => m_Version;

    public Type Type => typeof(Version);


    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        return propName is IBadString str &&
               (str.Value == "Major" || str.Value == "Minor" || str.Value == "Build" || str.Value == "Revision") ||
               base.HasProperty(propName, caller);
    }

    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is IBadString str)
        {
            if (str.Value == "Major")
            {
                return BadObjectReference.Make("Version.Major", () => m_Version.Major);
            }

            if (str.Value == "Minor")
            {
                return BadObjectReference.Make("Version.Minor", () => m_Version.Minor);
            }

            if (str.Value == "Build")
            {
                return BadObjectReference.Make("Version.Build", () => m_Version.Build);
            }

            if (str.Value == "Revision")
            {
                return BadObjectReference.Make("Version.Revision", () => m_Version.Revision);
            }

            if (str.Value == "ChangeVersion")
            {
                return m_ChangeVersion;
            }
        }

        return base.GetProperty(propName, caller);
    }

    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <summary>
    ///     The Version Constructor
    /// </summary>
    /// <param name="ctx">Caller Context</param>
    /// <param name="args">Arguments</param>
    /// <returns>Version Object</returns>
    /// <exception cref="BadRuntimeException">Gets raised if the arguments are invalid</exception>
    private static BadObject VersionCtor(BadExecutionContext ctx, BadObject[] args)
    {
        if (args.Length == 0)
        {
            return new BadVersion(new Version());
        }

        if (args.Length == 1)
        {
            return args[0] is IBadString str ? new BadVersion(new Version(str.Value)) : throw BadRuntimeException.Create(ctx.Scope, "Version Constructor expects string as argument");
        }

        if (args.Length >= 2)
        {
            if (args[0] is not IBadNumber major)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Expected major version to be a number");
            }

            if (args[1] is not IBadNumber minor)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Expected minor version to be a number");
            }

            if (args.Length == 2)
            {
                return new BadVersion(new Version((int)major.Value, (int)minor.Value));
            }

            if (args[2] is not IBadNumber build)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Expected build version to be a number");
            }

            if (args.Length == 3)
            {
                return new BadVersion(new Version((int)major.Value, (int)minor.Value, (int)build.Value));
            }

            if (args[3] is not IBadNumber revision)
            {
                throw BadRuntimeException.Create(ctx.Scope, "Expected revision version to be a number");
            }

            if (args.Length == 4)
            {
                return new BadVersion(
                    new Version(
                        (int)major.Value,
                        (int)minor.Value,
                        (int)build.Value,
                        (int)revision.Value
                    )
                );
            }
        }

        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count for Version Constructor");
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return m_Version.ToString();
    }
}