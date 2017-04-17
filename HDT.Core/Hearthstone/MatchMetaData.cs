using System;
using HearthMirror.Objects;

namespace HDT.Core.Hearthstone
{
	public class MatchMetaData : GameMetaData
	{
		public GameServerInfo ServerInfo { get; set; }
		public MatchInfo MatchInfo { get; set; }
		public int Build { get; set; }
		public DateTime StartTime { get; set; }

		public MatchMetaData(GameMetaData gameMetaData)
		{
			Deck = gameMetaData.Deck;
			ArenaInfo = gameMetaData.ArenaInfo;
			BrawlInfo = gameMetaData.BrawlInfo;
		}
	}

	public class GameMetaData
	{
		public Deck Deck { get; set; }
		public ArenaInfo ArenaInfo { get; set; }
		public BrawlInfo BrawlInfo { get; set; }
	}
}