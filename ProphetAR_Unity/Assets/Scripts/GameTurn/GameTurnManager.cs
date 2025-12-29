using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurnManager
    {
        public GameTurn CurrTurn { get; private set; }
        
        public GamePlayer CurrPlayer { get; private set; }
        
        public int TurnNum { get; private set; }

        private int _currPlayerIndex = -1;
        
        private readonly List<GamePlayer> _turnOrder;

        private readonly Level _level;

        public GameTurnManager(Level level, GamePlayer[] turnOrder)
        {
            _level = level;
            _turnOrder = new List<GamePlayer>(turnOrder);
        }

        public void NextTurn()
        {
            _level.EventProcessor.RaiseEventWithData(new GameEventOnGameTurnChanged(new GameEventOnGameTurnChangedData(TurnNum, TurnNum + 1)));
            
            TurnNum++;
            _currPlayerIndex = (_currPlayerIndex + 1) % _turnOrder.Count;

            CurrPlayer = _turnOrder[_currPlayerIndex];
            CurrTurn = new GameTurn(_level, CurrPlayer, TurnNum);
            
            CurrTurn.PreTurn();
            CurrTurn.InitialBuild();
            
            if (CurrPlayer.Config.IsAI)
            {
                CurrTurn.AIExecuteActionRequestsAutomatically();
            }
        }
    }
}