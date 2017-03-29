using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HearthMirror.Objects;
using HearthWatcher.EventArgs;
using HearthWatcher.Providers;

namespace HearthWatcher
{
	public class PackOpeningWatcher : Watcher
	{
		public delegate void PackEventHandler(object sender, PackEventArgs args);

		private readonly List<Card> _previousPack = new List<Card>();
		private bool _invokeEvent;
		private readonly IPackProvider _packProvider;

		public PackOpeningWatcher(IPackProvider packProvider, int delay = 500) : base(delay)
		{
			if(packProvider == null)
				throw new ArgumentNullException(nameof(packProvider));
			_packProvider = packProvider;
		}
		public event PackEventHandler NewPackEventHandler;

		public override void Update()
		{
			var cards = _packProvider.GetCards();
			if(cards?.Count == 5)
			{
				if(cards.All(x => _previousPack.Any(c => c.Id == x.Id & c.Premium == x.Premium)))
					return;
				if(_previousPack.Any())
					_invokeEvent = true;
				_previousPack.Clear();
				_previousPack.AddRange(cards);
				if(_invokeEvent)
					NewPackEventHandler?.Invoke(this, new PackEventArgs(cards, _packProvider.GetPackId()));
			}
			else
				_invokeEvent = true;
		}
	}
}
