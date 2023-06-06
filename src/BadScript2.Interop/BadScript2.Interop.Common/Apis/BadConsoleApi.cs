using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
/// Implements the "Console" API
/// </summary>
public class BadConsoleApi : BadInteropApi
{
	/// <summary>
	/// The Console Implementation that is used
	/// </summary>
	private readonly IBadConsole? m_Console;

	/// <summary>
	/// Constructs a new Console API Instance
	/// </summary>
	public BadConsoleApi() : this(BadConsole.GetConsole()) { }

	/// <summary>
	/// Constructs a new Console API Instance
	/// </summary>
	/// <param name="console">Console Implementation to use</param>
	public BadConsoleApi(IBadConsole console) : base("Console")
	{
		m_Console = console;
		OnWrite = Write;
		OnWriteLine = WriteLine;
		OnClear = Clear;
		OnReadLine = ReadLine;
		OnReadLineAsync = ReadLineAsync;
	}

	/// <summary>
	/// The Console Implementation that is used
	/// </summary>
	private IBadConsole Console => m_Console ?? BadConsole.GetConsole();

	/// <summary>
	/// Event Handler for the "Write" Function
	/// </summary>
	public Action<BadObject> OnWrite { get; set; }
	
	/// <summary>
	/// Event Handler for the "WriteLine" Function
	/// </summary>
	public Action<BadObject> OnWriteLine { get; set; }

	/// <summary>
	/// Event Handler for the "Clear" Function
	/// </summary>
	public Action OnClear { get; set; }

	/// <summary>
	/// Event Handler for the "ReadLine" Function
	/// </summary>
	public Func<string> OnReadLine { get; set; }

	/// <summary>
	/// Event Handler for the "ReadLineAsync" Function
	/// </summary>
	public Func<Task<string>> OnReadLineAsync { get; set; }

	/// <summary>
	/// If Set to false, the Console will throw an error if console input is requested.
	/// </summary>
	public bool AllowInput { get; set; } = true;

	/// <summary>
	/// Wrapper that calls the "Write" Function in a new Task
	/// </summary>
	/// <returns>Task</returns>
	private Task<string> ReadLineAsync()
	{
		return System.Threading.Tasks.Task.Run(Console.ReadLine);
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction("WriteLine", OnWriteLine);
		target.SetFunction("Write", OnWrite);
		target.SetFunction("Clear", OnClear);
		target.SetFunction("ReadLine", () => OnReadLine());
		target.SetFunction("ReadLineAsync",
			() => new BadTask(new ReadLineAsyncRunnable(ReadLineAsyncBlocking().GetEnumerator()),
				"Console.ReadLineAsync"));
	}

	/// <summary>
	/// Wrapper that will block until the Task is completed
	/// </summary>
	/// <returns>Awaitable Enumeration in BadScript</returns>
	private IEnumerable<BadObject> ReadLineAsyncBlocking()
	{
		Task<string>? t = OnReadLineAsync();

		while (!t.IsCompleted)
		{
			yield return BadObject.Null;
		}

		yield return t.Result;
	}

	/// <summary>
	/// Writes an Object to the Console
	/// </summary>
	/// <param name="obj">The Object to write</param>
	private void Write(BadObject obj)
	{
		if (obj is IBadString str)
		{
			Console.Write(str.Value);
		}
		else
		{
			Console.Write(obj.ToString());
		}
	}

	/// <summary>
	/// Writes an Object to the Console. Appends a newline.
	/// </summary>
	/// <param name="obj">The Object to write</param>
	private void WriteLine(BadObject obj)
	{
		if (obj is IBadString str)
		{
			Console.WriteLine(str.Value);
		}
		else
		{
			Console.WriteLine(obj.ToString());
		}
	}

	/// <summary>
	/// Clears the Console
	/// </summary>
	public void Clear()
	{
		Console.Clear();
	}

	/// <summary>
	/// Reads a line from the Console
	/// </summary>
	/// <returns>Console Input</returns>
	/// <exception cref="Exception">Gets raised if AllowInput is false</exception>
	public string ReadLine()
	{
		if (!AllowInput)
		{
			throw new NotSupportedException("Input is not allowed");
		}

		return Console.ReadLine() ?? "";
	}

	/// <summary>
	/// Awaitable Enumeration that wraps the ReadLineAsync Task
	/// </summary>
	private class ReadLineAsyncRunnable : BadRunnable
	{
		/// <summary>
		/// The Wrapped Task
		/// </summary>
		private readonly IEnumerator<BadObject> m_Task;
		
		/// <summary>
		/// The Return Value
		/// </summary>
		private BadObject m_Return = BadObject.Null;

		/// <summary>
		/// Constructs a new Instance
		/// </summary>
		/// <param name="task">The Awaitable Enumeration</param>
		public ReadLineAsyncRunnable(IEnumerator<BadObject> task)
		{
			m_Task = task;
		}

		/// <summary>
		/// The Enumerator that wraps the Task
		/// </summary>
		public override IEnumerator<BadObject> Enumerator
		{
			get
			{
				while (m_Task.MoveNext())
				{
					m_Return = m_Task.Current!;

					yield return m_Return;
				}
			}
		}

		/// <summary>
		/// The Return Value
		/// </summary>
		/// <returns>Return Value</returns>
		public override BadObject GetReturn()
		{
			return m_Return;
		}
	}
}
