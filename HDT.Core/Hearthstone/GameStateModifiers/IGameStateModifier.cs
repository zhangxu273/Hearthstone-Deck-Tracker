namespace HDT.Core.Hearthstone.GameStateModifiers
{
	public interface IGameStateModifier
	{
		void Apply(GameState gameState);
	}
}