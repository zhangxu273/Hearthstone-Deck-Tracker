#region

using System;
using System.Text.RegularExpressions;
using HDT.Core.LogParsers.PowerData;
using HearthDb.Enums;
using HearthWatcher.LogReader;
using static HDT.Core.LogParsers.PowerData.EntityType;

#endregion

namespace HDT.Core.LogParsers
{
	public class PowerParser : IPowerEvents
	{
		public static readonly Regex BlockStartRegex =
			new Regex(@".*BLOCK_START.*BlockType=(?<type>(\w+)).*id=(?<id>\d*).*(cardId=(?<cardId>(\w*))).*Target=(?<target>(.+))");

		public static readonly Regex CardIdRegex = new Regex(@"cardId=(?<cardId>(\w+))");

		public static readonly Regex FullEntityRegex =
			new Regex(@"FULL_ENTITY - Updating.*id=(?<id>(\d+)).*zone=(?<zone>(\w+)).*CardID=(?<cardId>(\w*))");

		public static readonly Regex CreationTagRegex = new Regex(@"tag=(?<tag>(\w+))\ value=(?<value>(\w+))");

		public static readonly Regex EntityRegex =
			new Regex(@"(?=id=(?<id>(\d+)))(?=name=(?<name>(\w+)))?(?=zone=(?<zone>(\w+)))?(?=zonePos=(?<zonePos>(\d+)))?(?=cardId=(?<cardId>(\w+)))?(?=player=(?<player>(\d+)))?(?=type=(?<type>(\w+)))?");

		public static readonly Regex GameEntityRegex = new Regex(@"GameEntity\ EntityID=(?<id>(\d+))");

		public static readonly Regex PlayerEntityRegex =
			new Regex(@"Player\ EntityID=(?<id>(\d+))\ PlayerID=(?<playerId>(\d+))\ GameAccountId=(?<gameAccountId>(.+))");

		public static readonly Regex TagChangeRegex =
			new Regex(@"TAG_CHANGE\ Entity=(?<entity>(.+))\ tag=(?<tag>(\w+))\ value=(?<value>(\w+))");

		public static readonly Regex ShowEntityRegex =
			new Regex(@"SHOW_ENTITY\ -\ Updating\ Entity=(?<entity>(.+))\ CardID=(?<cardId>(\w*))");

		public event Action<GameEntityData> OnGameEntity;
		public event Action<PlayerEntityData> OnPlayerEntity;
		public event Action<EntityData> OnFullEntity;
		public event Action<EntityData> OnShowEntity;
		public event Action OnCreateGame;
		public event Action<TagChangeData> OnTagChange;
		public event Action<BlockData> OnBlockStart;
		public event Action OnEndSpectator;
		public event Action OnBlockEnd;

		private EntityData ParseEntity(string entity)
		{
			int id;
			string name = null;
			entity = entity.Replace("UNKNOWN ENTITY ", "");
			var entityMatch = EntityRegex.Match(entity);
			if(entityMatch.Success)
				id = int.Parse(entityMatch.Groups["id"].Value);
			else if(!int.TryParse(entity, out id))
			{
				id = -1;
				name = entity;
			}
			return new EntityData(id, name, null, null);
		}

		public void Parse(LogLine line)
		{
			var match = GameEntityRegex.Match(line.LineContent);
			if(match.Success)
			{
				var id = int.Parse(match.Groups["id"].Value);
				OnGameEntity?.Invoke(new GameEntityData(id));
				return;
			}

			match = PlayerEntityRegex.Match(line.LineContent);
			if(match.Success)
			{
				var entityId = int.Parse(match.Groups["id"].Value);
				var playerId = int.Parse(match.Groups["playerId"].Value);
				OnPlayerEntity?.Invoke(new PlayerEntityData(entityId, playerId));
				return;
			}

			match = FullEntityRegex.Match(line.LineContent);
			if(match.Success)
			{
				var id = int.Parse(match.Groups["id"].Value);
				var cardId = match.Groups["cardId"].Value;
				var zone = GameTagParser.ParseEnum<Zone>(match.Groups["zone"].Value);
				OnFullEntity?.Invoke(new EntityData(id, null, cardId, zone));
				return;
			}

			match = TagChangeRegex.Match(line.LineContent);
			if(match.Success)
			{
				var entity = ParseEntity(match.Groups["entity"].Value);
				Enum.TryParse(match.Groups["tag"].Value, out GameTag tag);
				var value = GameTagParser.ParseTag(tag, match.Groups["value"].Value);
				var entityId = entity.Id == -1 ? null : (int?)entity.Id;
				OnTagChange?.Invoke(new TagChangeData(tag, value, false, entityId, entity.Name));
				return;
			}

			match = ShowEntityRegex.Match(line.LineContent);
			if(match.Success)
			{
				var cardId = match.Groups["cardId"].Value;
				var entity = ParseEntity(match.Groups["entity"].Value);
				OnShowEntity?.Invoke(new EntityData(entity.Id, entity.Name, cardId, null));
				return;
			}

			match = CreationTagRegex.Match(line.LineContent);
			if(match.Success && !line.LineContent.Contains("HIDE_ENTITY"))
			{
				var tag = GameTagParser.ParseEnum<GameTag>(match.Groups["tag"].Value);
				var value = GameTagParser.ParseTag(tag, match.Groups["value"].Value);
				OnTagChange?.Invoke(new TagChangeData(tag, value, true, null, null));
				return;
			}

			if(line.LineContent.Contains("End Spectator"))
			{
				OnEndSpectator?.Invoke();
				return;
			}

			match = BlockStartRegex.Match(line.LineContent);
			if(match.Success)
			{
				var type = match.Groups["type"].Value;
				var id = int.Parse(match.Groups["id"].Value);
				var cardId = match.Groups["cardId"].Value.Trim();
				OnBlockStart?.Invoke(new BlockData(type, id, cardId));
				return;
			}

			if(line.LineContent.Contains("CREATE_GAME"))
			{
				OnCreateGame?.Invoke();
				return;
			}

			if(line.LineContent.Contains("BLOCK_END"))
				OnBlockEnd?.Invoke();
		}
	}
}