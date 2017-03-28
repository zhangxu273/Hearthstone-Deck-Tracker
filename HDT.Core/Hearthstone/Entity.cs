#region

using System.Collections.Generic;
using HDT.Core.LogParsers.PowerData;
using HearthDb.Enums;

#endregion

namespace HDT.Core.Hearthstone
{
	public class Entity
	{
		private Entity(int id, string cardId)
		{
			Id = id;
			CardId = cardId;
			Tags = new Dictionary<GameTag, int>();
		}

		public int Id { get; }
		public string CardId { get; set; }
		public Dictionary<GameTag, int> Tags { get; }

		public int GetTag(GameTag tag)
		{
			Tags.TryGetValue(tag, out var value);
			return value;
		}

		public bool HasTag(GameTag tag)
		{
			return GetTag(tag) > 0;
		}

		public static Entity FromData(EntityData data)
		{
			var entity = new Entity(data.Id, data.CardId);
			if(data.Zone.HasValue)
				entity.Tags[GameTag.ZONE] = (int)data.Zone;
			return entity;
		}
	}
}