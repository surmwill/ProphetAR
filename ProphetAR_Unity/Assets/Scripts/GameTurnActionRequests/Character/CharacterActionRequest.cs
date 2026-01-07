using System;

namespace ProphetAR
{
    public class CharacterActionRequest : GameTurnActionRequest
    {
        public override Type CompletedByGameEventType => typeof(GameEventCharacterOutOfActions);
        
        public Character Character { get; }

        public CharacterActionRequest(Character character)
        {
            Character = character;
        }
        
        public override void OnFocusUI()
        {
            Character.Player.EventProcessor.RaiseEventWithData(new GameEventShowCharacterActionsUI(Character));
        }

        public override bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            return gameEvent is GameEventCharacterOutOfActions characterOutOfActions && characterOutOfActions.Data == Character;
        }
    }
}