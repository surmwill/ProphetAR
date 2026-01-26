namespace ProphetAR
{
    public interface ILevelLifecycleListener
    {
        void OnLevelLifecycleChanged(LevelLifecycleState lifecycleState, Level prevLevel, Level currLevel);
    }
}