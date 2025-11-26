namespace ProphetAR
{
    /// <summary>
    /// A game event that passes along data. All game events with this requirement should derive from this class.
    /// </summary>
    public class GameEventWithTypedData<TData>: GameEventWithData
    {
        public TData Data { get; }

        public GameEventWithTypedData(TData data) : base(data)
        {
            Data = data;
        }
    }
}