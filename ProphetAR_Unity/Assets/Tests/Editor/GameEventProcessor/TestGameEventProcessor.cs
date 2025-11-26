using NUnit.Framework;

namespace ProphetAR.Tests
{
    public class TestGameEventProcessor
    {
        // Test basic raise
        
        // Test multiple raises 
        
        // Test raises, add, and the added gets raised too
        
        // Test raise, move to next, remove first, and we should exit
        
        [Test]
        public void TestSimpleRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
        }

        private class SampleListenerWithData : IGameEventCharacterMoveListener, IGameEventFireballStrikeListener
        {
            public void OnEvent(GameEventCharacterMoveData data)
            {
                
            }

            public void OnEvent(GameEventFireballStrikeData data)
            {
                
            }
        }

        private class SampleListenerWithoutData : IGameEventOpenMainMenuListener, IGameEventOpenSettingsListener
        {
            public void IGameEventOpenMainMenuListener.OnEvent()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}