using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurnManager
    {
        public Level Level { get; private set; }
        
        public List<GamePlayer> Players { get; }
        
        public GameTurn CurrTurn { get; private set; }
        
        public GamePlayer CurrPlayer { get; private set; }
        
        public int TurnNum { get; private set; }

        private int _currIndexTurnOrder = -1;

        public GameTurnManager(Level level, List<GamePlayer> players)
        {
            Level = level;
            Players = players;
        }

        public void NextTurn()
        {
            TurnNum++;
            _currIndexTurnOrder = (_currIndexTurnOrder + 1) % Players.Count;

            CurrPlayer = Players[_currIndexTurnOrder];
            CurrTurn = new GameTurn(this, TurnNum, CurrPlayer.Uid);
            OnPreTurn();
            
            CurrTurn.InitialBuild();
            OnTurnInitialBuild();
            
            if (CurrPlayer.Config.IsAI)
            {
                ExecuteAutomaticPartOfTurn();
            }
        }
        
        public void OnPreTurn()
        {
            // Raise event    
        }
        
        public void OnTurnInitialBuild()
        {
            // Raise event    
        }
        
        public void OnManualPartOfTurnCompleted()
        {
            ExecuteAutomaticPartOfTurn();
        }

        public void ExecuteAutomaticPartOfTurn()
        {
            // Raise event for automatic events
            
            CurrTurn.ExecuteAutomatically();
            if (!CurrTurn.HasActionRequests)
            {
                OnTurnCompleted();
            }
        }
        
        public void OnTurnCompleted()
        {
            // Raise event
            
            // Something needs to listen, wait a couple seconds, and then call next turn
            // NextTurn();
        }
    }
}