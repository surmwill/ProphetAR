
namespace ProphetAR
{
    public class GamePlayerConfig
    {
        public string PlayerUid { get; }
        
        public bool IsAI { get; }
        
        public GamePlayerConfig(string playerUid, bool isAI)
        {
            PlayerUid = playerUid;
            IsAI = isAI;
        }

        // Set any initial player state variables
        public void InitializePlayerState(GamePlayerState playerState)
        {
            
        }
    }
}