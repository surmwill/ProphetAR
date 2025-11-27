namespace ProphetAR
{
    /// <summary>
    /// Base interface for all game event listeners that don't take data.
    /// 
    /// All listeners should derive from IGameEventWithoutDataListenerExplicit instead of this, which provides an extra generic parameter that
    /// allows for explicit interface implementation. If we derived from this we would not be able to handle implement multiple OnEvents differently.
    /// The extra generic parameter allows the different interfaces, all which have the same OnEvent signature, to be seen and treated differently.
    ///
    /// When we want to treat all those generic classes under one hood we need to use this interface though.
    /// </summary>
    public interface IGameEventWithoutDataListener : IGameEventListener
    {
    }
}