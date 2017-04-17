using HearthMirror;
using HearthMirror.Objects;
using HearthWatcher.Providers;

namespace HearthWatcher
{
	public class HearthMirrorArenaProvider : IArenaProvider
	{
		public ArenaInfo GetArenaInfo() => Reflection.GetArenaDeck();
		public Card[] GetDraftChoices() => Reflection.GetArenaDraftChoices()?.ToArray();
	}
}