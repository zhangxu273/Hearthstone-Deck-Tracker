#region

using System.Windows.Media;
using Hearthstone_Deck_Tracker.Utility.Themes;

#endregion

namespace Hearthstone_Deck_Tracker.Hearthstone
{
	internal class CardImageObject
	{
		public DrawingBrush Image { get; }
		public int Count { get; }
		public bool Jousted { get; }
		public bool ColoredFrame { get; }
		public bool ColoredGem { get; }
		public bool Created { get; }
		public string Theme { get; }
		public int TextColorHash { get; }
		public string Language { get; }

		public CardImageObject(DrawingBrush image, Card card) : this(card)
		{
			Image = image;
		}

		public CardImageObject(Card card)
		{
			Count = card.Count;
			Jousted = card.Jousted;
			ColoredFrame = Config.Instance.RarityCardFrames;
			ColoredGem = Config.Instance.RarityCardGems;
			Theme = ThemeManager.CurrentTheme?.Name;
			TextColorHash = card.ColorPlayer.Color.GetHashCode();
			Created = card.IsCreated;
			Language = Config.Instance.SelectedLanguage;
		}

		public override bool Equals(object obj)
		{
			var cardObj = obj as CardImageObject;
			return cardObj != null && Equals(cardObj);
		}

		protected bool Equals(CardImageObject other)
			=> Count == other.Count && Jousted == other.Jousted && ColoredFrame == other.ColoredFrame && ColoredGem == other.ColoredGem
				&& string.Equals(Theme, other.Theme) && TextColorHash == other.TextColorHash && Created == other.Created
				&& string.Equals(Language, other.Language);

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Count;
				hashCode = (hashCode * 397) ^ Jousted.GetHashCode();
				hashCode = (hashCode * 397) ^ ColoredFrame.GetHashCode();
				hashCode = (hashCode * 397) ^ ColoredGem.GetHashCode();
				hashCode = (hashCode * 397) ^ (Theme?.GetHashCode() ?? 0);
				hashCode = (hashCode * 397) ^ TextColorHash;
				hashCode = (hashCode * 397) ^ Created.GetHashCode();
				hashCode = (hashCode * 397) ^ Language.GetHashCode();
				return hashCode;
			}
		}
	}
}
