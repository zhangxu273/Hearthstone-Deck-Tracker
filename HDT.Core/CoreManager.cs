#region

using System.Diagnostics;
using HDT.Core.Hearthstone;
using HDT.Core.Utility.Logging;
using HearthMirror.Enums;
using HearthWatcher;

#endregion

namespace HDT.Core
{
	public class CoreManager
	{
		private readonly ProcessWatcher _processWatcher;
		private readonly SceneModeWatcher _sceneModeWatcher;
		private Process _currentProcess;
		public Game CurrentGame { get; private set; }
		public Game PreviousGame { get; private set; }

		public CoreManager()
		{
			Log.InitializeDefault();
			_processWatcher = new ProcessWatcher();
			_processWatcher.OnStart += Process_OnStart;
			_processWatcher.OnExit += Process_OnExit;
			_sceneModeWatcher = new SceneModeWatcher();
			_sceneModeWatcher.OnSceneModeChanged += SceneModeWatcher_OnSceneModeChanged;
		}

		private void SceneModeWatcher_OnSceneModeChanged(SceneMode current, SceneMode previous)
		{
			Log.Info($"SceneMode changed: {previous} -> {current}");
			if(current == SceneMode.GAMEPLAY)
				CurrentGame = new Game(_currentProcess);
			else if(previous == SceneMode.GAMEPLAY)
			{
				CurrentGame.End();
				PreviousGame = CurrentGame;
				CurrentGame = null;
			}
		}

		private async void Process_OnExit(Process proc)
		{
			Log.Info($"Process with Id={proc.Id} exited. Stopping SceneMode watcher.");
			_currentProcess = null;
			await _sceneModeWatcher.Stop();
		}

		private void Process_OnStart(Process proc)
		{
			Log.Info($"Found process with Id={proc.Id}. Starting SceneMode watcher.");
			_currentProcess = proc;
			_sceneModeWatcher.Run();
		}

		public bool Running { get; private set; }

		public void Run()
		{
			if(Running)
				return;

			Log.Info("Waiting for Process...");
			_processWatcher.Run();

			Running = true;
		}
	}
}