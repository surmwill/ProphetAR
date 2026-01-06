using Swill.Recycler;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnScreenActionRequestsRecyclerUIEntry : RecyclerScrollRectEntry<long, TestTurnScreenActionRequestsRecyclerUIData>
    {
        [SerializeField]
        private TMP_Text _actionDescription = null;
        
        protected override void OnBind(TestTurnScreenActionRequestsRecyclerUIData entryData)
        {
            _actionDescription.text = entryData.ActionRequest.GetType().Name;
        }
    }
}