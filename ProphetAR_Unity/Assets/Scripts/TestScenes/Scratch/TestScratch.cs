using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// A scratch file for when you need to test or verify something simple that does not need to be preserved
    /// </summary>
    public class TestScratch : MonoBehaviour
    {
        [SerializeField]
        private ARContentPlacementCenterOnGridCell _placement = null;

        private void Start()
        {
            _placement.PositionContent();
        }
    }
}