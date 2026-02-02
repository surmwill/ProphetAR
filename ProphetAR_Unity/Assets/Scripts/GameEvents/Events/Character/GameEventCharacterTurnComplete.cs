namespace ProphetAR
{
    public class GameEventCharacterTurnComplete : GameEventWithTypedData<Character>
    {
        public GameEventCharacterTurnComplete(Character data) : base(data)
        {
        }
    }
}