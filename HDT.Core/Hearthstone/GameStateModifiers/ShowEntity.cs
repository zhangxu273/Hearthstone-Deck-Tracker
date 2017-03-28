namespace HDT.Core.Hearthstone.GameStateModifiers
{
	public class ShowEntity : IGameStateModifier
	{
		private readonly int _entityId;
		private readonly string _cardId;

		public ShowEntity(int entityId, string cardId)
		{
			_entityId = entityId;
			_cardId = cardId;
		}

		public void Apply(GameState gameState)
		{
			gameState.Entities[_entityId].CardId = _cardId;
		}
	}
}