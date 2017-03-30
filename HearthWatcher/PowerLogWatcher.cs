using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HearthWatcher.LogReader;

namespace HearthWatcher
{
	public class PowerLogWatcher : Watcher
	{
		private readonly LogFileWatcher _logFileWatcher;
		public Action<LogLine> OnPowerTaskList;
		public Action<LogLine> OnGameState;
		public Action OnPowerLogFound;
		public Action<string> OnLineIgnored;

		private static LogWatcherInfo PowerLogWatcherInfo => new LogWatcherInfo
		{
			Name = "Power",
			StartsWithFilters = new[] { "PowerTaskList.DebugPrintPower", "GameState." },
			ContainsFilters = new[] { "Begin Spectating", "Start Spectator", "End Spectator" }
		};

		public PowerLogWatcher(Process proc)
		{
			_logFileWatcher = new LogFileWatcher(PowerLogWatcherInfo);
			var executable = new FileInfo(proc.MainModule.FileName);
			if (executable.Directory == null)
				throw new Exception("Hearthstone directory not found");
			var logDirectory = Path.Combine(executable.Directory.FullName, "Logs");
			var entryPoint = _logFileWatcher.FindEntryPoint(logDirectory, "GameState.DebugPrintPower() - CREATE_GAME");
			_logFileWatcher.OnLogFileFound += (log) => OnPowerLogFound?.Invoke();
			_logFileWatcher.OnLogLineIgnored += (line) => OnLineIgnored?.Invoke(line);
			_logFileWatcher.Start(entryPoint, logDirectory);
		}

		public override void Update()
		{
			var lines = _logFileWatcher.Collect().ToList();
			foreach(var line in lines)
			{
				if(line.LineContent.StartsWith("GameState."))
					OnGameState?.Invoke(line);
				else
					OnPowerTaskList?.Invoke(line);
			}
		}
	}
}