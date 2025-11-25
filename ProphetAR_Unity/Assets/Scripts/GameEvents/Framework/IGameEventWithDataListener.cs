namespace ProphetAR
{
    /// <summary>
    /// Base class for all game event listeners that take data.
    /// This class is only used internally to fit all the various generic data types under a single non-generic parent type.
    /// All game event listeners that take data should implement IGameEventWithTypedDataListener instead.
    /// </summary>
    public interface IGameEventWithDataListener : IGameEventListener
    {
        
    }
}