using BadScript2.Interop.NUnit;

using NUnitLite;

namespace BadScript2.Console.Systems.Test
{
    public class BadTestSystem : BadConsoleSystem<BadTestSystemSettings>
    {
        public override string Name => "test";

        protected override int Run(BadTestSystemSettings settings)
        {
            try
            {
                new AutoRun(typeof(BadUnitTests).Assembly).Execute(Array.Empty<string>());

                return -1;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);

                return e.HResult;
            }
        }
    }
}