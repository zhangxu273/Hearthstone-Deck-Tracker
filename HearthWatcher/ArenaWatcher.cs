using System;
using System.Linq;
using HearthMirror.Objects;
using HearthWatcher.EventArgs;
using HearthWatcher.Providers;

namespace HearthWatcher
{
	public class ArenaWatcher : Watcher
	{
		public delegate void ChoicesChangedEventHandler(object sender, ChoicesChangedEventArgs args);
		public delegate void CardPickedEventHandler(object sender, CardPickedEventArgs args);
		public delegate void CompleteDeckEventHandler(object sender, CompleteDeckEventArgs args);
		public delegate void RewardsEventHandler(object sender, RewardsEventArgs args);

		private int _prevSlot = -1;
		private bool _sameChoices;
		private Card[] _prevChoices;
		private ArenaInfo _prevInfo;
		private const int MaxDeckSize = 30;
		private readonly IArenaProvider _arenaProvider;

		public ArenaWatcher(IArenaProvider arenaProvider, int delay = 500) : base(delay)
		{
			if(arenaProvider == null)
				throw new ArgumentNullException(nameof(arenaProvider));
			_arenaProvider = arenaProvider;
		}

		public event ChoicesChangedEventHandler OnChoicesChanged;
		public event CardPickedEventHandler OnCardPicked;
		public event CompleteDeckEventHandler OnCompleteDeck;
		public event RewardsEventHandler OnRewards;

		protected override void Reset()
		{
			_prevSlot = -1;
			_prevInfo = null;
		}

		public override void Update()
		{
			var arenaInfo = _arenaProvider.GetArenaInfo();
			if(arenaInfo == null)
				return;
			var numCards = arenaInfo.Deck.Cards.Sum(x => x.Count);
			if(numCards == MaxDeckSize)
			{
				if(_prevSlot == MaxDeckSize)
					CardPicked(arenaInfo);
				OnCompleteDeck?.Invoke(this, new CompleteDeckEventArgs(arenaInfo));
				if(arenaInfo.Rewards?.Any() ?? false)
					OnRewards?.Invoke(this, new RewardsEventArgs(arenaInfo));
				Stop();
				return;
			}
			if(HasChanged(arenaInfo, arenaInfo.CurrentSlot))
			{
				var choices = _arenaProvider.GetDraftChoices();
				if(choices == null || choices.Length == 0)
					return;
				if(arenaInfo.CurrentSlot > _prevSlot)
				{
					if(ChoicesChanged(choices) || _sameChoices)
					{
						_sameChoices = false;
						OnChoicesChanged?.Invoke(this, new ChoicesChangedEventArgs(choices, arenaInfo.Deck));
					}
					else
					{
						_sameChoices = true;
						return;
					}
				}
				if(_prevSlot == 0 && arenaInfo.CurrentSlot == 1)
					HeroPicked(arenaInfo);
				else if(_prevSlot > 0 && arenaInfo.CurrentSlot > _prevSlot)
					CardPicked(arenaInfo);
				_prevSlot = arenaInfo.CurrentSlot;
				_prevInfo = arenaInfo;
				_prevChoices = choices;
			}
		}

		private bool ChoicesChanged(Card[] choices) => _prevChoices == null || choices[0] != _prevChoices[0] || choices[1] != _prevChoices[1] || choices[2] != _prevChoices[2];

		private bool HasChanged(ArenaInfo arenaInfo, int slot) 
			=> _prevInfo == null || _prevInfo.Deck.Hero != arenaInfo.Deck.Hero ||  slot > _prevSlot;

		private void HeroPicked(ArenaInfo arenaInfo)
		{
			var hero = _prevChoices.FirstOrDefault(x => x.Id == arenaInfo.Deck.Hero);
			if(hero != null)
				OnCardPicked?.Invoke(this, new CardPickedEventArgs(hero, _prevChoices));
		}

		private void CardPicked(ArenaInfo arenaInfo)
		{
			var pick = arenaInfo.Deck.Cards.FirstOrDefault(x => !_prevInfo?.Deck.Cards.Any(c => x.Id == c.Id && x.Count == c.Count) ?? false);
			if(pick != null)
				OnCardPicked?.Invoke(this, new CardPickedEventArgs(new Card(pick.Id, 1, pick.Premium), _prevChoices));
		}
	}
}
