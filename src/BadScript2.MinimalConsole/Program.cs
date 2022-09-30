using System;
using System.IO;

using BadHtml;

using BadScript2.ConsoleAbstraction;

namespace BadScript2.MinimalConsole
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            //Set Debugger Path
            BadHtmlTemplate.DebuggerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BadHtml", "Debugger.bs");
            
            
            if (args.Length < 1 || args.Length > 3)
            {
                BadConsole.WriteLine("Usage: BadScript2.MinimalConsole.exe [debug] <script> <UQL-Statement>");
            }
            bool debug = args[0]== "debug";
            string script = debug ? args[1] : args[0];

            BadHtmlTemplate.Run(script, null, debug);


        }
    }
}