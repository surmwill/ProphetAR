namespace ProphetAR
{
    /// <summary>
    /// Listens to a game event that passes data.
    /// All game event listeners that take data should implement this interface.
    /// </summary>
    public interface IGameEventWithTypedDataListener<TInterface, TData> : IGameEventWithDataListener where TInterface : IGameEventWithTypedDataListener<TInterface, TData>
    {
        public void OnEvent(TData data);
    }
}