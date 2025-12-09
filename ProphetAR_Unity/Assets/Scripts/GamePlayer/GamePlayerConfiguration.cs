
namespace ProphetAR
{
    public class GamePlayerConfiguration
    {
        public string PlayerUid { get; }
        
        public bool IsAI { get; }
        
        public GamePlayerConfiguration(string playerUid, bool isAI)
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