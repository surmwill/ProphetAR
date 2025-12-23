namespace ProphetAR
{
    public interface ILevelConfigContributor
    {
        string LevelConfigEditedBy { get; }

        void EditLevelConfig(LevelConfig levelConfig);
    }
}