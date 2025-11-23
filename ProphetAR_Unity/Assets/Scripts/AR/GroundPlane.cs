using UnityEngine;

namespace ProphetAR
{
    public class GroundPlane : MonoBehaviour
    {
        [SerializeField]
        private Transform _content = null;

        public Transform Content => _content;
        
        public const string Layer = "GroundPlane";
    }
}