using NUnit.Framework;

namespace ProphetAR.Tests
{
    public class TestGameEventProcessor
    {
        [Test]
        public void Test()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
        }

        class CharacterMoveListener : IGameEventCharacterMoveListener, IGameEventFireballStrikeListener
        {
            public void OnEvent(GameEventCharacterMoveData data)
            {
                
            }

            public void OnEvent(GameEventFireballStrikeData data)
            {
                
            }
        }
    }
}