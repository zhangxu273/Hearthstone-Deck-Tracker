using System;
using System.Collections.Generic;

namespace HDT.Core.LogEventHandlers
{
	internal interface IPlayerIdLookupProvider
	{
		Dictionary<string, int> Players { get; }
		event Action OnUpdated;
	}
}