namespace ProphetAR
{
    /// <summary>
    /// Represents a game event listener that does not take data. All game events with that requirement should implement this.
    /// 
    /// <typeparam name="TInterfaceSelf"> The interface implementing this one (it passes its own type). Used to distinguish difference response methods with the same parameterless signature </typeparam>
    /// </summary>
    public interface IGameEventWithoutDataListener<TInterfaceSelf> : IGameEventListener where TInterfaceSelf : IGameEventWithoutDataListener<TInterfaceSelf>
    {
        public void OnEvent();
    }
}