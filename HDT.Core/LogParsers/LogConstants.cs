#region

using System.Text.RegularExpressions;

#endregion

namespace HDT.Core.LogParsers
{
	public static class LogConstants
	{
		public static readonly Regex GoldProgressRegex = new Regex(@"(?<wins>(\d))/3 wins towards 10 gold");
		public static readonly Regex UnloadCardRegex = new Regex(@"unloading\ name=(?<id>(\w+_\w+))\ family=CardPrefab\ persistent=False");
		public static readonly Regex UnloadBrawlAsset = new Regex(@"unloading name=Tavern_Brawl\ ");

		public static readonly Regex CardMovementRegex =
			new Regex(@"\w*(cardId=(?<Id>(\w*))).*(zone\ from\ (?<from>((\w*)\s*)*))((\ )*->\ (?<to>(\w*\s*)*))*.*");

		public static readonly Regex NewChoiceRegex = new Regex(@"Client chooses: .* \((?<id>(.+))\)");
		public static readonly Regex GameModeRegex = new Regex(@"prevMode=(?<prev>(\w+)).*currMode=(?<curr>(\w+))");
		public static readonly Regex ConnectionRegex = new Regex(@"ConnectAPI\.GotoGameServer -- address=(?<address>(.*)), game=(?<game>(.*)), client=(?<client>(.*)), spectateKey=(?<spectateKey>(.*)),? reconn");
		public static readonly Regex LegendRankRegex = new Regex(@"legend rank (?<rank>(\d+))");
		public static readonly Regex BeginBlurRegex = new Regex(@"BeginEffect blur \d => 1");
	}
}