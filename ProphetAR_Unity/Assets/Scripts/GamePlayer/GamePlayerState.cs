namespace ProphetAR
{
    public class GamePlayerState
    {
        public GamePlayerMultiTurnActions MultiTurnActions { get; }
        
        private readonly GamePlayer _player;
        
        public GamePlayerState(GamePlayer player)
        {
            _player = player;
            MultiTurnActions = new GamePlayerMultiTurnActions(_player);
        }
    }
}