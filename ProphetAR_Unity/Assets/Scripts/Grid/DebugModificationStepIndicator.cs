using TMPro;
using UnityEngine;

namespace ProphetAR
{
    public class DebugModificationStepIndicator : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _modificationValueText = null;

        public void SetModificationText(int modificationText)
        {
            _modificationValueText.text = modificationText.ToString();
        }
    }
}