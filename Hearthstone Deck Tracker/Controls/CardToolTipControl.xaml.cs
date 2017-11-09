#region

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Hearthstone;

#endregion

namespace Hearthstone_Deck_Tracker.Controls
{
	public partial class CardToolTipControl : INotifyPropertyChanged
	{
		public CardToolTipControl()
		{
			InitializeComponent();
			DataContextChanged += (sender, e) => OnPropertyChanged(nameof(RaceOrType));
		}

		private Hearthstone.Card Card => (Hearthstone.Card) DataContext;

		public string RaceOrType => Card == null ? null : HearthDbConverter.RaceConverter(Card.Race)
														?? HearthDbConverter.CardTypeConverter(Card.Type);

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
