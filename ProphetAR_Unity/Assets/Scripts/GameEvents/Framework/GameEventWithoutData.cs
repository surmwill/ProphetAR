namespace ProphetAR
{
    public abstract class GameEventWithoutData : GameEvent
    {
        public GameEventWithoutData(GameEventType gameEventType, int? customPriority = null) : base(gameEventType, customPriority)
        {
        }
    }
}