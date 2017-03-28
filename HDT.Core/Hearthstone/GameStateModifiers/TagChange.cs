using HearthDb.Enums;

namespace HDT.Core.Hearthstone.GameStateModifiers
{
	public class TagChange : IGameStateModifier
	{
		private readonly int _entityId;
		private readonly GameTag _tag;
		private readonly int _value;

		public TagChange(int entityId, GameTag tag, int value)
		{
			_entityId = entityId;
			_tag = tag;
			_value = value;
		}

		public void Apply(GameState gameState)
		{
			gameState.Entities[_entityId].Tags[_tag] = _value;
		}
	}
}