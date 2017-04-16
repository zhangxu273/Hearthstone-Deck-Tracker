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
			PendingTagChanges = new List<TagChangeData>();
			PlayerEntityIds = new Dictionary<int, int>();
		}

		public int CurrentEntity { get; set; }
		public List<TagChangeData> PendingTagChanges { get; }
		public Dictionary<int, int> PlayerEntityIds { get; }
		public int GameEntityId { get; set; }
	}

	internal class PowerHandler : IGameEventSource
	{
		private readonly IPlayerIdLookupProvider _playerIdLookup;
		private PowerHandlerState _state;

		public PowerHandler(IPowerEvents powerEvents, IPlayerIdLookupProvider playerIdLookup)
		{
			_playerIdLookup = playerIdLookup;
			_playerIdLookup.OnUpdated += PlayerIdLookupOnUpdated;
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

		private void PlayerIdLookupOnUpdated()
		{
			if(_state == null)
				return;
			for (var i = _state.PendingTagChanges.Count - 1; i > 0; i--)
			{
				var tagChange = _state.PendingTagChanges[i];
				if (!TryGetEntityId(tagChange.EntityName, out var playerId))
					continue;
				if(!_state.PlayerEntityIds.TryGetValue(playerId, out var entityId))
					continue;
				_state.PendingTagChanges.RemoveAt(i);
				OnGameStateChange?.Invoke(new TagChange(entityId, tagChange.Tag, tagChange.Value));
			}
		}

		public event Action OnCreateGame;
		public event Action<IGameStateModifier> OnGameStateChange;

		private void PowerEvents_OnCreateGame()
		{
			_state = new PowerHandlerState();
			OnCreateGame?.Invoke();
		}

		private void PowerEvents_OnGameEntity(GameEntityData data)
		{
			_state.CurrentEntity = data.Id;
			_state.GameEntityId = data.Id;
			OnGameStateChange?.Invoke(new FullEntity(data));
		}

		private void PowerEvents_OnPlayerEntity(PlayerEntityData data)
		{
			_state.CurrentEntity = data.Id;
			_state.PlayerEntityIds[data.PlayerId] = data.Id;
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
			if(!data.EntityId.HasValue && data.EntityName != null)
			{
				if(!TryGetEntityId(data.EntityName, out entityId))
				{
					_state.PendingTagChanges.Add(data);
					return;
				}
			}
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

		private bool TryGetEntityId(string name, out int entityId)
		{
			if(name == "GameEntity")
			{
				entityId = _state.GameEntityId;
				return true;
			}
			if(_playerIdLookup.Players.TryGetValue(name, out var playerId))
				return _state.PlayerEntityIds.TryGetValue(playerId, out entityId);
			entityId = -1;
			return false;
		}

	}
}