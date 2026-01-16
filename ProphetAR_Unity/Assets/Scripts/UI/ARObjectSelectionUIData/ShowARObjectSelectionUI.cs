using System;

namespace ProphetAR
{
    public class ShowARObjectSelectionUI : IDisposable
    {
        public Level Level { get; private set; }
        
        public ShowARObjectSelectionUI(Level level, GameEventShowARObjectSelectionUI gameEvent)
        {
            Level = level;
            level.EventProcessor.RaiseEventWithData(gameEvent);
        }
        
        public ShowARObjectSelectionUI(Level level, params ARObjectSelectionUIOptionData[] options)
        {
            Level = level;
            level.EventProcessor.RaiseEventWithData(new GameEventShowARObjectSelectionUI(
                new GameEventShowARObjectSelectionUIOptionsData(options)));
        }
        
        public void Dispose()
        {
            Level.EventProcessor.RaiseEventWithoutData(new GameEventHideARObjectionSelectionUI());
        }
    }
}