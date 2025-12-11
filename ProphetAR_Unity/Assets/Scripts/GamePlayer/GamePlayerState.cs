namespace ProphetAR
{
    public class GamePlayerState
    {
        public CustomPriorityQueue<MultiGameTurnAction> MultiTurnActions { get; } = new();
        
        private readonly GamePlayer _player;
        
        public GamePlayerState(GamePlayer player)
        {
            _player = player;
        }
    }
}