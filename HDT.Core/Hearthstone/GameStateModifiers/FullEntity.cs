using HDT.Core.LogParsers.PowerData;

namespace HDT.Core.Hearthstone.GameStateModifiers
{
	public class FullEntity : IGameStateModifier
	{
		private readonly EntityData _data;

		public FullEntity(EntityData data)
		{
			_data = data;
		}
		public void Apply(GameState gameState)
		{
			gameState.Entities[_data.Id] = Entity.FromData(_data);
		}
	}
}