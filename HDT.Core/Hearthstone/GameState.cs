#region

using System;
using System.Collections.Generic;
using HDT.Core.Hearthstone.GameStateModifiers;
using HDT.Core.LogEventHandlers;

#endregion

namespace HDT.Core.Hearthstone
{
	public class GameState
	{
		public GameState(IGameEventSource events)
		{
			Entities = new Dictionary<int, Entity>();
			events.OnGameStateChange += Apply;
		}

		public event Action OnModified;

		public Dictionary<int, Entity> Entities { get; }

		public void Apply(IGameStateModifier modifier)
		{
			modifier.Apply(this);
			OnModified?.Invoke();
		}
	}
}