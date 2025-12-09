namespace ProphetAR
{
    public class Level
    {
        public LevelState LevelState { get; } = new();

        public LevelConfig LevelConfig { get; } = new();
        
        public Level(LevelConfig levelConfig)
        {
            levelConfig.InitializeLevelState(LevelState);
        }
    }
}