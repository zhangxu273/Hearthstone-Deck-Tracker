using System.Diagnostics;
using HDT.Core.LogEventHandlers;
using HDT.Core.LogParsers;
using HDT.Core.Utility.Logging;
using HearthMirror.Objects;
using HearthWatcher;

namespace HDT.Core.Hearthstone
{
	public class Game
	{
		public GameState GameState { get; }
		public MatchInfo MatchInfo { get; private set; }
		public GameServerInfo GameServerInfo { get; private set; }
		private readonly PowerParser _powerParser;
		private readonly PowerHandler _powerHandler;
		private readonly MatchInfoWatcher _matchInfoWatcher;
		private readonly PlayerIdProvider _playerIdProvider;
		private readonly PowerLogWatcher _powerLogWatcher;
		private bool _ended;

		public Game(Process process)
		{
			_powerParser = new PowerParser();
			_matchInfoWatcher = new MatchInfoWatcher();
			_matchInfoWatcher.OnMatchInfoChanged += OnMatchInfoChanged;
			_matchInfoWatcher.OnGameServerInfoChanged += OnGameServerInfoChanged;
			_playerIdProvider = new PlayerIdProvider(_matchInfoWatcher);
			_powerHandler = new PowerHandler(_powerParser, _playerIdProvider);
			GameState = new GameState(_powerHandler);
			_matchInfoWatcher.Run();
			_powerLogWatcher = new PowerLogWatcher(process);
			_powerLogWatcher.OnPowerLogFound += OnPowerLogFound;
			_powerLogWatcher.OnPowerTaskList += _powerParser.Parse;
			_powerLogWatcher.Run();

			GameState.OnModified += GameEvents.OnGameEnd(End);
			GameState.OnModified += GameEvents.OnCardPlayed(entity => Log.Info($"Played: {entity.Id} - {entity.CardId}"));
			GameState.OnModified += GameEvents.OnCardDrawn(entity => Log.Info($"Drawn: {entity.Id} - {entity.CardId}"));
		}

		private void OnMatchInfoChanged(MatchInfo matchInfo)
		{
			MatchInfo = matchInfo;
		}

		private void OnGameServerInfoChanged(GameServerInfo gameServerInfo)
		{
			GameServerInfo = gameServerInfo;
		}

		private void OnPowerLogFound() => Log.Info("Power.log found");

		public async void End()
		{
			if(_ended)
				return;
			_ended = true;
			Log.Info("Game ended");
			_matchInfoWatcher.OnGameServerInfoChanged -= OnGameServerInfoChanged;
			_matchInfoWatcher.OnMatchInfoChanged -= OnMatchInfoChanged;
			_powerLogWatcher.OnPowerLogFound -= OnPowerLogFound;
			_powerLogWatcher.OnPowerTaskList -= _powerParser.Parse;
			await _powerLogWatcher.Stop();
		}
	}
}