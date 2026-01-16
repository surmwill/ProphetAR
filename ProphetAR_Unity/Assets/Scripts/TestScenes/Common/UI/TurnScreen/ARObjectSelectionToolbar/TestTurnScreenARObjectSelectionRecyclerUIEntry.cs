using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static ProphetAR.GameEventShowARObjectSelectionUIOptionsData;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionRecyclerUIEntry : RecyclerScrollRectEntry<string, TestTurnScreenARObjectSelectionRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _text = null;

        [SerializeField]
        private Button _button = null;
        
        public Button Button { get; }

        private ARObjectSelectionUIOptionData SelectionOptionData => Data.SelectionOptionData;
        
        protected override void OnBind(TestTurnScreenARObjectSelectionRecyclerUIData entryData)
        {
            _button.onClick.AddListener(OnOption);  
            _text.text = SelectionOptionData.Uid;
        }

        protected override void OnRecycled()
        {
            _button.onClick.RemoveListener(OnOption);
        }

        private void OnOption()
        {
            SelectionOptionData.OnSelect?.Invoke();
        }
    }
}