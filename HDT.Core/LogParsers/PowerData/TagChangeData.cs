#region

using HearthDb.Enums;

#endregion

namespace HDT.Core.LogParsers.PowerData
{
	public class TagChangeData
	{
		public TagChangeData(GameTag tag, int value, bool creationTag, EntityData entity)
		{
			Entity = entity;
			Tag = tag;
			Value = value;
			CreationTag = creationTag;
		}

		public EntityData Entity { get; }
		public GameTag Tag { get; }
		public int Value { get; }
		public bool CreationTag { get; }
	}
}