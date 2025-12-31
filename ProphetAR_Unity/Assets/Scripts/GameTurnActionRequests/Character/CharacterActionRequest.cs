using System;

namespace ProphetAR
{
    public class CharacterActionRequest : GameTurnActionRequest
    {
        public override Type CompletedByGameEventType => typeof(GameEventCharacterEndTurnData);
        
        public Character Character { get; }

        public CharacterActionRequest(Character character)
        {
            Character = character;
        }
        
        public override void OnFocusUI()
        {
            Character.Player.EventProcessor.RaiseEventWithData(new GameEventShowCharacterActionsUI(new GameEventShowCharacterActionsUIData(Character)));
        }
    }
}