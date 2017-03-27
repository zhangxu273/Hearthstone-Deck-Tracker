#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HDT.Core.Utility;
using HDT.Core.Utility.Logging;
using HearthWatcher;
using HearthWatcher.LogReader;

#endregion

namespace HDT.Core
{
	internal class LogWatcherManager
	{
		private readonly LogWatcher _logWatcher;
		private bool _stop;

		public LogWatcherManager()
		{
			_logWatcher = new LogWatcher(new[]
			{
				PowerLogWatcherInfo,
				RachelleLogWatcherInfo,
				ArenaLogWatcherInfo,
				LoadingScreenLogWatcherInfo,
				FullScreenFxLogWatcherInfo
			});
			_logWatcher.OnNewLines += OnNewLines;
			_logWatcher.OnLogFileFound += OnLogFileFound;
			_logWatcher.OnLogLineIgnored += OnLogLineIgnored;
		}

		private static LogWatcherInfo PowerLogWatcherInfo => new LogWatcherInfo
		{
			Name = "Power",
			StartsWithFilters = new[] { "PowerTaskList.DebugPrintPower", "GameState." },
			ContainsFilters = new[] { "Begin Spectating", "Start Spectator", "End Spectator" }
		};

		private static LogWatcherInfo RachelleLogWatcherInfo => new LogWatcherInfo { Name = "Rachelle" };
		private static LogWatcherInfo ArenaLogWatcherInfo => new LogWatcherInfo { Name = "Arena", Reset = false };

		private static LogWatcherInfo LoadingScreenLogWatcherInfo
			=> new LogWatcherInfo
				{
					Name = "LoadingScreen",
					StartsWithFilters = new[] { "LoadingScreen.OnSceneLoaded", "Gameplay" }
				};

		private static LogWatcherInfo FullScreenFxLogWatcherInfo => new LogWatcherInfo { Name = "FullScreenFX", Reset = false };

		public event Action<LogLine> OnPowerGameState;
		public event Action<LogLine> OnPowerPowerTaskList;
		public event Action<LogLine> OnRachelle;
		public event Action<LogLine> OnLoadingScreen;
		public event Action<LogLine> OnArena;
		public event Action<LogLine> OnFullScreenFx;

		private void OnLogFileFound(string msg) => Log.Info(msg);
		private void OnLogLineIgnored(string msg) => Log.Warn(msg);

		internal async Task Start()
		{
			var path = await HearthstoneProc.GetExecutablePath();
			_stop = false;
			_logWatcher.Start(Path.Combine(path, "Logs"));
		}

		internal async Task<bool> Stop(bool force = false)
		{
			_stop = true;
			return await _logWatcher.Stop(force);
		}

		private void OnNewLines(IEnumerable<LogLine> lines)
		{
			foreach(var line in lines)
			{
				if(_stop)
					break;
				switch(line.Namespace)
				{
					case "Power":
						if(line.LineContent.StartsWith("GameState."))
							OnPowerGameState?.Invoke(line);
						else
							OnPowerPowerTaskList?.Invoke(line);
						break;
					case "Rachelle":
						OnRachelle?.Invoke(line);
						break;
					case "Arena":
						OnArena?.Invoke(line);
						break;
					case "LoadingScreen":
						OnLoadingScreen?.Invoke(line);
						break;
					case "FullScreenFX":
						OnFullScreenFx?.Invoke(line);
						break;
				}
			}
		}
	}
}