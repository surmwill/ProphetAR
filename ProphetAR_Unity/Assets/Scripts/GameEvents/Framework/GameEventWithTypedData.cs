namespace ProphetAR
{
    /// <summary>
    /// A game event that passes along data. All game events with this requirment should derive from this class.
    /// </summary>
    public class GameEventWithTypedData<TData>: GameEventWithData
    {
        public TData Data { get; }

        public GameEventWithTypedData(GameEventType gameEventType, TData data, int? customPriority = null) : base(gameEventType, data, customPriority)
        {
            Data = data;
        }
    }
}