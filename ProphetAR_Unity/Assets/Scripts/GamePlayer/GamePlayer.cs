namespace ProphetAR
{
    public class GamePlayer
    {
        public string Uid => Config.PlayerUid;

        public GamePlayerState State { get; }
        
        public GamePlayerConfig Config { get; }

        public GameEventProcessor EventProcessor { get; } = new();
        
        public GamePlayer(GamePlayerConfig config)
        {
            State = new GamePlayerState(this);
            Config = config;
        }
    }
}