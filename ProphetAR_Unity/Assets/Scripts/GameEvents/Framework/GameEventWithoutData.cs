namespace ProphetAR
{
    public class GameEventWithoutData : GameEvent
    {
        public GameEventWithoutData(GameEventType gameEventType, int? customPriority = null) : base(gameEventType, customPriority)
        {
        }
    }
}