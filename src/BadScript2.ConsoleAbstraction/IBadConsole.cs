using System;
using System.Threading.Tasks;

namespace BadScript2.ConsoleAbstraction
{
    public interface IBadConsole
    {
        void Write(string str);
        void WriteLine(string str);
        string ReadLine();
        Task<string> ReadLineAsync();
        void Clear();
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
    }
}