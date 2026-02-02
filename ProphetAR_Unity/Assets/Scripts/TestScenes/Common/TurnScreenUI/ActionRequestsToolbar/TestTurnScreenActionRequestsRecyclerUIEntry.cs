using Swill.Recycler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsRecyclerUIEntry : RecyclerScrollRectEntry<long, TestTurnScreenActionRequestsRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _actionDescription = null;
        
        [SerializeField] 
        private Button _showActionUIButton = null;
        
        protected override void OnBind(TestTurnScreenActionRequestsRecyclerUIData entryData)
        {
            _actionDescription.text = entryData.ActionRequest.Name;
            _showActionUIButton.onClick.AddListener(ShowActionUI);
        }

        protected override void OnRecycled()
        {
            _showActionUIButton.onClick.RemoveListener(ShowActionUI);
        }

        private void ShowActionUI()
        {
            Data.ActionRequest.OnFocusUI();
            Data.ActionRequest.OnFocusCamera();
        }
    }
}