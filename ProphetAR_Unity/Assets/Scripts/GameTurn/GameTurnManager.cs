using System;
using System.Collections;
using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurnManager
    {
        public GameTurn CurrTurn { get; private set; }
        
        public GamePlayer CurrPlayer { get; private set; }

        public GamePlayer NextPlayer => _turnOrder[NextPlayerIndex];

        public int TurnNum { get; private set; }
        
        private int NextPlayerIndex => (_currPlayerIndex + 1) % _turnOrder.Count;

        private int _currPlayerIndex = -1;
        
        private readonly List<GamePlayer> _turnOrder;

        private readonly Level _level;

        public GameTurnManager(Level level, GamePlayer[] turnOrder)
        {
            _level = level;
            _turnOrder = new List<GamePlayer>(turnOrder);
        }
        
        public IEnumerator NextTurnCoroutine(Action onComplete = null)
        {
            _level.EventProcessor.RaiseEventWithData(new GameEventOnGameTurnChanged(
                new GameEventOnGameTurnChangedData(TurnNum, TurnNum + 1, CurrPlayer, NextPlayer)));
            
            TurnNum++;
            _currPlayerIndex = NextPlayerIndex;

            CurrPlayer = _turnOrder[_currPlayerIndex];
            CurrTurn = new GameTurn(_level, CurrPlayer, TurnNum);
            
            CurrTurn.PreTurn();
            CurrTurn.InitialBuild();

            if (CurrPlayer.Config.IsAI)
            {
                yield return CurrTurn.AIExecuteTurnAutomaticallyCoroutine();
            }
            
            onComplete?.Invoke();
        }
    }
}