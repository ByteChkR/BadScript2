using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

///<summary>
///	Contains Versioning Extensions and APIs for the BadScript2 Runtime
/// </summary>
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
        Prototype = new BadNativeClassPrototype<BadVersion>("Version", VersionCtor,
            null);

    /// <summary>
    ///     The Version Class Prototype
    /// </summary>
    private static readonly BadClassPrototype s_Prototype;

    /// <summary>
    ///     The Change Version Function Reference
    /// </summary>
    private readonly BadObjectReference m_ChangeVersion;

    /// <summary>
    ///     The Inner Version Object
    /// </summary>
    private readonly Version m_Version;

    /// <summary>
    /// Static Constructor
    /// </summary>
    static BadVersion()
    {
        s_Prototype = new BadNativeClassPrototype<BadVersion>("Version", VersionCtor,
            null);
    }

    /// <summary>
    ///     Creates a new Version Object
    /// </summary>
    /// <param name="version">Version Object</param>
    public BadVersion(Version version)
    {
        m_Version = version;

        m_ChangeVersion = BadObjectReference.Make("Version.ChangeVersion",
                                                  (p) => new BadDynamicInteropFunction<string>("ChangeVersion",
                                                       (_, s) => new BadVersion(m_Version.ChangeVersion(s)),
                                                       s_Prototype,
                                                       new BadFunctionParameter("changeFormat", false, true, false, null, BadNativeClassBuilder.GetNative("string"))
                                                      )
                                                 );
    }

#region IBadNative Members

    /// <summary>
    ///     Checks if the Version is equal to another Version
    /// </summary>
    /// <param name="other">Other Version</param>
    /// <returns>True if the Versions are equal</returns>
    public bool Equals(IBadNative other)
    {
        return other is BadVersion v && m_Version.Equals(v.m_Version);
    }

    /// <inheritdoc />
    public object Value => m_Version;

    /// <inheritdoc />
    public Type Type => typeof(Version);

#endregion


    /// <inheritdoc />
    public override bool HasProperty(string propName, BadScope? caller = null)
    {
        return propName is "Major" or "Minor" or "Build" or "Revision" or "ChangeVersion" ||
               base.HasProperty(propName, caller);
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
    {
        return propName switch
        {
            "Major"         => BadObjectReference.Make("Version.Major", (p) => m_Version.Major),
            "Minor"         => BadObjectReference.Make("Version.Minor", (p) => m_Version.Minor),
            "Build"         => BadObjectReference.Make("Version.Build", (p) => m_Version.Build),
            "Revision"      => BadObjectReference.Make("Version.Revision", (p) => m_Version.Revision),
            "ChangeVersion" => m_ChangeVersion,
            _               => base.GetProperty(propName, caller),
        };
    }

    /// <inheritdoc />
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
        switch (args.Length)
        {
            case 0:
                return new BadVersion(new Version());
            case 1:
                return args[0] is IBadString str
                           ? new BadVersion(new Version(str.Value))
                           : throw BadRuntimeException.Create(ctx.Scope,
                                                              "Version Constructor expects string as argument"
                                                             );
            case >= 2:
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
                    return new BadVersion(new Version((int)major.Value,
                                                      (int)minor.Value,
                                                      (int)build.Value,
                                                      (int)revision.Value
                                                     )
                                         );
                }

                break;
            }
        }

        throw BadRuntimeException.Create(ctx.Scope, "Invalid Argument Count for Version Constructor");
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return m_Version.ToString();
    }
}