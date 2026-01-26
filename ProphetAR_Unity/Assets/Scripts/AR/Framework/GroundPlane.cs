using UnityEngine;

namespace ProphetAR
{
    public class GroundPlane : MonoBehaviour
    {
        [SerializeField]
        private Transform _content = null;

        public Transform Content
        {
            get => _content;
            set
            {
                if (_content != null)
                {
                    Destroy(_content.gameObject);
                    _content = null;
                }
                
                // The content parent should always have an identity transform.
                // Any of our (child) custom content can have their own transform values relative to this.
                _content = value;
                _content.SetParent(transform);
                _content.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        public const string Layer = "GroundPlane";
    }
}