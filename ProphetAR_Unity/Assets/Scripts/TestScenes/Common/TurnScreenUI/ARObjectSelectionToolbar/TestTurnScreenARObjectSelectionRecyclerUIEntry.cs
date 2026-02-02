using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenARObjectSelectionRecyclerUIEntry : RecyclerScrollRectEntry<string, TestTurnScreenARObjectSelectionRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _text = null;

        [SerializeField]
        private Button _button = null;

        private ARObjectSelectionUIOptionData OptionData => Data.OptionData;
        
        protected override void OnBind(TestTurnScreenARObjectSelectionRecyclerUIData entryData)
        {
            _text.text = entryData.OptionData.Name;
            _button.onClick.AddListener(OnOption);  
        }

        protected override void OnRecycled()
        {
            _button.onClick.RemoveListener(OnOption);
        }

        private void OnOption()
        {
            OptionData.OnSelect?.Invoke();
        }
    }
}