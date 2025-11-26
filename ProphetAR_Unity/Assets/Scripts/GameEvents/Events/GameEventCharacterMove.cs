namespace ProphetAR
{
    public class GameEventCharacterMove : GameEventWithTypedData<GameEventMovementStepData>
    {
        public GameEventCharacterMove(GameEventType gameEventType, GameEventMovementStepData data, int? customPriority = null) : 
            base(gameEventType, data, customPriority) { }
    }
}