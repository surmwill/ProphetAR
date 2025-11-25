namespace ProphetAR
{
    public class GameEventMovementStep : GameEventWithTypedData<GameEventMovementStepData>
    {
        public GameEventMovementStep(GameEventType gameEventType, GameEventMovementStepData data, int? customPriority = null) : 
            base(gameEventType, data, customPriority) { }
    }
}