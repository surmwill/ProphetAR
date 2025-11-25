namespace ProphetAR
{
    public abstract class GameEventWithData : GameEvent
    {
        public object RawData { get; }
        
        protected GameEventWithData(GameEventType gameEventType, object rawData, int? customPriority = null) : base(gameEventType, customPriority)
        {
            RawData = rawData;
        }
    }
}