using System;
using HDT.Core.LogParsers.PowerData;

namespace HDT.Core.LogParsers
{
	internal interface IPowerEvents
	{
		event Action<GameEntityData> OnGameEntity;
		event Action<PlayerEntityData> OnPlayerEntity;
		event Action<EntityData> OnFullEntity;
		event Action<EntityData> OnShowEntity;
		event Action OnCreateGame;
		event Action<TagChangeData> OnTagChange;
		event Action<BlockData> OnBlockStart;
		event Action OnEndSpectator;
		event Action OnBlockEnd;
	}
}