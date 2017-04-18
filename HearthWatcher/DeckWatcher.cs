using System;
using System.Collections.Generic;
using System.Linq;
using HearthMirror;
using HearthMirror.Objects;

namespace HearthWatcher
{
	public class DeckWatcher : Watcher
	{
		private long _selectedDeckId;
		private Dictionary<long, Deck> _decks;

		public event Action<Deck> OnSelectedDeckChanged;

		public DeckWatcher()
		{
			_decks = new Dictionary<long, Deck>();
			_selectedDeckId = 0;
		}

		public override void Update()
		{
			var deckId = Reflection.GetSelectedDeckInMenu();
			if(deckId != 0 && _selectedDeckId != deckId)
			{
				_selectedDeckId = deckId;
				if(!_decks.TryGetValue(deckId, out var deck))
				{
					_decks = Reflection.GetDecks().ToDictionary(x => x.Id);
					_decks.TryGetValue(deckId, out deck);
				}
				if(deck != null)
					OnSelectedDeckChanged?.Invoke(deck);
			}
		}
	}
}
