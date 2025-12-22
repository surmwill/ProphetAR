namespace ProphetAR
{
    public interface IContributesToLevelConfig
    {
        string LevelConfigEditedBy { get; }

        void EditLevelConfig(LevelConfig levelConfig);
    }
}