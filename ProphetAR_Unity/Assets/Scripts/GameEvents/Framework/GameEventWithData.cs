namespace ProphetAR
{
    /// <summary>
    /// A game event that passes along data.
    /// Only used internally to fit all the generic data game events under one non-generic reference.
    /// Any game event that passes along data should derive from GameEventWithTypedData 
    /// </summary>
    public abstract class GameEventWithData : GameEvent
    {
        public object RawData { get; }
        
        protected GameEventWithData(GameEventType gameEventType, object rawData, int? customPriority = null) : base(gameEventType, customPriority)
        {
            RawData = rawData;
        }
    }
}