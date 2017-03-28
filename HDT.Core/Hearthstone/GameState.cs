#region

using System.Collections.Generic;
using HDT.Core.Hearthstone.GameStateModifiers;

#endregion

namespace HDT.Core.Hearthstone
{
	public class GameState
	{
		public GameState()
		{
			Entities = new Dictionary<int, Entity>();
		}

		public Dictionary<int, Entity> Entities { get; }

		public void Apply(IGameStateModifier modifier)
		{
			modifier.Apply(this);
		}
	}
}