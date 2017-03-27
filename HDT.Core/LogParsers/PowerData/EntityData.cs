#region

using HearthDb.Enums;

#endregion

namespace HDT.Core.LogParsers.PowerData
{
	public class EntityData
	{
		public EntityData(int id, string name, string cardId, Zone? zone, EntityType? type)
		{
			Id = id;
			Name = name;
			CardId = cardId;
			Zone = zone;
			Type = type;
		}

		public int Id { get; }
		public string Name { get; }
		public string CardId { get; }
		public Zone? Zone { get; }
		public EntityType? Type { get; }
	}
}