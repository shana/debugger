using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using Debugger.Backend;
using NDesk.Options;

namespace Debugger
{
	public class Program
	{
		static int Port { get; set; }
		static string Path { get; set; }

		DebuggerSession session;

		delegate bool CommandHandler (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls);
		Dictionary<string, Dictionary<string, CommandHandler>> Commands = new Dictionary<string, Dictionary<string, CommandHandler>> ();

		static void Main (string[] args)
		{
			var p = new OptionSet () {
				{ "p|port=", v => Port = int.Parse(v) },
				{ "s|path=", v => Path = v }
			};

			try
			{
				p.Parse (args);
			}
			catch (OptionException e)
			{
				Console.WriteLine ("Error parsing option '{0}' : {1}", e.OptionName, e.Message);
				Console.WriteLine ();
				return;
			}

			if (Port == 0)
			{

				var f = new StreamReader (File.Open (@"C:\debug.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				var str = f.ReadLine ();
				f.Close ();
				Port = int.Parse (str.Substring ("Listening on 0.0.0.0:".Length, 5));
			}
		}

		public Program ()
		{
			this.session = new DebuggerSession();

			var subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("threads", ListThreads);
			subs.Add ("breakpoints", ListBreakpoints);
			subs.Add ("b", ListBreakpoints);
			subs.Add ("sources", ListSources);
			subs.Add ("s", ListSources);
			subs.Add ("types", ListTypes);
			subs.Add ("t", ListTypes);
			subs.Add ("assemblies", ListAssemblies);
			subs.Add ("a", ListAssemblies);

			Commands.Add ("info", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", DoBreak);
			Commands.Add ("b", subs);
			Commands.Add ("break", subs);
			Commands.Add ("d", subs);
			Commands.Add ("delete", subs);
			Commands.Add ("enable", subs);
			Commands.Add ("disable", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", DoExecution);
			Commands.Add ("step", subs);
			Commands.Add ("next", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", DoExit);
			Commands.Add ("q", subs);
			Commands.Add ("quit", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", DoHelp);
			Commands.Add ("h", subs);
			Commands.Add ("help", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", (c, cs, cl) => { this.session.ExecutionProvider.Resume (); return true; });
			Commands.Add ("c", subs);
			Commands.Add ("continue", subs);

			subs = new Dictionary<string, CommandHandler> ();
			subs.Add ("", (c, cs, cl) =>
				{
					Console.WriteLine();
					Console.WriteLine(this.session.ExecutionProvider.Running ? "running" : "paused");
					return true;
				});
			Commands.Add ("status", subs);

		}

		public void Run ()
		{
			session.TypeProvider.BasePath = Path;
			session.Port = Port;

			bool active = false;
			if (File.Exists ("console.log"))
				File.Delete ("console.log");

			session.TraceCallback += s => File.AppendAllText ("console.log", s + "\n");
			session.ExecutionProvider.Break += () => {
				var location = session.ExecutionProvider.Location;
				Console.WriteLine ();
				Console.WriteLine ("breaking on {0}:{1}", location.SourceFile, location.LineNumber);
				using (var fs = File.OpenText (location.SourceFile))
				{
					int start = location.LineNumber - 5;
					if (start < 0)
						start = 0;
					int end = start + 6;
					string line;
					int i = 0;
					for (i = 0, line = fs.ReadLine (); i < end || !fs.EndOfStream; i++, line = fs.ReadLine ())
					{
						if (i >= start) {
							if (i == location.LineNumber)
								Console.Write ("--> ");
							Console.WriteLine (line);
						}
					}
				}
				Console.WriteLine ();

			};

			Console.Write ("connecting");
			session.Start ();

			while (!session.Active)
			{
				Console.Write (".");
				Thread.Sleep (300);
			}

			Console.WriteLine ();

			while (true)
			{
				Console.WriteLine ();
				Console.Write ("(udb) ");
				var commands = new Stack<string> ((Console.ReadLine () ?? "").Trim ().Split ().Reverse ());
				if (!DoCommand (commands))
					break;
			}
		}

		private bool ListAssemblies (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			foreach (var a in session.VM.Assemblies)
				Console.WriteLine ("{0}", a);
			return true;
		}

		private bool ListSources (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			foreach (var t in session.TypeProvider.SourceFiles)
				Console.WriteLine ("{0}", t);
			return true;
		}

		private bool ListTypes (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			foreach (var t in session.TypeProvider.LoadedTypes)
				Console.WriteLine ("{0}", t);
			return true;
		}

		//private bool ListAssemblies (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		//{
		//    foreach (var t in session.LoadedAssemblies)
		//        Console.WriteLine ("{0}", t);
		//    return true;
		//}

		private bool DoHelp (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			foreach (var c in Commands)
			{
				if (c.Value.ContainsKey (""))
					Console.WriteLine (c.Key);
				else
				{
					foreach (var cs in c.Value.Where (s => s.Key != ""))
						Console.WriteLine (c.Key + " " + cs.Key);
				}
			}
			Console.WriteLine ();
			return true;
		}

		private bool DoBreak (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			if (commands.Count < 1)
				return true;

			var arg1 = commands.Pop ();
			int br;
			if (int.TryParse (arg1, out br))
				br--;
			switch (command)
			{
				case "break":
				case "b":
					{
						var file = arg1;
						var line = int.Parse (commands.Pop ());
						session.BreakpointProvider.AddBreakpoint (file, line);
						Console.WriteLine ("added breakpoint {0} on {1}:{2}", session.BreakpointProvider.Breakpoints.Count (), file, line);
					}
					break;
				case "d":
				case "delete":
					{
						if (br >= session.BreakpointProvider.Breakpoints.Count () || session.BreakpointProvider[br] == null)
						{
							Console.WriteLine ("breakpoint {0} doesn't exist", arg1);
							return true;
						}
						var breakpoint = session.BreakpointProvider[br];
						session.BreakpointProvider.RemoveBreakpoint (breakpoint);
						Console.WriteLine ("deleted breakpoint {0} on {1}:{2}", arg1, breakpoint.Location.SourceFile, breakpoint.Location.LineNumber);
					}
					break;
				case "enable":
					{
						if (br >= session.BreakpointProvider.Breakpoints.Count () || session.BreakpointProvider[br] == null)
						{
							Console.WriteLine ("breakpoint {0} doesn't exist", arg1);
							return true;
						}
						var breakpoint = session.BreakpointProvider[br];
						if (!breakpoint.Enabled)
						{
							breakpoint.Enable ();
							Console.WriteLine ("enabled breakpoint {0} on {1}:{2}", arg1, breakpoint.Location.SourceFile, breakpoint.Location.LineNumber);
						}
						else
							Console.WriteLine ("breakpoint {0} is already enabled", arg1);
					}
					break;
				case "disable":
					{
						if (br >= session.BreakpointProvider.Breakpoints.Count () || session.BreakpointProvider[br] == null)
						{
							Console.WriteLine ("breakpoint {0} doesn't exist", arg1);
							return true;
						}
						var breakpoint = session.BreakpointProvider[br];
						if (breakpoint.Enabled)
						{
							breakpoint.Disable ();
							Console.WriteLine ("disabled breakpoint {0} on {1}:{2}", arg1, breakpoint.Location.SourceFile, breakpoint.Location.LineNumber);
						}
						else
							Console.WriteLine ("breakpoint {0} is already disabled", arg1);

					}
					break;
				default:
					return true;
			}

			return true;
		}

		private bool DoExecution (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			session.ExecutionProvider.Step ();
			return true;
		}

		private bool ListBreakpoints (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			var bps = session.BreakpointProvider.Breakpoints.Keys.ToArray ();
			for (int i = 0; i < bps.Length; i++)
			{
				var l = bps[i].Location;
				Console.WriteLine ("breakpoint {0} on {1}:{2}", i + 1, l.SourceFile, l.LineNumber);
			}
			return true;
		}

		private bool ListThreads (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			session.VM.Suspend ();
			var threads = session.VM.Threads;
			foreach (var t in threads)
			{
				Console.WriteLine ("{0} '{1}' {2}", t.Id, t.Name, t.GetFrames ().Count);
			}
			session.VM.Resume ();

			return true;
		}

		bool DoCommand (Stack<string> commands, Dictionary<string, CommandHandler> calls = null)
		{
			var command = commands.Peek ();
			if (calls == null)
			{
				if (!Commands.ContainsKey (command))
					return true;
				return DoCommand (commands, Commands[command]);
			}

			commands.Pop ();
			if (calls.ContainsKey (""))
				return calls[""] (command, commands, null);
			if (commands.Count == 0)
				return true;
			command = commands.Pop ();
			if (calls.ContainsKey (command))
				return calls[command] (command, commands, null);
			return true;
		}

		private bool DoExit (string command, Stack<string> commands, Dictionary<string, CommandHandler> calls)
		{
			session.Stop ();
			Console.WriteLine ("exiting...");
			return false;
		}
	}
}
