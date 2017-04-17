#region

using System;
using System.Diagnostics;
using HDT.Core.Hearthstone;
using HDT.Core.Utility.Logging;
using HearthMirror.Enums;
using HearthMirror.Objects;
using HearthWatcher;
using HearthWatcher.EventArgs;

#endregion

namespace HDT.Core
{
	public class CoreManager
	{
		private readonly ProcessWatcher _processWatcher;
		private readonly SceneModeWatcher _sceneModeWatcher;
		private readonly ArenaWatcher _arenaWatcher;
		private readonly BrawlInfoWatcher _brawlInfoWatcher;
		private Process _currentProcess;
		public Game CurrentGame { get; private set; }
		public Game PreviousGame { get; private set; }
		public GameMetaData MetaData { get; }


		public CoreManager()
		{
			Log.Initialize();
			_processWatcher = new ProcessWatcher();
			_processWatcher.OnStart += Process_OnStart;
			_processWatcher.OnExit += Process_OnExit;
			_sceneModeWatcher = new SceneModeWatcher();
			_sceneModeWatcher.OnSceneModeChanged += SceneModeWatcher_OnSceneModeChanged;
			_arenaWatcher = new ArenaWatcher();
			_arenaWatcher.OnCompleteDeck += ArenaWatcherOnOnCompleteDeck;
			_brawlInfoWatcher = new BrawlInfoWatcher();
			_brawlInfoWatcher.OnBrawlInfoChanged += BrawlInfoWatcherOnOnBrawlInfoChanged;
			MetaData = new GameMetaData();
		}

		private void BrawlInfoWatcherOnOnBrawlInfoChanged(BrawlInfo brawlInfo)
		{
			MetaData.BrawlInfo = brawlInfo;
		}

		private void ArenaWatcherOnOnCompleteDeck(object sender, CompleteDeckEventArgs args)
		{
			MetaData.Deck = args.Info.Deck;
			MetaData.ArenaInfo = args.Info;
		}

		private void SceneModeWatcher_OnSceneModeChanged(SceneMode current, SceneMode previous)
		{
			Log.Info($"SceneMode changed: {previous} -> {current}");

			if(current == SceneMode.GAMEPLAY)
				CurrentGame = new Game(_currentProcess, MetaData);
			else if(previous == SceneMode.GAMEPLAY)
			{
				CurrentGame.End();
				PreviousGame = CurrentGame;
				CurrentGame = null;
			}

			if(current == SceneMode.DRAFT)
				_arenaWatcher.Run();
			else if(previous == SceneMode.DRAFT)
				_arenaWatcher.Stop();

			if(current == SceneMode.TAVERN_BRAWL)
				_brawlInfoWatcher.Run();
			else if(previous == SceneMode.TAVERN_BRAWL)
				_brawlInfoWatcher.Stop();
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