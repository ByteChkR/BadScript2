using System.Diagnostics;

using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Task;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

using Newtonsoft.Json;

namespace BadScript2.ConsoleCore.Systems.Shell;

public class BadShell
{
	private readonly BadExecutionContextOptions m_ContextOptions;
	private readonly BadTaskRunner m_TaskRunner;
	private BadExecutionContext? m_Context;
	private bool m_Exit;

	public BadShell(BadTaskRunner? runner = null, BadExecutionContextOptions? contextOptions = null)
	{
		m_TaskRunner = runner ?? BadTaskRunner.Instance;
		m_ContextOptions = contextOptions ?? BadExecutionContextOptions.Default;
		m_ContextOptions.AddApi(new BadShellApi(this));
	}

	private static string ConfigDirectory =>
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments,
				Environment.SpecialFolderOption.Create),
			".badshell");

	private void LoadUserSettings()
	{
		if (!Directory.Exists(ConfigDirectory))
		{
			Directory.CreateDirectory(ConfigDirectory);
		}

		//Load Environment Variables
		string envPath = Path.Combine(ConfigDirectory, "env.json");

		if (File.Exists(envPath))
		{
			Dictionary<string, string> env =
				JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(envPath))!;

			foreach (KeyValuePair<string, string> kvp in env)
			{
				Environment.SetEnvironmentVariable(kvp.Key, kvp.Value, EnvironmentVariableTarget.Process);
			}
		}
	}

	public void Initialize()
	{
		if (m_Context != null)
		{
			return;
		}

		m_Context = m_ContextOptions.Build();
	}

	public void Exit()
	{
		m_Exit = true;
	}

	private BadTask RunScript(string script)
	{
		BadTask task = new BadTask(new BadInteropRunnable(m_Context!.Execute(BadSourceParser.Parse("<stdin>", script))
					.GetEnumerator(),
				true),
			"<stdin>");
		m_TaskRunner.AddTask(task, true);

		return task;
	}

	private IEnumerable<BadObject> WaitFor(Process p)
	{
		while (!p.HasExited)
		{
			yield return BadObject.Null;
		}

		yield return p.ExitCode;
	}

	private BadTask RunCommand(string file, string[] args)
	{
		ProcessStartInfo psi = new ProcessStartInfo(file, string.Join(" ", args));
		psi.UseShellExecute = false;
		psi.CreateNoWindow = false;
		Process proc = Process.Start(psi)!;
		BadTask task = new BadTask(new BadInteropRunnable(WaitFor(proc).GetEnumerator(), true),
			file + " " + string.Join(" ", args));
		m_TaskRunner.AddTask(task, true);

		return task;
	}

	public BadTask? Run(string command)
	{
		Initialize();

		if (command.StartsWith('.')) //. <file> <args> Syntax
		{
			string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length < 2)
			{
				BadConsole.WriteLine($"Invalid Command: {command}");

				return null;
			}

			if (parts[1].EndsWith(".bs"))
			{
				BadExecutionContext ctx;

				if (parts[1].StartsWith("|"))
				{
					parts[1] = parts[1].Substring(1);
					ctx = m_Context!;
				}
				else
				{
					ctx = m_ContextOptions.Build();
				}

				IEnumerable<BadExpression> src = BadSourceParser.Parse(parts[1], File.ReadAllText(parts[1]));


				BadRuntimeApi.StartupArguments = parts.Skip(2);

				BadTask task = new BadTask(new BadInteropRunnable(ctx.Execute(src).GetEnumerator(),
						true),
					parts[1] + " " + string.Join(" ", parts.Skip(2)));
				m_TaskRunner.AddTask(task, true);

				return task;
			}

			return RunCommand(parts[1], parts.Skip(2).ToArray());
		}


		return RunScript(command);
	}

	public void RunUntilComplete(BadTask task)
	{
		while (!m_TaskRunner.IsIdle && task.IsRunning)
		{
			m_TaskRunner.RunStep();
		}
	}

	public int Run()
	{
		while (!m_Exit)
		{
			BadConsole.Write($"{Directory.GetCurrentDirectory()}>> ");
			Task<string> commandTask = BadConsole.ReadLineAsync();

			while (!commandTask.IsCompleted)
			{
				if (!m_TaskRunner.IsIdle)
				{
					m_TaskRunner.RunStep();
				}
				else
				{
					Thread.Sleep(100);
				}
			}

			string command = commandTask.Result;
			BadTask? task = Run(command);

			if (task == null)
			{
				continue;
			}

			RunUntilComplete(task);
		}

		return -1;
	}
}
