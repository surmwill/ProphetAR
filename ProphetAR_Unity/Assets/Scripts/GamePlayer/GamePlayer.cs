namespace ProphetAR
{
    public class GamePlayer
    {
        public string Uid => Config.PlayerUid;
        
        public int Index { get; }

        public GamePlayerState State { get; }
        
        public GamePlayerConfig Config { get; }

        public GameEventProcessor EventProcessor { get; } = new();
        
        public GamePlayer(int index, GamePlayerConfig config)
        {
            Index = index;
            Config = config;
            
            State = new GamePlayerState(this, config);
        }
    }
}