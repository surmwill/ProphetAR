using System;
using Swill.Recycler;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionRecyclerUIData : IRecyclerScrollRectData<string>
    {
        public string Key { get; }
        
        public ARObjectSelectionUIOptionData OptionData { get; }
        
        public TestTurnScreenARObjectSelectionRecyclerUIData(ARObjectSelectionUIOptionData optionData)
        {
            Key = optionData.Key;
            OptionData = optionData;
        }
    }
}