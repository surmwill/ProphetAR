namespace ProphetAR
{
    public class GamePlayer
    {
        public string Uid => Config.PlayerUid;

        public GamePlayerState State { get; } = new();
        
        public GamePlayerConfig Config { get; }
        
        public GamePlayer(GamePlayerConfig config)
        {
            Config = config;
            config.InitializePlayerState(State);
        }
    }
}