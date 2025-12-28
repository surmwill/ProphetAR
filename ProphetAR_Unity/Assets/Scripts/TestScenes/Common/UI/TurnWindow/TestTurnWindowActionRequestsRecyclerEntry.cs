using Swill.Recycler;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class TestTurnWindowActionRequestsRecyclerEntry : RecyclerScrollRectEntry<long, TestTurnWindowActionRequestsRecyclerData>
    {
        [SerializeField]
        private TMP_Text _actionDescription = null;
        
        protected override void OnBind(TestTurnWindowActionRequestsRecyclerData entryData)
        {
            _actionDescription.text = entryData.ActionRequest.GetType().Name;
        }
    }
}