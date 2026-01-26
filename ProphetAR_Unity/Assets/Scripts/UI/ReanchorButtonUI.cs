using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class ReanchorButtonUI : MonoBehaviour
    {
        [SerializeField]
        private Button _button = null;

        private void Awake()
        {
            _button.onClick.AddListener(OnReAnchor);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnReAnchor);
        }

        private void OnReAnchor()
        {
            Level.Current.EventProcessor.RaiseEventWithoutData(new GameEventOnReanchor());
        }

        private void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }
    }
}