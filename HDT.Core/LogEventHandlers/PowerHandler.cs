#region

using System;
using System.Collections.Generic;
using HDT.Core.Hearthstone;
using HDT.Core.Hearthstone.GameStateModifiers;
using HDT.Core.LogParsers;
using HDT.Core.LogParsers.PowerData;

#endregion

namespace HDT.Core.LogEventHandlers
{
	internal class PowerHandlerState
	{
		public PowerHandlerState()
		{
			EntityNames = new Dictionary<string, int>();
		}

		public int CurrentEntity { get; set; }
		public Dictionary<string, int> EntityNames { get; }
	}

	internal class PowerHandler
	{
		private PowerHandlerState _state;

		public PowerHandler(IPowerEvents powerEvents)
		{
			powerEvents.OnCreateGame += PowerEvents_OnCreateGame;
			powerEvents.OnGameEntity += PowerEvents_OnGameEntity;
			powerEvents.OnPlayerEntity += PowerEvents_OnPlayerEntity;
			powerEvents.OnFullEntity += PowerEvents_OnFullEntity;
			powerEvents.OnShowEntity += PowerEvents_OnShowEntity;
			powerEvents.OnTagChange += PowerEvents_OnTagChange;
			powerEvents.OnBlockEnd += PowerEvents_OnBlockEnd;
			powerEvents.OnBlockStart += PowerEvents_OnBlockStart;
			powerEvents.OnEndSpectator += PowerEvents_OnEndSpectator;
		}

		public event Action OnCreateGame;
		public event Action<IGameStateModifier> OnGameStateChange;

		private void PowerEvents_OnCreateGame()
		{
			_state = new PowerHandlerState();
			OnCreateGame?.Invoke();
		}

		private void PowerEvents_OnGameEntity(EntityData data)
		{
			_state.CurrentEntity = data.Id;
			_state.EntityNames["GameEntity"] = data.Id;
			OnGameStateChange?.Invoke(new FullEntity(data));
		}

		private void PowerEvents_OnPlayerEntity(EntityData data)
		{
			_state.CurrentEntity = data.Id;
			OnGameStateChange?.Invoke(new FullEntity(data));
		}

		private void PowerEvents_OnFullEntity(EntityData data)
		{
			_state.CurrentEntity = data.Id;
			OnGameStateChange?.Invoke(new FullEntity(data));
		}

		private void PowerEvents_OnShowEntity(EntityData data)
		{
			_state.CurrentEntity = data.Id;
			OnGameStateChange?.Invoke(new ShowEntity(data.Id, data.CardId));
		}

		private void PowerEvents_OnTagChange(TagChangeData data)
		{
			var entityId = data.EntityId ?? _state.CurrentEntity;
			OnGameStateChange?.Invoke(new TagChange(entityId, data.Tag, data.Value));
		}

		private void PowerEvents_OnBlockEnd()
		{
		}

		private void PowerEvents_OnBlockStart(BlockData data)
		{
		}

		private void PowerEvents_OnEndSpectator()
		{
		}
	}
}