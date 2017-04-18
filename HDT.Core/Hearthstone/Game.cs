using System;
using System.Collections.Generic;
using System.Diagnostics;
using HDT.Core.HsReplay;
using HDT.Core.LogEventHandlers;
using HDT.Core.LogParsers;
using HDT.Core.Utility;
using HDT.Core.Utility.Logging;
using HearthMirror.Objects;
using HearthWatcher;
using HearthWatcher.LogReader;

namespace HDT.Core.Hearthstone
{
	public class Game
	{
		public GameState GameState { get; }
		public MatchMetaData MetaData { get; }
		private readonly PowerParser _powerParser;
		private readonly PowerHandler _powerHandler;
		private readonly MatchInfoWatcher _matchInfoWatcher;
		private readonly PlayerIdProvider _playerIdProvider;
		private readonly PowerLogWatcher _powerLogWatcher;
		private bool _ended;
		private readonly List<string> _gameStateLog;

		public Game(Process process, GameMetaData gameMetaData)
		{
			_gameStateLog = new List<string>();
			_powerParser = new PowerParser();
			_matchInfoWatcher = new MatchInfoWatcher();
			_matchInfoWatcher.OnMatchInfoChanged += OnMatchInfoChanged;
			_matchInfoWatcher.OnGameServerInfoChanged += OnGameServerInfoChanged;
			_playerIdProvider = new PlayerIdProvider(_matchInfoWatcher);
			_powerHandler = new PowerHandler(_powerParser, _playerIdProvider);
			GameState = new GameState(_powerHandler);
			MetaData = new MatchMetaData(gameMetaData)
			{
				StartTime = DateTime.Now,
				Build = Helper.GetExecutableBuild(process)
			};
			_matchInfoWatcher.Run();
			_powerLogWatcher = new PowerLogWatcher(process);
			_powerLogWatcher.OnPowerLogFound += OnPowerLogFound;
			_powerLogWatcher.OnPowerTaskList += _powerParser.Parse;
			_powerLogWatcher.OnGameState += PowerLog_OnGameState;
			_powerLogWatcher.Run();

			GameState.OnModified += GameEvents.OnGameEnd(End);
			GameState.OnModified += GameEvents.OnCardPlayed(entity => Log.Info($"Played: {entity.Id} - {entity.CardId}"));
			GameState.OnModified += GameEvents.OnCardDrawn(entity => Log.Info($"Drawn: {entity.Id} - {entity.CardId}"));
		}

		private void PowerLog_OnGameState(LogLine line)
		{
			if(_gameStateLog.Count > 0 || line.LineContent.Contains("CREATE_GAME"))
				_gameStateLog.Add(line.Line);
		}

		private void OnMatchInfoChanged(MatchInfo matchInfo)
		{
			MetaData.MatchInfo = matchInfo;
		}

		private void OnGameServerInfoChanged(GameServerInfo gameServerInfo)
		{
			MetaData.ServerInfo = gameServerInfo;
		}

		private void OnPowerLogFound() => Log.Info("Power.log found");

		public void End()
		{
			if(_ended)
				return;
			_ended = true;
			Log.Info("Game ended");
			_matchInfoWatcher.OnGameServerInfoChanged -= OnGameServerInfoChanged;
			_matchInfoWatcher.OnMatchInfoChanged -= OnMatchInfoChanged;
			_powerLogWatcher.OnPowerLogFound -= OnPowerLogFound;
			_powerLogWatcher.OnPowerTaskList -= _powerParser.Parse;

			LogUploader.Upload(_gameStateLog.ToArray(), MetaData);
			_powerLogWatcher.Stop();
		}
	}
}