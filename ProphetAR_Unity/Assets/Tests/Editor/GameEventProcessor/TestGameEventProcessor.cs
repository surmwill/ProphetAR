using NUnit.Framework;
using UnityEngine;

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
            SampleListenerWithData sampleListenerWithData = new SampleListenerWithData();
            
            gameEventProcessor.AddListenerWithData<IGameEventCharacterMoveListener, GameEventCharacterMoveData>(sampleListenerWithData);
            gameEventProcessor.AddListenerWithData<IGameEventFireballStrikeListener, GameEventFireballStrikeData>(sampleListenerWithData);
            
            gameEventProcessor.RemoveListenerWithData<IGameEventFireballStrikeListener>(sampleListenerWithData);
            
            gameEventProcessor.RaiseEvent(new GameEventFireballStrike(new GameEventFireballStrikeData()));
            gameEventProcessor.RaiseEvent(new GameEventCharacterMove(new GameEventCharacterMoveData()));
        }

        private class SampleListenerWithData : IGameEventCharacterMoveListener, IGameEventFireballStrikeListener
        {
            void IGameEventWithTypedDataListener<IGameEventCharacterMoveListener, GameEventCharacterMoveData>.OnEvent(GameEventCharacterMoveData data)
            {
                Debug.Log("ON MOVE");
            }

            void IGameEventWithTypedDataListener<IGameEventFireballStrikeListener, GameEventFireballStrikeData>.OnEvent(GameEventFireballStrikeData data)
            {
                Debug.Log("ON FIREBALL");
            }
        }
    }
}