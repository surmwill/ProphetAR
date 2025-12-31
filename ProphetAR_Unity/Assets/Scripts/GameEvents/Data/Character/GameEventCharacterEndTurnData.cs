namespace ProphetAR
{
    public class GameEventCharacterEndTurnData
    {
        public Character Character { get; }

        public GameEventCharacterEndTurnData(Character character)
        {
            Character = character;
        }
    }
}