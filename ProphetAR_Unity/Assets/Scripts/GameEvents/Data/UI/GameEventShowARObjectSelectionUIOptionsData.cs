using System;
using System.Collections.Generic;

namespace ProphetAR
{
    /// <summary>
    /// When we're selecting an AR object there's UI options that go with it. These are those UI options
    /// </summary>
    public class GameEventShowARObjectSelectionUIOptionsData
    {
        public List<ARObjectSelectionUIOptionData> OptionsData { get; }
        
        public GameEventShowARObjectSelectionUIOptionsData(List<ARObjectSelectionUIOptionData> optionsData)
        {
            OptionsData = optionsData;
        }

        public class ARObjectSelectionUIOptionData
        {
            public Action OnSelect { get; }
            
            public string Uid { get; }

            public ARObjectSelectionUIOptionData(string uid, Action onSelect)
            {
                Uid = uid;
                OnSelect = onSelect;
            }
        }
    }
}