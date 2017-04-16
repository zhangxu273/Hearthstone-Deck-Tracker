#region

using System;
using System.Collections.Generic;
using HDT.Core.Hearthstone.GameStateModifiers;
using HDT.Core.LogEventHandlers;
using HDT.Core.Hearthstone.Entities;

#endregion

namespace HDT.Core.Hearthstone
{
	public class GameState
	{
		public GameState(IGameEventSource events)
		{
			Entities = new Dictionary<int, Entity>();
			PlayerEntities = new Dictionary<int, PlayerEntity>();
			events.OnGameStateChange += Apply;
		}

		public Dictionary<int, Entity> Entities { get; }

		public GameEntity GameEntity { get; set; }

		public Dictionary<int, PlayerEntity> PlayerEntities { get; }

		public event Action<IGameStateModifier, GameState> OnModified;

		public void Apply(IGameStateModifier modifier)
		{
			modifier.Apply(this);
			OnModified?.Invoke(modifier, this);
		}
	}
}