namespace ProphetAR
{
    /// <summary>
    /// Base interface for all game event listeners, those that recieve data and those that don't
    /// </summary>
    public interface IGameEventListener
    {
        public const string OnEventMethodName = "OnEvent";
    }
}