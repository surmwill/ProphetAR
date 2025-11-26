namespace ProphetAR
{
    /// <summary>
    /// A game event that does not pass along any data. All game events with this requirement should derive from this class.
    /// </summary>
    public abstract class GameEventWithoutData : GameEvent
    {
        public GameEventWithoutData(GameEventType gameEventType, int? customPriority = null) : base(gameEventType, customPriority)
        {
        }
    }
}