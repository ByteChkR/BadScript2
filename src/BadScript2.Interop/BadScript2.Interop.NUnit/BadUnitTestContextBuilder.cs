using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.NUnit
{
    public class BadUnitTestContextBuilder
    {
        private readonly List<BadInteropApi> m_Apis;
        private readonly List<BadNUnitTestCase> m_Cases = new List<BadNUnitTestCase>();
        private readonly List<BadFunction> m_Setup = new List<BadFunction>();
        private readonly List<BadFunction> m_Teardown = new List<BadFunction>();

        public BadUnitTestContextBuilder(IEnumerable<BadInteropApi> apis)
        {
            m_Apis = apis.ToList();
            m_Apis.Add(new BadNUnitApi());
            m_Apis.Add(new BadNUnitConsoleApi(this));
        }

        public BadUnitTestContextBuilder(params BadInteropApi[] apis) : this((IEnumerable<BadInteropApi>)apis) { }

        public void Register(params string[] files)
        {
            foreach (string file in files)
            {
                SetupStage(file);
            }
        }

        public BadUnitTestContext CreateContext()
        {
            return new BadUnitTestContext(m_Cases.ToList(), m_Setup.ToList(), m_Teardown.ToList());
        }

        public void AddTest(BadFunction function, BadObject testName)
        {
            string? name = null;
            if (testName != BadObject.Null)
            {
                name = (testName as IBadString)?.Value ?? throw new InvalidOperationException("Test name must be a string");
            }

            m_Cases.Add(new BadNUnitTestCase(function, name));
        }

        public void AddSetup(BadFunction function)
        {
            m_Setup.Add(function);
        }


        public void AddTeardown(BadFunction function)
        {
            m_Teardown.Add(function);
        }

        public void Reset()
        {
            m_Teardown.Clear();
            m_Setup.Clear();
            m_Cases.Clear();
        }


        private void LoadApis(BadExecutionContext context)
        {
            foreach (BadInteropApi api in m_Apis)
            {
                BadTable target;
                if (context.Scope.HasLocal(api.Name) && context.Scope.GetVariable(api.Name).Dereference() is BadTable table)
                {
                    target = table;
                }
                else
                {
                    target = new BadTable();
                    context.Scope.DefineVariable(api.Name, target);
                }

                api.Load(target);
            }
        }

        private void SetupStage(string file)
        {
            //Load expressions
            IEnumerable<BadExpression> expressions = BadSourceParser.Create(file, File.ReadAllText(file)).Parse();

            //Create Context
            BadExecutionContext context = BadExecutionContext.Create();

            //Add Apis
            LoadApis(context);

            context.Run(expressions);
        }
    }
}