
namespace ProphetAR
{
    /// <summary>
    /// Base game event class used internally to treat all game events (those that pass data and those that don't) under one common reference type.
    /// All game events should either derive from GameEventWithTypedData or GameEventWithoutData
    /// </summary>
    public abstract class GameEvent
    {
    }
}