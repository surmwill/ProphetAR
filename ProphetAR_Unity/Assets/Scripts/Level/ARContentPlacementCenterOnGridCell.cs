using UnityEngine;

namespace ProphetAR
{
    public class ARContentPlacementCenterOnGridCell : MonoBehaviour, IARContentPlacementPositioner
    {
        [SerializeField]
        private GridCell _gridCell = null;

        public void PositionContent()
        {
            transform.position += transform.position - _gridCell.Middle;
        }
    }
}