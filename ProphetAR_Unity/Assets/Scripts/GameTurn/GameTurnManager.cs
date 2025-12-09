using System.Collections.Generic;

namespace ProphetAR
{
    public class GameTurnManager
    {
        public Level Level { get; private set; }
        
        public List<GamePlayer> TurnOrder { get; }
        
        public GameTurn CurrTurn { get; private set; }
        
        public GamePlayer CurrPlayer { get; private set; }
        
        public int TurnNum { get; private set; }

        private int _currIndexTurnOrder = -1;

        public GameTurnManager(Level level, GamePlayer[] turnOrder)
        {
            Level = level;
            TurnOrder = new List<GamePlayer>(turnOrder);
        }

        public void NextTurn()
        {
            TurnNum++;
            _currIndexTurnOrder = (_currIndexTurnOrder + 1) % TurnOrder.Count;

            CurrPlayer = TurnOrder[_currIndexTurnOrder];
            CurrTurn = new GameTurn(TurnNum, CurrPlayer);
            
            CurrPlayer.EventProcessor.RaiseEventWithoutData(new GameEventOnPreTurn());
            
            CurrTurn.InitialBuild();
            CurrPlayer.EventProcessor.RaiseEventWithoutData(new GameEventOnInitialTurnBuilt());
            
            if (CurrPlayer.Config.IsAI)
            {
                CurrTurn.ExecuteAutomatically();
                CurrPlayer.EventProcessor.RaiseEventWithoutData(new GameEventOnTurnCompleted());
                CurrPlayer.EventProcessor.RaiseEventWithoutData(new Game);
            }
            // Else user completes the manual part of their turn, 
        }

        public void CompleteTurn()
        {
            
        }
        
        public void OnPreTurn()
        {
            // Raise event    
        }
        
        public void OnTurnInitialBuild()
        {
            // Raise event    
        }
        
        public void OnUserManualPartOfTurnCompleted()
        {
            UserExecuteAutomaticPartOfTurn();
        }

        public void UserExecuteAutomaticPartOfTurn()
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
            
            // Something needs to listen (maybe level itself), wait a couple seconds, and then call next turn
            // NextTurn();
        }

        public void OnPostTurn()
        {
            
        }
    }
}