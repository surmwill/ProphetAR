using System;
using Swill.Recycler;

using static ProphetAR.GameEventShowARObjectSelectionUIOptionsData;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionRecyclerUIData : IRecyclerScrollRectData<string>
    {
        public string Key { get; }
     
        public ARObjectSelectionUIOptionData SelectionOptionData { get; }

        public TestTurnScreenARObjectSelectionRecyclerUIData(ARObjectSelectionUIOptionData selectionOptionData)
        {
            Key = selectionOptionData.Uid;
            SelectionOptionData = selectionOptionData;
        }
    }
}