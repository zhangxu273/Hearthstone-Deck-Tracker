using HDT.Core.Hearthstone.Entities;
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
			var entity = EntityFactory.FromData(_data);

			if(entity is GameEntity gameEntity)
				gameState.GameEntity = gameEntity;
			else if(entity is PlayerEntity playerEntity)
				gameState.PlayerEntities[playerEntity.PlayerId] = playerEntity;

			gameState.Entities[_data.Id] = entity;
		}
	}
}