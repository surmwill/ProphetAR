using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestRecyclerUIEntry : RecyclerScrollRectEntry<string, TestRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _text = null;

        [SerializeField]
        private Image _background = null;
        
        [SerializeField]
        private LayoutElement _layoutElement = null;
        
        protected override void OnBind(TestRecyclerUIData entryData)
        {
            _text.text = entryData.Key;
            _background.color = entryData.Color;

            _layoutElement.preferredHeight = !entryData.IsLarge ?
                TestTestRecyclerUI.SizeSmallEntry :
                TestTestRecyclerUI.SizeLargeEntry;
        }
    }
}