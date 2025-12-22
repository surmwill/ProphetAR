
namespace ProphetAR
{
    /// <summary>
    /// Listens to a game event that passes data.
    /// All game event listeners that take data should implement this interface.
    /// </summary>
    public interface IGameEventWithTypedDataListener<TInterfaceSelf, TData> : IGameEventWithDataListener where TInterfaceSelf : IGameEventWithTypedDataListener<TInterfaceSelf, TData>
    {
        public void OnEvent(TData data);
    }
}