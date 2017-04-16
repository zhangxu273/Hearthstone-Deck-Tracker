using HDT.Core.LogParsers.PowerData;
using HearthDb.Enums;

namespace HDT.Core.Hearthstone.Entities
{
	public static class EntityFactory
	{
		public static Entity FromData(EntityData data)
		{
			var entity = new Entity(data.Id, data.CardId);
			if(data.Zone.HasValue)
				entity.Tags[GameTag.ZONE] = (int)data.Zone;
			return entity;
		}

		public static PlayerEntity FromData(PlayerEntityData data)
		{
			return new PlayerEntity(data.Id, data.PlayerId);
		}

		public static GameEntity FromData(GameEntityData data)
		{
			return new GameEntity(data.Id);
		}

	}
}