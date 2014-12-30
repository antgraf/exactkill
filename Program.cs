using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace ExactKill
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				if(args.Length != 2)
				{
					Usage();
					return;
				}

				FindAndKill(args[0], args[1]);
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
			//Console.ReadKey(true);
		}

		private static void Usage()
		{
			Console.WriteLine("Usage: ExactKill.exe \"Process Name\" \"Command line parameter\"");
			Console.WriteLine("This tool searches for processes with corresponding process name and command line parameters and kills them.");
			Console.WriteLine("Command line parameter may be just any part of the command line.");
			Console.WriteLine("Example: ExactKill.exe node.exe http-server");
		}

		private static void FindAndKill(string name, string evidence)
		{
			List<string> ids = Find(name, evidence);
			foreach(string id in ids)
			{
				Kill(id);
			}
		}

		private static List<string> Find(string name, string evidence)
		{
			List<string> ret = new List<string>();

			string wmiQuery = string.Format("select Handle, CommandLine from Win32_Process where Name='{0}'", name);
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
			ManagementObjectCollection retObjectCollection = searcher.Get();
			foreach(ManagementBaseObject bo in retObjectCollection)
			{
				try
				{
					ManagementObject retObject = (ManagementObject) bo;
					if(retObject["CommandLine"].ToString().Contains(evidence))
					{
						ret.Add(retObject["Handle"].ToString());
					}
				}
				catch(Exception e)
				{
					Console.WriteLine("Error accessing Win32_Process WMI object: {0}", e);
				}
			}

			return ret;
		}

		private static void Kill(string id)
		{
			Console.WriteLine("Kill process #{0}", id);
			try
			{
				Process.GetProcessById(int.Parse(id)).Kill();
			}
			catch(Exception e)
			{
				Console.WriteLine("Error killing a process: {0}", e);
			}
		}
	}
}
