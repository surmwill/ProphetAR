namespace ProphetAR
{
    public class GameEventWithTypedData<TData>: GameEventWithData
    {
        public TData Data { get; }

        public GameEventWithTypedData(GameEventType gameEventType, TData data, int? customPriority = null) : base(gameEventType, data, customPriority)
        {
            Data = data;
        }
    }
}