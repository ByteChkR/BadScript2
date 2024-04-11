using System.Text;

using BadScript2.ConsoleAbstraction;

using Newtonsoft.Json;

using XtermBlazor;
namespace BadScript2.Web.Frontend.Utils;

public class XTermConsole : IBadConsole
{
    private readonly TerminalOptions _options;
    private readonly Xterm _terminal;
    private readonly StringBuilder _inputBuffer = new StringBuilder();
    private int _cursorPosition;
    private readonly Queue<string> _inputQueue = new Queue<string>();

    public XTermConsole(TerminalOptions options, Xterm terminal)
    {
        _options = options;
        _terminal = terminal;
        _terminal.AttachCustomKeyEventHandler((args) =>
        {
            if (args.Type != "keydown")
                return true;
            if (args.Key == "Enter")
            {
                _inputQueue.Enqueue(_inputBuffer.ToString());
                _inputBuffer.Clear();
                _cursorPosition = 0;
                WriteLine(string.Empty);
            }
            else if (args.Key == "Backspace")
            {
                if (_inputBuffer.Length > 0 && _cursorPosition > 0)
                {
                    _inputBuffer.Remove(_cursorPosition - 1, 1);
                    _cursorPosition--;
                    // Move cursor back
                    Write("\b");
                    // Write out the rest of the line with space to clear the character
                    Write(_inputBuffer.ToString().Substring(_cursorPosition) + " ");
                    // Move cursor back to the original position
                    Write(new string('\b', _inputBuffer.Length - _cursorPosition + 1));
                }
            }
            else if(args.Key == "ArrowLeft")
            {
                if (_cursorPosition > 0)
                {
                    _cursorPosition--;
                    Write("\b");
                }
            }
            else if(args.Key == "ArrowRight")
            {
                if (_cursorPosition < _inputBuffer.Length)
                {
                    _cursorPosition++;
                    Write("\u001b[C");
                }
            }
            else if(args.Key.Length == 1)
            {
                if(_cursorPosition == _inputBuffer.Length)
                {
                    _inputBuffer.Append(args.Key);
                    _cursorPosition++;
                    Write(args.Key);
                }
                else
                {
                    _inputBuffer.Insert(_cursorPosition, args.Key);
                    _cursorPosition++;
                    Write(args.Key);
                    Write(_inputBuffer.ToString().Substring(_cursorPosition));
                    Write(new string('\b', _inputBuffer.Length - _cursorPosition));
                }
            }
            else if(args.Key == "Tab")
            {
                _inputBuffer.Insert(_cursorPosition, "    ");
                _cursorPosition += 4;
                Write("    ");
            }
            else
            {
                Console.WriteLine("Unknown Key: " + args.Key);   
            }

            return true;
        });
    }

    private ConsoleColor _foregroundColor;
    public ConsoleColor ForegroundColor { get => _foregroundColor; set => SetForegroundColor(value); }


    private ConsoleColor _backgroundColor;
    public ConsoleColor BackgroundColor { get => _backgroundColor; set => SetBackgroundColor(value); }
    
    private static Dictionary<ConsoleColor, string> _colorMap = new Dictionary<ConsoleColor, string>
    {
        {ConsoleColor.Black, "#000000"},
        {ConsoleColor.DarkBlue, "#000080"},
        {ConsoleColor.DarkGreen, "#008000"},
        {ConsoleColor.DarkCyan, "#008080"},
        {ConsoleColor.DarkRed, "#800000"},
        {ConsoleColor.DarkMagenta, "#800080"},
        {ConsoleColor.DarkYellow, "#808000"},
        {ConsoleColor.Gray, "#c0c0c0"},
        {ConsoleColor.DarkGray, "#808080"},
        {ConsoleColor.Blue, "#0000ff"},
        {ConsoleColor.Green, "#00ff00"},
        {ConsoleColor.Cyan, "#00ffff"},
        {ConsoleColor.Red, "#ff0000"},
        {ConsoleColor.Magenta, "#ff00ff"},
        {ConsoleColor.Yellow, "#ffff00"},
        {ConsoleColor.White, "#ffffff"}
    };

    private void SetBackgroundColor(ConsoleColor color)
    {
        _backgroundColor = color;
        _options.Theme.Background = _colorMap[color];
        _terminal.SetOptions(_options);
    }
    
    private void SetForegroundColor(ConsoleColor color)
    {
        _foregroundColor = color;
        _options.Theme.Foreground = _colorMap[color];
        _terminal.SetOptions(_options);
    }

    public void Write(string str)
    {
        if (str.Contains('\n'))
        {
            string[] lines = str.Split('\n');
            foreach (string line in lines.SkipLast(1))
            {
                _terminal.WriteLine(line);
            }
            _terminal.Write(lines.Last());
            return;
        }
        _terminal.Write(str);
    }
    public void WriteLine(string str)
    {
        if (str.Contains('\n'))
        {
            string[] lines = str.Split('\n');
            foreach (string line in lines)
            {
                _terminal.WriteLine(line);
            }
            return;
        }
        _terminal.WriteLine(str);
    }
    public string ReadLine()
    {
        if(_inputQueue.Count > 0)
        {
            return _inputQueue.Dequeue();
        }
        throw new NotSupportedException("Can not read line synchronously from XTermConsole");
    }
    public async Task<string> ReadLineAsync()
    {
        if(_inputQueue.Count > 0)
        {
            return _inputQueue.Dequeue();
        }
        
        while (_inputQueue.Count == 0)
        {
            await Task.Delay(100);   
        }
        
        return _inputQueue.Dequeue();
    }
    public void Clear()
    {
        _terminal.Clear();
    }
}