namespace ProphetAR
{
    public class GamePlayerState
    {
        private readonly GamePlayer _player;
        private readonly GamePlayerMultiTurnActions _multiTurnActions;
        
        public GamePlayerState(GamePlayer player)
        {
            _player = player;
            _multiTurnActions = new GamePlayerMultiTurnActions(_player);
        }


    }
}