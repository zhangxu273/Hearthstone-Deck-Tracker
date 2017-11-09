#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Utility.Themes;

#endregion

namespace Hearthstone_Deck_Tracker.Hearthstone
{
	[Serializable]
	public class Card : ICloneable, INotifyPropertyChanged
	{
		[NonSerialized]
		private HearthDb.Card _dbCard;

		private readonly Regex _overloadRegex = new Regex(@"Overload:.+?\((?<value>(\d+))\)");
		private int _count = 1;
		private int _inHandCount;
		private bool _isCreated;
		private bool _loaded;
		private int? _overload;
		private bool _wasDiscarded;
		private string _id;

		[NonSerialized]
		private static readonly Dictionary<string, Dictionary<int, CardImageObject>> CardImageCache =
			new Dictionary<string, Dictionary<int, CardImageObject>>();

		public IEnumerable<string> AlternativeNames => _dbCard != null ? Config.Instance.AlternativeLanguages
				.Select(x => x != Config.Instance.SelectedLanguage && Enum.TryParse(x, out Locale lang) ? _dbCard.GetLocName(lang) : null) : new List<string>();

		public IEnumerable<string> AlternativeTexts => _dbCard != null ? Config.Instance.AlternativeLanguages
				.Select(x => x != Config.Instance.SelectedLanguage && Enum.TryParse(x, out Locale lang) ? _dbCard.GetLocText(lang) : null) : new List<string>();

		public string Id
		{
			get => _id;
			set
			{
				_id = value;
				if(_dbCard == null)
					Load();
			}
		}

		public int DbfIf => _dbCard?.DbfId ?? 0;

		public string[] Mechanics => _dbCard?.Mechanics ?? new string[0];

		public CardClass PlayerClass => _dbCard?.Class ?? CardClass.INVALID;

		public Rarity Rarity => _dbCard?.Rarity ?? Rarity.INVALID;

		public Card()
		{
			Count = 1;
		}

		public Card(string id, int count, int inHandCount = 0, HearthDb.Card dbCard = null)
		{
			_dbCard = dbCard;
			Id = id;
			InHandCount = inHandCount;
			Count = count;
		}

		private Locale SelectedLanguage
		{
			get
			{
				if(!Enum.TryParse(Config.Instance.SelectedLanguage, out Locale lang))
					lang = Locale.enUS;
				return lang;
			}
		}

		public Card(HearthDb.Card dbCard)
		{
			_dbCard = dbCard;
			Id = dbCard.Id;
			_loaded = true;
		}

		public int Count
		{
			get => _count;
			set
			{
				_count = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Background));
			}
		}

		[XmlIgnore]
		public bool Jousted { get; set; }

		public int Attack => _dbCard?.Attack ?? 0;

		public int Health => _dbCard?.Health ?? 0;

		public string Text => CleanText(_dbCard?.GetLocText(SelectedLanguage) ?? string.Empty);

		public string FormattedText => CleanText(_dbCard?.GetLocText(SelectedLanguage), false) ?? "";

		public string EnglishText => CleanText(_dbCard?.GetLocText(Locale.enUS) ?? string.Empty);

		public string AlternativeLanguageText => GetAlternativeText(false);

		public string FormattedAlternativeLanguageText => GetAlternativeText(true);

		private string GetAlternativeText(bool formatted)
		{
			var result = "";
			var names = AlternativeNames.ToList();
			var texts = AlternativeTexts.ToList();
			for(var i = 0; i < names.Count; ++i)
			{
				if(i > 0)
					result += "-\n";
				result += "[" + names[i] + "]\n";
				if(texts[i] != null)
					result += CleanText(texts[i], !formatted) + "\n";
			}
			return result.TrimEnd(' ', '\n');
		}

		public Visibility ShowAlternativeLanguageTextInTooltip => AlternativeNames.Any(x => x != null) ? Visibility.Visible : Visibility.Collapsed;

		public bool HasVisibleStats => Type != CardType.SPELL && Type != CardType.ENCHANTMENT && Type != CardType.HERO_POWER && !IsPlayableHeroCard;

		public Visibility ShowIconsInTooltip => HasVisibleStats ? Visibility.Visible : Visibility.Hidden;

		public Visibility ShowArmorIconInTooltip => IsPlayableHeroCard ? Visibility.Visible : Visibility.Hidden;

		public Visibility ShowHealthValueInTooltip => HasVisibleStats || IsPlayableHeroCard ? Visibility.Visible : Visibility.Hidden;

		public CardSet Set => _dbCard?.Set ?? CardSet.INVALID;

		public Race Race => _dbCard?.Race ?? Race.INVALID;

		public int Durability => _dbCard?.Durability ?? 0;

		public int ArmorDurabilityOrHealth => (IsPlayableHeroCard ? _dbCard?.Armor : Durability) ?? Health;

		public CardType Type => _dbCard?.Type ?? CardType.INVALID;

		public string Name => _dbCard?.GetLocName(Locale.enUS) ?? string.Empty;

		public int Cost => _dbCard?.Cost ?? 0;

		public bool IsPlayableHeroCard => Type == CardType.HERO && Set != CardSet.CORE && Set != CardSet.HERO_SKINS;

		public int Overload
		{
			get
			{
				if(_overload.HasValue)
					return _overload.Value;
				var overload = -1;
				if(!string.IsNullOrEmpty(EnglishText))
				{
					var match = _overloadRegex.Match(EnglishText);
					if(match.Success)
						int.TryParse(match.Groups["value"].Value, out overload);
					_overload = overload;
				}
				return overload;
			}
		}

		public int DustCost
		{
			get
			{
				switch(Rarity)
				{
					case Rarity.COMMON:
						return 40;
					case Rarity.RARE:
						return 100;
					case Rarity.EPIC:
						return 400;
					case Rarity.LEGENDARY:
						return 1600;
				}
				return 0;
			}
		}

		public string Artist => _dbCard?.ArtistName ?? string.Empty;

		public string LocalizedName => _dbCard?.GetLocName(SelectedLanguage) ?? Name;

		public string[] EntourageCardIds => _dbCard?.EntourageCardIds ?? new string[0];

		[XmlIgnore]
		public int InHandCount
		{
			get => _inHandCount;
			set
			{
				_inHandCount = value;
				OnPropertyChanged();
			}
		}

		public bool IsClassCard => PlayerClass >= CardClass.DRUID && PlayerClass <= CardClass.WARRIOR;

		[XmlIgnore]
		public bool IsCreated
		{
			get => _isCreated;
			set
			{
				_isCreated = value;
				OnPropertyChanged();
			}
		}

		[XmlIgnore]
		public bool WasDiscarded
		{
			get => _wasDiscarded;
			set
			{
				_wasDiscarded = value;
				OnPropertyChanged();
			}
		}

		public bool IsClass(CardClass playerClass)
		{
			if(PlayerClass == playerClass)
				return true;
			var group = _dbCard?.Entity.GetTag(GameTag.MULTI_CLASS_GROUP) ?? 0;
			return group != 0 && Helper.MultiClassGroups[(MultiClassGroup)group].Contains(playerClass);
		}

		public SolidColorBrush ColorPlayer
		{
			get
			{
				Color color;
				if(HighlightInHand && Config.Instance.HighlightCardsInHand)
					color = Colors.GreenYellow;
				else if(WasDiscarded && Config.Instance.HighlightDiscarded)
					color = Colors.IndianRed;
				else
					color = Colors.White;
				return new SolidColorBrush(color);
			}
		}

		public SolidColorBrush ColorOpponent => new SolidColorBrush(Colors.White);

		public string CardFileName => Name.ToLowerInvariant().Replace(' ', '-').Replace(":", "").Replace("'", "-").Replace(".", "").Replace("!", "").Replace(",", "");

		public FontFamily Font
		{
			get
			{
				var lang = Config.Instance.SelectedLanguage;
				var font = new FontFamily();
				// if the language uses a Latin script use Belwe font
				if(Helper.LatinLanguages.Contains(lang) || Config.Instance.NonLatinUseDefaultFont == false)
					font = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#Belwe Bd BT");
				return font;
			}
		}

		public DrawingBrush Background
		{
			get
			{
				if(Id == null || Name == null)
					return new DrawingBrush();
				var cardImageObj = new CardImageObject(this);
				if(CardImageCache.TryGetValue(Id, out Dictionary<int, CardImageObject> cache))
				{
					if(cache.TryGetValue(cardImageObj.GetHashCode(), out CardImageObject cached))
						return cached.Image;
				}
				try
				{
					var image = ThemeManager.GetBarImageBuilder(this).Build();
					if (image.CanFreeze)
						image.Freeze();
					cardImageObj = new CardImageObject(image, this);
					if(cache == null)
					{
						cache = new Dictionary<int, CardImageObject>();
						CardImageCache.Add(Id, cache);
					}
					cache.Add(cardImageObj.GetHashCode(), cardImageObj);
					return cardImageObj.Image;
				}
				catch(Exception ex)
				{
					Log.Error($"Image builder failed: {ex.Message}");
					return new DrawingBrush();
				}
			}
		}

		internal void Update() => OnPropertyChanged(nameof(Background));
		internal void UpdateHighlight() => OnPropertyChanged(nameof(Highlight));

		public ImageBrush Highlight => ThemeManager.CurrentTheme?.HighlightImage ?? new ImageBrush();

		[XmlIgnore]
		public bool HighlightInHand { get; set; }

		public string FlavorText => CleanText(_dbCard?.GetLocFlavorText(SelectedLanguage)) ?? "";
		
		public string FormattedFlavorText => CleanText(_dbCard?.GetLocFlavorText(SelectedLanguage), false) ?? "";

		public bool Collectible => _dbCard?.Collectible ?? false;

		public object Clone() => new Card(Id, Count, InHandCount, _dbCard);

		public override string ToString() => Name + "(" + Count + ")";

		public override bool Equals(object card)
		{
			if(!(card is Card c))
				return false;
			return c.Id == Id;
		}

		public bool EqualsWithCount(Card card) => card.Id == Id && card.Count == Count;

		public override int GetHashCode() => Name.GetHashCode();

		public void Load()
		{
			if(_loaded)
				return;

			_loaded = true;
			var stats = Database.GetCardFromId(Id);
			if(stats == null)
				return;
			_dbCard = stats._dbCard;
			OnPropertyChanged();
		}

		private static string CleanText(string text, bool replaceTags = true)
		{
			if (replaceTags)
				text = text?.Replace("<b>", "").Replace("</b>", "").Replace("<i>", "").Replace("</i>", "");
			return text?.Replace("$", "").Replace("#", "").Replace("\\n", "\n").Replace("[x]", "");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
