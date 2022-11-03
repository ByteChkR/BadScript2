using System;
using System.Threading.Tasks;

namespace BadScript2.ConsoleAbstraction
{
    public interface IBadConsole
    {
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        void Write(string str);
        void WriteLine(string str);
        string ReadLine();
        Task<string> ReadLineAsync();
        void Clear();
    }
}