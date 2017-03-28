using HDT.Core.LogParsers;
using HDT.Core.LogParsers.PowerData;
using HearthDb.Enums;
using HearthWatcher.LogReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDT.Core.Tests
{
	[TestClass]
	public class PowerParserTests
	{

		[TestMethod]
		public void TestEntityCreation_GameEntity()
		{
			var parser = new PowerParser();
			EntityData entityData = null;
			parser.OnGameEntity += (data) => entityData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:05.2789766 PowerTaskList.DebugPrintPower() -         GameEntity EntityID=1"));
			Assert.IsNotNull(entityData);
			Assert.AreEqual(entityData.Id, 1);
			Assert.AreEqual(entityData.Type, EntityType.GameEntity);
			Assert.IsNull(entityData.Zone);
			Assert.IsNull(entityData.CardId);
			Assert.IsNull(entityData.Name);
		}

		[TestMethod]
		public void TestEntityCreation_PlayerEntity()
		{
			var parser = new PowerParser();
			EntityData entityData = null;
			parser.OnPlayerEntity += (data) => entityData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:05.2794779 PowerTaskList.DebugPrintPower() -         Player EntityID=2 PlayerID=1 GameAccountId=[hi=144115198130930503 lo=15856412]"));
			Assert.IsNotNull(entityData);
			Assert.AreEqual(entityData.Id, 2);
			Assert.AreEqual(entityData.Type, EntityType.PlayerEntity);
			Assert.IsNull(entityData.Zone);
			Assert.IsNull(entityData.CardId);
			Assert.IsNull(entityData.Name);
		}

		[TestMethod]
		public void TestEntityCreation_CardEntity_Revealed()
		{
			var parser = new PowerParser();
			EntityData entityData = null;
			parser.OnFullEntity += (data) => entityData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:05.2930114 PowerTaskList.DebugPrintPower() -     FULL_ENTITY - Updating [name=Garrosh Hellscream id=64 zone=PLAY zonePos=0 cardId=HERO_01 player=1] CardID=HERO_01"));
			Assert.IsNotNull(entityData);
			Assert.AreEqual(entityData.Id, 64);
			Assert.AreEqual(entityData.Type, EntityType.CardEntity);
			Assert.IsTrue(entityData.Zone.HasValue);
			Assert.AreEqual(entityData.Zone.Value, Zone.PLAY);
			Assert.AreEqual(entityData.CardId, "HERO_01");
			Assert.IsNull(entityData.Name);
		}

		[TestMethod]
		public void TestEntityCreation_CardEntity_Hidden()
		{
			var parser = new PowerParser();
			EntityData entityData = null;
			parser.OnFullEntity += (data) => entityData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:05.2915068 PowerTaskList.DebugPrintPower() -     FULL_ENTITY - Updating [name=UNKNOWN ENTITY [cardType=INVALID] id=33 zone=DECK zonePos=0 cardId= player=1] CardID="));
			Assert.IsNotNull(entityData);
			Assert.AreEqual(entityData.Id, 33);
			Assert.AreEqual(entityData.Type, EntityType.CardEntity);
			Assert.IsTrue(entityData.Zone.HasValue);
			Assert.AreEqual(entityData.Zone.Value, Zone.DECK);
			Assert.AreEqual(entityData.CardId, string.Empty);
			Assert.IsNull(entityData.Name);
		}

		[TestMethod]
		public void TestTagChange_CreationTag()
		{
			var parser = new PowerParser();
			TagChangeData tagChangeData = null;
			parser.OnTagChange += (data) => tagChangeData = data;
			parser.Parse(new LogLine("Power", "D 11:06:06.4768941 PowerTaskList.DebugPrintPower() -         tag=HEALTH value=4"));
			Assert.IsNotNull(tagChangeData);
			Assert.IsNull(tagChangeData.EntityId);
			Assert.IsNull(tagChangeData.EntityName);
			Assert.IsTrue(tagChangeData.CreationTag);
			Assert.AreEqual(tagChangeData.Tag, GameTag.HEALTH);
			Assert.AreEqual(tagChangeData.Value, 4);
		}


		[TestMethod]
		public void TestTagChange_NamedEntity()
		{
			var parser = new PowerParser();
			TagChangeData tagChangeData = null;
			parser.OnTagChange += (data) => tagChangeData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:35.0269603 PowerTaskList.DebugPrintPower() -     TAG_CHANGE Entity=The Innkeeper tag=MULLIGAN_STATE value=DONE"));
			Assert.IsNotNull(tagChangeData);
			Assert.IsNull(tagChangeData.EntityId);
			Assert.AreEqual(tagChangeData.EntityName, "The Innkeeper");
			Assert.IsFalse(tagChangeData.CreationTag);
			Assert.AreEqual(tagChangeData.Tag, GameTag.MULLIGAN_STATE);
			Assert.AreEqual(tagChangeData.Value, (int)Mulligan.DONE);
		}

		[TestMethod]
		public void TestTagChange_UnknownEntity()
		{
			var parser = new PowerParser();
			TagChangeData tagChangeData = null;
			parser.OnTagChange += (data) => tagChangeData = data;
			parser.Parse(new LogLine("Power",
				"D 11:06:35.0274603 PowerTaskList.DebugPrintPower() -     TAG_CHANGE Entity=[name=UNKNOWN ENTITY [cardType=INVALID] id=15 zone=DECK zonePos=0 cardId= player=1] tag=ZONE_POSITION value=1"));
			Assert.IsNotNull(tagChangeData);
			Assert.AreEqual(tagChangeData.EntityId, 15);
			Assert.IsNull(tagChangeData.EntityName);
			Assert.IsFalse(tagChangeData.CreationTag);
			Assert.AreEqual(tagChangeData.Tag, GameTag.ZONE_POSITION);
			Assert.AreEqual(tagChangeData.Value, 1);
		}

		[TestMethod]
		public void TestTagChange_NumericEntity()
		{
			// This does currently not happen in PowerTaskList
			var parser = new PowerParser();
			TagChangeData tagChangeData = null;
			parser.OnTagChange += (data) => tagChangeData = data;
			parser.Parse(new LogLine("Power", "D 11:06:05.2403474 GameState.DebugPrintPower() - TAG_CHANGE Entity=1 tag=STATE value=RUNNING"));
			Assert.IsNotNull(tagChangeData);
			Assert.IsNull(tagChangeData.EntityName);
			Assert.AreEqual(tagChangeData.EntityId, 1);
			Assert.IsFalse(tagChangeData.CreationTag);
			Assert.AreEqual(tagChangeData.Tag, GameTag.STATE);
			Assert.AreEqual(tagChangeData.Value, (int)State.RUNNING);
		}

		[TestMethod]
		public void TestShowEntity()
		{
			var parser = new PowerParser();
			EntityData entityData = null;
			parser.OnShowEntity += (data) => entityData = data;
			parser.Parse(new LogLine("Power", "D 11:06:06.4768941 PowerTaskList.DebugPrintPower() -     SHOW_ENTITY - Updating Entity=[name=UNKNOWN ENTITY [cardType=INVALID] id=24 zone=DECK zonePos=0 cardId= player=1] CardID=EX1_604"));
			Assert.IsNotNull(entityData);
			Assert.IsNull(entityData.Name);
			Assert.AreEqual(entityData.Id, 24);
			Assert.AreEqual(entityData.CardId, "EX1_604");
		}

		[TestMethod]
		public void TestCreateGame()
		{
			var parser = new PowerParser();
			var success = false;
			parser.OnCreateGame += () => success = true;
			parser.Parse(new LogLine("Power", "D 11:06:05.2789766 PowerTaskList.DebugPrintPower() -     CREATE_GAME"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TestEndSpectator()
		{
			var parser = new PowerParser();
			var success = false;
			parser.OnEndSpectator += () => success = true;
			parser.Parse(new LogLine("Power", "D 11:06:06.4778733 ... End Spectator ..."));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TestBlockEnd()
		{
			var parser = new PowerParser();
			var success = false;
			parser.OnBlockEnd += () => success = true;
			parser.Parse(new LogLine("Power", "D 11:06:06.4778733 PowerTaskList.DebugPrintPower() - BLOCK_END"));
			Assert.IsTrue(success);
		}

		[TestMethod]
		public void TestBlockStart()
		{
			var parser = new PowerParser();
			BlockData blockData = null;
			parser.OnBlockStart += (data) => blockData = data;
			parser.Parse(new LogLine("Power", "D 11:06:49.8757851 PowerTaskList.DebugPrintPower() - BLOCK_START BlockType=PLAY Entity=[name=Sir Finley Mrrgglton id=7 zone=HAND zonePos=6 cardId=LOE_076 player=1] EffectCardId= EffectIndex=0 Target=0"));
			Assert.IsNotNull(blockData);
			Assert.AreEqual(blockData.Type, "PLAY");
			Assert.AreEqual(blockData.Id, 7);
			Assert.AreEqual(blockData.CardId, "LOE_076");
		}
	}
}
