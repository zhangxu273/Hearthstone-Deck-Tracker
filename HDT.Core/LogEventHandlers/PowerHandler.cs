#region

using HDT.Core.LogParsers;
using HDT.Core.LogParsers.PowerData;

#endregion

namespace HDT.Core.LogEventHandlers
{
	internal class PowerHandler
	{
		public PowerHandler(IPowerEvents powerEvents)
		{
			powerEvents.OnCreateGame += OnCreateGame;
			powerEvents.OnGameEntity += OnGameEntity;
			powerEvents.OnPlayerEntity += OnPlayerEntity;
			powerEvents.OnFullEntity += OnFullEntity;
			powerEvents.OnShowEntity += OnShowEntity;
			powerEvents.OnTagChange += OnTagChange;
			powerEvents.OnBlockEnd += OnBlockEnd;
			powerEvents.OnBlockStart += OnBlockStart;
			powerEvents.OnEndSpectator += OnEndSpectator;

		}

		private void OnCreateGame()
		{
		}

		private void OnGameEntity(EntityData data)
		{
		}

		private void OnPlayerEntity(EntityData data)
		{
		}

		private void OnFullEntity(EntityData data)
		{
		}

		private void OnShowEntity(EntityData data)
		{
		}

		private void OnTagChange(TagChangeData data)
		{
		}

		private void OnBlockEnd()
		{
		}

		private void OnBlockStart(BlockData data)
		{
		}

		private void OnEndSpectator()
		{
		}
	}
}