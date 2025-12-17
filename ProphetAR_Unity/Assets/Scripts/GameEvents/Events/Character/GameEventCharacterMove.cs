namespace ProphetAR
{
    public class GameEventCharacterMove : GameEventWithTypedData<GameEventCharacterMoveData>
    {
        public GameEventCharacterMove(GameEventCharacterMoveData data) : base(data) { }
    }
}