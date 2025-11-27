namespace ProphetAR
{
    /// <summary>
    /// Represents a game event listener that does not take data. All game events with that requirement should implement this.
    ///
    /// "Explicit" is for the extra type parameter which allows us to distinguish between different data-less listeners, all of which
    /// have the same method signature. Without the type parameter, we would not be able to distinguish between the different OnEvents
    /// </summary>
    public interface IGameEventWithoutDataListenerExplicit<T> : IGameEventWithoutDataListener where T : IGameEventWithoutDataListenerExplicit<T>
    {
        public void OnEvent();
    }
}