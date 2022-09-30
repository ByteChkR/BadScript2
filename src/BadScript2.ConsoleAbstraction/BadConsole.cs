using System;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations;

namespace BadScript2.ConsoleAbstraction
{
    public static class BadConsole
    {
        private static IBadConsole m_Console;

        static BadConsole()
        {
            m_Console = new SystemConsole();
        }

        public static void SetConsole(IBadConsole console) => m_Console = console;
        public static void Write(string str) => m_Console.Write(str);
        public static void WriteLine(string str) => m_Console.WriteLine(str);
        public static string ReadLine() => m_Console.ReadLine();
        public static Task<string> ReadLineAsync() => m_Console.ReadLineAsync();
        public static void Clear() => m_Console.Clear();

        public static ConsoleColor ForegroundColor
        {
            get => m_Console.ForegroundColor;
            set => m_Console.ForegroundColor = value;
        }

        public static ConsoleColor BackgroundColor
        {
            get => m_Console.BackgroundColor;
            set => m_Console.BackgroundColor = value;
        }
        
    }
}