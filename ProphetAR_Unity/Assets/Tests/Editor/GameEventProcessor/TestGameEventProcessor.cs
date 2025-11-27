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
        
        // Test multiple objects with same listeners
        
        [Test]
        public void TestSimpleDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListenerWithData sampleListenerWithData = new SampleListenerWithData();
            
            gameEventProcessor.AddListenerWithData<IGameEventCharacterMoveListener, GameEventCharacterMoveData>(sampleListenerWithData);
            gameEventProcessor.AddListenerWithData<IGameEventFireballStrikeListener, GameEventFireballStrikeData>(sampleListenerWithData);
            
            gameEventProcessor.RemoveListenerWithData<IGameEventFireballStrikeListener>(sampleListenerWithData);
            
            gameEventProcessor.RaiseEventWithData(new GameEventFireballStrike(new GameEventFireballStrikeData()));
            gameEventProcessor.RaiseEventWithData(new GameEventCharacterMove(new GameEventCharacterMoveData()));
        }

        [Test]
        public void TestSimpleNoDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListenerWithoutData sampleListenerWithoutData = new SampleListenerWithoutData();
            
            gameEventProcessor.AddListenerWithoutData<IGameEventOpenMainMenuListener>(sampleListenerWithoutData);
            gameEventProcessor.AddListenerWithoutData<IGameEventOpenSettingsListener>(sampleListenerWithoutData);
            
            gameEventProcessor.RaiseEventWithoutData(new GameEventOpenMainMenu());
            gameEventProcessor.RaiseEventWithoutData(new GameEventOpenSettings());
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

        private class SampleListenerWithoutData : IGameEventOpenMainMenuListener, IGameEventOpenSettingsListener
        {
            void IGameEventWithoutDataListenerExplicit<IGameEventOpenMainMenuListener>.OnEvent()
            {
                Debug.Log("ON OPEN MAIN MENU");
            }

            void IGameEventWithoutDataListenerExplicit<IGameEventOpenSettingsListener>.OnEvent()
            {
                Debug.Log("ON OPEN SETTINGS");
            }
        }
    }
}