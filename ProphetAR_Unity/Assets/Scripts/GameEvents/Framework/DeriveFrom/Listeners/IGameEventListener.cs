using System;

namespace ProphetAR
{
    /// <summary>
    /// Base interface for all game event listeners: those that receive data and those that don't
    /// </summary>
    public interface IGameEventListener
    {
        public const string OnEventMethodName = "OnEvent";
    }
}