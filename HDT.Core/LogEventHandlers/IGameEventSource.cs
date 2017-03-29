using System;
using HDT.Core.Hearthstone.GameStateModifiers;

namespace HDT.Core.LogEventHandlers
{
	public interface IGameEventSource
	{
		event Action<IGameStateModifier> OnGameStateChange;
	}
}