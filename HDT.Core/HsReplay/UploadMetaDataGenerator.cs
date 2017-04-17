#region

using System.Linq;
using HDT.Core.Hearthstone;
using HDT.Core.Utility;
using HearthDb.Enums;
using HearthMirror.Objects;
using HSReplay;

#endregion

namespace HDT.Core.HsReplay
{
	public static class UploadMetaDataGenerator
	{
		public static UploadMetaData Generate(MatchMetaData matchMetaData)
		{
			var metaData = new UploadMetaData();
			var playerInfo = GetPlayerInfo(matchMetaData);
			if(playerInfo != null)
			{
				metaData.Player1 = playerInfo.Player1;
				metaData.Player2 = playerInfo.Player2;
			}

			metaData.ServerIp = DefaultNull(matchMetaData.ServerInfo.Address);
			metaData.ServerPort = DefaultNull(matchMetaData.ServerInfo.Port)?.ToString();
			metaData.GameHandle = DefaultNull(matchMetaData.ServerInfo.GameHandle)?.ToString();
			metaData.ClientHandle = DefaultNull(matchMetaData.ServerInfo.ClientHandle)?.ToString();
			metaData.SpectatePassword = DefaultNull(matchMetaData.ServerInfo.SpectatorPassword);
			metaData.AuroraPassword = DefaultNull(matchMetaData.ServerInfo.AuroraPassword);
			metaData.ServerVersion = DefaultNull(matchMetaData.ServerInfo.Version);
			metaData.ScenarioId = DefaultNull(matchMetaData.ServerInfo.Mission);
			metaData.Resumable = matchMetaData.ServerInfo.Resumable;

			metaData.SpectatorMode = matchMetaData.MatchInfo.Spectator;
			metaData.FriendlyPlayerId = matchMetaData.MatchInfo.LocalPlayer.Id;
			metaData.BrawlSeason = DefaultNull(matchMetaData.MatchInfo.BrawlSeasonId);
			metaData.LadderSeason = DefaultNull(matchMetaData.MatchInfo.RankedSeasonId);
			metaData.Format = matchMetaData.MatchInfo.FormatType;
			metaData.GameType =
				(int)GameTypeConverter.GetBnetGameType((GameType)matchMetaData.MatchInfo.GameType, (FormatType)matchMetaData.MatchInfo.FormatType);

			metaData.MatchStart = matchMetaData.StartTime.ToString("o");
			metaData.HearthstoneBuild = matchMetaData.Build;

			return metaData;
		}

		public static int GetRank(MatchInfo.Player player, bool wild)
		{
			if(wild)
				return player.WildRank > 0 ? player.WildRank : player.WildLegendRank;
			return player.StandardRank > 0 ? player.StandardRank : player.StandardLegendRank;
		}

		private static PlayerInfo GetPlayerInfo(MatchMetaData matchMetaData)
		{
			var friendly = new UploadMetaData.Player();
			var opposing = new UploadMetaData.Player();


			switch(matchMetaData.MatchInfo.GameType)
			{
				case (int)GameType.GT_RANKED:
					if(matchMetaData.MatchInfo.FormatType == (int)FormatType.FT_STANDARD)
					{
						friendly.Rank = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.StandardRank);
						opposing.Rank = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.StandardRank);
						friendly.LegendRank = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.StandardLegendRank);
						opposing.LegendRank = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.StandardLegendRank);
						friendly.Stars = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.StandardStars);
						opposing.Stars = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.StandardStars);
					}
					else if(matchMetaData.MatchInfo.FormatType == (int)FormatType.FT_WILD)
					{
						friendly.Rank = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.WildRank);
						opposing.Rank = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.WildRank);
						friendly.LegendRank = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.WildLegendRank);
						opposing.LegendRank = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.WildLegendRank);
						friendly.Stars = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.WildStars);
						opposing.Stars = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.WildStars);
					}
					break;
				case (int)GameType.GT_ARENA:
					friendly.Wins = matchMetaData.ArenaInfo?.Wins;
					friendly.Losses = matchMetaData.ArenaInfo?.Losses;
					break;
				case (int)GameType.GT_TAVERNBRAWL:
				case (int)GameType.GT_TB_1P_VS_AI:
				case (int)GameType.GT_TB_2P_COOP:
					friendly.Wins = matchMetaData.BrawlInfo?.Wins;
					friendly.Losses = matchMetaData.BrawlInfo?.Losses;
					break;
			}

			friendly.Cardback = DefaultNull(matchMetaData.MatchInfo.LocalPlayer.CardBackId);
			opposing.Cardback = DefaultNull(matchMetaData.MatchInfo.OpposingPlayer.CardBackId);

			if(matchMetaData.Deck != null)
			{
				friendly.DeckId = matchMetaData.Deck.Id;
				friendly.DeckList = matchMetaData.Deck.Cards.SelectMany(c => Enumerable.Repeat(c.Id, c.Count)).ToArray();
			}

			var localPlayerId = matchMetaData.MatchInfo.LocalPlayer.Id;
			return new PlayerInfo(localPlayerId == 1 ? friendly : opposing,
				localPlayerId == 2 ? friendly : opposing);
		}

		private static int? DefaultNull(int i) => i == 0 ? null : (int?)i;

		private static long? DefaultNull(long i) => i == 0 ? null : (long?)i;

		private static string DefaultNull(string s) => string.IsNullOrEmpty(s) ? null : s;
	}

	public class PlayerInfo
	{
		public PlayerInfo(UploadMetaData.Player player1, UploadMetaData.Player player2, int friendlyPlayerId = -1)
		{
			Player1 = player1;
			Player2 = player2;
			FriendlyPlayerId = friendlyPlayerId;
		}

		public UploadMetaData.Player Player1 { get; }
		public UploadMetaData.Player Player2 { get; }
		public int FriendlyPlayerId { get; }
	}
}