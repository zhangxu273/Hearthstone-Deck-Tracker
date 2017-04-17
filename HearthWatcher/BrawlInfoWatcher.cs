using System;
using HearthMirror;
using HearthMirror.Objects;

namespace HearthWatcher
{
	public class BrawlInfoWatcher : Watcher
	{
		public event Action<BrawlInfo> OnBrawlInfoChanged;

		public override void Update()
		{
			var brawlInfo = Reflection.GetBrawlInfo();
			if(brawlInfo != null)
			{
				OnBrawlInfoChanged?.Invoke(brawlInfo);
				Stop();
			}
		}
	}
}