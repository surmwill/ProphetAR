namespace ProphetAR
{
    /// <summary>
    /// Responds to game events that don't pass data.
    /// All game events that don't receive data should implement this interface.
    /// </summary>
    public interface IGameEventWithoutDataListener : IGameEventListener
    {
        public void OnEvent();
    }
}