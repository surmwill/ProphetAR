using System;
using System.Collections;

namespace ProphetAR
{
    /// <summary>
    /// An action performed over multiple turns 
    /// </summary>
    public class GameTurnActionOverTurnsTurn
    {
        public IEnumerator TurnCoroutine { get; }
        
        public Action TurnCallback { get; }
        
        public GameTurnActionRequest ManualActionRequest { get; }

        public TurnOperation Operation
        {
            get
            {
                if (TurnCoroutine != null)
                {
                    return TurnOperation.Coroutine;
                }

                if (TurnCallback != null)
                {
                    return TurnOperation.Callback;
                }
                
                if (ManualActionRequest != null)
                {
                    return TurnOperation.ManualActionRequest;
                }

                return TurnOperation.Empty;
            }
        }

        public GameTurnActionOverTurnsTurn()
        {
            
        }

        public GameTurnActionOverTurnsTurn(IEnumerator turnCoroutine)
        {
            TurnCoroutine = turnCoroutine;
        }
        
        public GameTurnActionOverTurnsTurn(Action turnCallback)
        {
            TurnCallback = turnCallback;
        }
        
        public GameTurnActionOverTurnsTurn(GameTurnActionRequest manualActionRequest)
        {
            ManualActionRequest = manualActionRequest;
        }

        public enum TurnOperation
        {
            Empty = 0,
            Coroutine = 1,
            Callback = 2,
            ManualActionRequest = 3,
        }
    }
}