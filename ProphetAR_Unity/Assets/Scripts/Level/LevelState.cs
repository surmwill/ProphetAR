namespace ProphetAR
{
    public class LevelState
    {
        public Level Level { get; }
        
        
        
        public LevelState(Level level, LevelConfig initFromConfig)
        {
            Level = level;
        }
    }
}