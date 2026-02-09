using UnityEngine;

namespace ProphetAR
{
    public class TestCharacterAttackCharacter : Character
    {
        public override Vector3 GetLocalPositionInCell(GridCellContent cell)
        {
            return cell.transform.InverseTransformPoint(GridTransform.transform.position);
        }
    }
}