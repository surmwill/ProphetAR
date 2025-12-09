namespace ProphetAR
{
    public class GamePlayer
    {
        public string Uid => Config.PlayerUid;

        public GamePlayerState State { get; } = new();
        
        public GamePlayerConfiguration Config { get; }
        
        public GamePlayer(GamePlayerConfiguration config)
        {
            Config = config;
            config.InitializePlayerState(State);
        }
    }
}