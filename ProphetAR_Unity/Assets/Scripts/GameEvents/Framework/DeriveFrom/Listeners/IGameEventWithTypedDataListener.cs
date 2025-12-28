
namespace ProphetAR
{
    /// <summary>
    /// Listens to a game event that passes data.
    /// All game event listeners that take data should implement this interface.
    /// </summary>
    /// <typeparam name="TInterfaceSelf"> The interface implementing this one (it passes its own type) </typeparam>
    /// <typeparam name="TData"> The type of data being passed </typeparam>
    public interface IGameEventWithTypedDataListener<TInterfaceSelf, in TData> : IGameEventListener where TInterfaceSelf : IGameEventWithTypedDataListener<TInterfaceSelf, TData>
    {
        public void OnEvent(TData data);
    }
}