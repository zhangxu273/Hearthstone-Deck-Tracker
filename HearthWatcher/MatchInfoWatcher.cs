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
			//TODO: Fix comparisons, '!=' always returns true since they are different objects
			var matchInfo = Reflection.GetMatchInfo();
			if(matchInfo != _matchInfo)
			{
				_matchInfo = matchInfo;
				OnMatchInfoChanged?.Invoke(_matchInfo);
			}

			var gameServerInfo = Reflection.GetServerInfo();
			if(gameServerInfo != _gameServerInfo)
			{
				_gameServerInfo = gameServerInfo;
				OnGameServerInfoChanged?.Invoke(gameServerInfo);
			}
		}
	}
}