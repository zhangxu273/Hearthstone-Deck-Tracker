#region

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HDT.Core.LogEventHandlers;
using HearthMirror.Objects;
using HearthWatcher;

#endregion

namespace HDT.Core.Hearthstone
{
	public class PlayerIdProvider : IPlayerIdLookupProvider
	{
		public Dictionary<string, int> Players { get; }
		public event Action  OnUpdated;

		public PlayerIdProvider(MatchInfoWatcher matchInfoWatcher)
		{
			Players = new Dictionary<string, int>();
			matchInfoWatcher.OnMatchInfoChanged += OnMatchInfoChanged;
		}

		private void OnMatchInfoChanged(MatchInfo matchInfo)
		{
			if(matchInfo?.LocalPlayer != null && matchInfo.OpposingPlayer != null)
			{
				Players[matchInfo.LocalPlayer.Name] = matchInfo.LocalPlayer.Id;
				Players[matchInfo.OpposingPlayer.Name] = matchInfo.OpposingPlayer.Id;
				OnUpdated?.Invoke();
			}
		}
	}
}