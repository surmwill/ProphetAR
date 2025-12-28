namespace ProphetAR
{
    /// <summary>
    /// Represents a game event listener that does not take data. All game events with that requirement should implement this.
    ///
    /// "Explicit" is for the extra type parameter that allows us to distinguish between different data-less listeners, all of which
    /// have the same method signature (no parameters). Without the type parameter, they'd all hook up to the same OnEvent response.
    /// 
    /// <typeparam name="TInterfaceSelf"> The interface implementing this one (it passes its own type) </typeparam>
    /// </summary>
    public interface IGameEventWithoutDataListener<TInterfaceSelf> : IGameEventListener where TInterfaceSelf : IGameEventWithoutDataListener<TInterfaceSelf>
    {
        public void OnEvent();
    }
}