using System;
using System.Diagnostics;
using System.Linq;

namespace HearthWatcher
{
	public class ProcessWatcher : Watcher
	{
		public Action<Process> OnStart;
		public Action<Process> OnExit;
		private Process _proc;

		public override void Update()
		{
			try
			{
				var proc = GetProcess();
				if(proc?.Id != _proc?.Id)
				{
					if(proc == null)
						OnExit?.Invoke(_proc);
					else
						OnStart?.Invoke(proc);
					_proc = proc;
				}
			}
			catch(Exception)
			{
			}
		}

		private static Process GetProcess()
		{
			return Process.GetProcessesByName("Hearthstone").FirstOrDefault();
		}
	}
}
