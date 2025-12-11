using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurnManager
    {
        public GameTurn CurrTurn { get; private set; }
        
        public GamePlayer CurrPlayer { get; private set; }
        
        public int TurnNum { get; private set; }

        private int _currIndexTurnOrder = -1;
        
        private readonly List<GamePlayer> _turnOrder;

        private readonly Level _level;

        public GameTurnManager(Level level, GamePlayer[] turnOrder)
        {
            _level = level;
            _turnOrder = new List<GamePlayer>(turnOrder);
        }

        public void NextTurn()
        {
            TurnNum++;
            _currIndexTurnOrder = (_currIndexTurnOrder + 1) % _turnOrder.Count;

            CurrPlayer = _turnOrder[_currIndexTurnOrder];
            CurrTurn = new GameTurn(TurnNum, CurrPlayer);
            
            CurrTurn.PreTurn();
            CurrTurn.InitialBuild();
            
            if (CurrPlayer.Config.IsAI)
            {
                CurrTurn.AIExecuteActionRequestsAutomatically();
                CurrPlayer.EventProcessor.RaiseEventWithoutData(new GameEventOnGameTurnCompleted());
            }
        }
    }
}