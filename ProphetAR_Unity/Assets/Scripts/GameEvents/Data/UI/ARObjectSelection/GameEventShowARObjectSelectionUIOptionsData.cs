using System.Collections.Generic;
using System.Linq;

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
        
        public GameEventShowARObjectSelectionUIOptionsData(params ARObjectSelectionUIOptionData[] optionsData)
        {
            OptionsData = optionsData.ToList();
        }
    }
}