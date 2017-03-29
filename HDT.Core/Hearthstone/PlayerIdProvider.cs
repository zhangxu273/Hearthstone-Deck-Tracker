#region

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HDT.Core.LogEventHandlers;
using HearthMirror.Objects;
using HearthWatcher;

#endregion

namespace HDT.Core.Hearthstone
{
	public class PlayerIdProvider : IPlayerIdLookupProvider
	{
		public Dictionary<string, int> Players { get; } = new Dictionary<string, int>();
		public event Action OnUpdated;
		private MatchInfoWatcher _matchInfoWatcher;

		//TODO: Start/Stop watcher
		public PlayerIdProvider()
		{
			_matchInfoWatcher = new MatchInfoWatcher();
			_matchInfoWatcher.OnMatchInfoChanged += OnMatchInfoChanged;
			_matchInfoWatcher.Run();
		}

		private void OnMatchInfoChanged(MatchInfo matchInfo)
		{
			if(matchInfo == null)
				Players.Clear();
			else if(matchInfo.LocalPlayer != null && matchInfo.OpposingPlayer != null)
			{
				Players[matchInfo.LocalPlayer.Name] = matchInfo.LocalPlayer.Id;
				Players[matchInfo.OpposingPlayer.Name] = matchInfo.OpposingPlayer.Id;
				OnUpdated?.Invoke();
			}
		}
	}
}