using System.Collections;
using System.Collections.Generic;

namespace ProphetAR
{
    public class GamePlayerMultiTurnActions : IEnumerable<IMultiGameTurnAction>
    {
        private readonly SmallPriorityQueue<IMultiGameTurnAction, int> _multiTurnActions = new();
        private readonly GamePlayer _player;
        
        public GamePlayerMultiTurnActions(GamePlayer player)
        {
            _player = player;
        }
        
        public void AddMultiTurnAction(IMultiGameTurnAction multiTurnAction)
        {
            _multiTurnActions.Enqueue(multiTurnAction, multiTurnAction.Priority ?? IMultiGameTurnAction.DefaultPriority);
            _player.EventProcessor.RaiseEventWithData(new GameEventMultiGameTurnActionsModified(
                new GameEventMultiGameTurnActionsModifiedData(multiTurnAction, GameEventMultiGameTurnActionsModifiedData.ModificationType.Added)));
        }
        
        public void RemoveMultiTurnAction(IMultiGameTurnAction multiTurnAction)
        {
            _multiTurnActions.Remove(multiTurnAction, multiTurnAction.Priority ?? IMultiGameTurnAction.DefaultPriority);
            _player.EventProcessor.RaiseEventWithData(new GameEventMultiGameTurnActionsModified(
                new GameEventMultiGameTurnActionsModifiedData(multiTurnAction, GameEventMultiGameTurnActionsModifiedData.ModificationType.Removed)));
        }

        public void ChangeMultiTurnActionPriority(IMultiGameTurnAction multiTurnAction, int? newPriority)
        {
            int prevPrio = multiTurnAction.Priority ?? GameTurnActionRequest.DefaultPriority;
            int newPrio = newPriority ?? GameTurnActionRequest.DefaultPriority; 
            
            _multiTurnActions.ChangePriority(multiTurnAction, prevPrio, newPrio);
            _player.EventProcessor.RaiseEventWithData(new GameEventMultiGameTurnActionsModified(
                new GameEventMultiGameTurnActionsModifiedData(multiTurnAction, GameEventMultiGameTurnActionsModifiedData.ModificationType.PriorityChanged, prevPrio, newPriority)));
        }

        public IEnumerator<IMultiGameTurnAction> GetEnumerator()
        {
            return _multiTurnActions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}