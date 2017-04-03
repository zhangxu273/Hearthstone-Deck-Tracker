using HearthDb.Enums;

namespace HDT.Core.Hearthstone.GameStateModifiers
{
	public class TagChange : IGameStateModifier
	{
		public int Value { get; }
		public GameTag Tag { get; }
		public int EntityId { get; }
		public int? PreviousValue { get; private set; }

		public TagChange(int entityId, GameTag tag, int value)
		{
			EntityId = entityId;
			Tag = tag;
			Value = value;
		}

		public void Apply(GameState gameState)
		{
			var entity = gameState.Entities[EntityId];
			PreviousValue = entity.GetTag(Tag);
			entity.Tags[Tag] = Value;
		}
	}
}