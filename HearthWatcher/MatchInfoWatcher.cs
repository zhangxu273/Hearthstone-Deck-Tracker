using System;
using HearthMirror;
using HearthMirror.Objects;

namespace HearthWatcher
{
	public class MatchInfoWatcher : Watcher
	{
		public event Action<MatchInfo> OnMatchInfoChanged;
		public event Action<GameServerInfo> OnGameServerInfoChanged;

		private MatchInfo _matchInfo;
		private GameServerInfo _gameServerInfo;

		protected override void Reset()
		{
			_matchInfo = null;
			_gameServerInfo = null;
		}

		public override void Update()
		{
			var matchInfo = Reflection.GetMatchInfo();
			if(_matchInfo == null && matchInfo?.LocalPlayer != null 
				&& matchInfo.OpposingPlayer != null && matchInfo.GameType > 0)
			{
				_matchInfo = matchInfo;
				OnMatchInfoChanged?.Invoke(_matchInfo);
			}

			var gameServerInfo = Reflection.GetServerInfo();
			if(_gameServerInfo == null && gameServerInfo != null)
			{
				_gameServerInfo = gameServerInfo;
				OnGameServerInfoChanged?.Invoke(gameServerInfo);
			}

			if(_matchInfo != null && _gameServerInfo != null)
				Stop();
		}
	}
}