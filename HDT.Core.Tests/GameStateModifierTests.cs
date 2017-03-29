using System;
using HDT.Core.Hearthstone;
using HDT.Core.Hearthstone.GameStateModifiers;
using HDT.Core.LogEventHandlers;
using HDT.Core.LogParsers.PowerData;
using HearthDb.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HDT.Core.Tests
{
	[TestClass]
	public class GameStateModifierTests
	{
		[TestMethod]
		public void TestFullEntity()
		{
			var gameState = new GameState(new MockGameEventSource());
			Assert.AreEqual(gameState.Entities.Count, 0);
			var entityData = new EntityData(5, null, "NEW1_034", Zone.PLAY);
			gameState.Apply(new FullEntity(entityData));
			Assert.AreEqual(gameState.Entities.Count, 1);
			var entityData2 = new EntityData(7, null, "NEW1_033", Zone.HAND);
			gameState.Apply(new FullEntity(entityData2));
			Assert.AreEqual(gameState.Entities.Count, 2);
			var entity = gameState.Entities[5];
			Assert.IsNotNull(entity);
			Assert.AreEqual(entity.Id, 5);
			Assert.AreEqual(entity.CardId, "NEW1_034");
			Assert.AreEqual(entity.Tags[GameTag.ZONE], (int)Zone.PLAY);
			var entity2 = gameState.Entities[7];
			Assert.IsNotNull(entity2);
			Assert.AreEqual(entity2.Id, 7);
			Assert.AreEqual(entity2.CardId, "NEW1_033");
			Assert.AreEqual(entity2.Tags[GameTag.ZONE], (int)Zone.HAND);
		}

		[TestMethod]
		public void TestShowEntity()
		{
			var gameState = new GameState(new MockGameEventSource());
			var entityData = new EntityData(5, null, null, null);
			gameState.Apply(new FullEntity(entityData));
			var entity = gameState.Entities[5];
			Assert.IsNull(entity.CardId);
			gameState.Apply(new ShowEntity(entityData.Id, "NEW1_034"));
			Assert.AreEqual(entity.CardId, "NEW1_034");
		}

		[TestMethod]
		public void TestTagChange()
		{
			var gameState = new GameState(new MockGameEventSource());
			var entityData = new EntityData(5, null, null, null);
			gameState.Apply(new FullEntity(entityData));
			var entity = gameState.Entities[5];
			Assert.IsFalse(entity.HasTag(GameTag.ZONE));
			gameState.Apply(new TagChange(5, GameTag.ZONE, (int)Zone.HAND));
			Assert.AreEqual(entity.GetTag(GameTag.ZONE), (int)Zone.HAND);
		}

		private class MockGameEventSource : IGameEventSource
		{
			public event Action<IGameStateModifier> OnGameStateChange;
		}
	}
}
