#region

using HearthDb.Enums;

#endregion

namespace HDT.Core.LogParsers.PowerData
{
	public class TagChangeData
	{
		public TagChangeData(GameTag tag, int value, bool creationTag, int? entityId, string entityName)
		{
			Tag = tag;
			Value = value;
			CreationTag = creationTag;
			EntityId = entityId;
			EntityName = entityName;
		}

		public int? EntityId { get; }
		public string EntityName { get; }
		public GameTag Tag { get; }
		public int Value { get; }
		public bool CreationTag { get; }
	}
}