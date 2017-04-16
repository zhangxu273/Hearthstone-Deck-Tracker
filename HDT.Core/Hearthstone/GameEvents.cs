using System;
using HDT.Core.Hearthstone.Entities;
using HDT.Core.Hearthstone.GameStateModifiers;
using HearthDb.Enums;

namespace HDT.Core.Hearthstone
{
	public static class GameEvents
	{
		public static Action<IGameStateModifier, GameState> OnGameEnd(Action action)
		{
			return OnTagChange(GameTag.STATE, (int)State.COMPLETE, entity => action());
		}

		private static Action<IGameStateModifier, GameState> OnTagChange(GameTag tag, int value, Action<Entity> action)
		{
			return OnTagChange(tag, null, value, action);
		}

		private static Action<IGameStateModifier, GameState> OnTagChange(GameTag tag, int? prev, int value, Action<Entity> action)
		{
			return (modifier, state) =>
			{
				if(!(modifier is TagChange tagChange))
					return;
				if(tagChange.Tag == tag && tagChange.Value == value && (!prev.HasValue || prev.Value == tagChange.PreviousValue))
					action.Invoke(state.Entities[tagChange.EntityId]);
			};
		}

		public static Action<IGameStateModifier, GameState> OnCardPlayed(Action<Entity> action)
		{
			return OnTagChange(GameTag.ZONE, (int)Zone.HAND, (int)Zone.PLAY, action);
		}

		public static Action<IGameStateModifier, GameState> OnCardDrawn(Action<Entity> action)
		{
			return OnTagChange(GameTag.ZONE, (int)Zone.DECK, (int)Zone.HAND, action);
		}
	}
}