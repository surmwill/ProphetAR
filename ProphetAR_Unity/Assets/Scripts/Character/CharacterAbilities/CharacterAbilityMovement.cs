using System.Collections;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class CharacterAbilityMovement : CharacterAbility
    {
        private const float OnSelectedGridCellRaise = 0.3f;
        private const float OnSelectedGridCellScale = 1.3f;
        private const float OnSelectedGridCellAnimTime = 1.4f; 
        
        public override string Uid => nameof(CharacterAbilityMovement);

        public override int MinNumActionPoints => 1;

        public override IEnumerator AbilityCoroutine => ExecuteAbility();

        private WaitForARObjectSelection<GridCell> _waitForMovementCellSelection;

        private Sequence _onSelectedGridCellSequence;

        private IEnumerator ExecuteAbility()
        {
            (NavigationDestinationSet destinations, GridSlice area) = Character.GetMovementArea();

            using (Character.Grid.GridPainter.ShowMovementArea(destinations, area))
            {
                _waitForMovementCellSelection = ARManager.Instance.ARGridCellSelector.StartObjectSelection(
                    onHovered: OnGridCellHovered, 
                    isValidForSelection: gridCell => area.ContainsCoordinates(gridCell.Coordinates),
                    onSelected: StopCellHover,
                    onCancelled: () => StopCellHover(_waitForMovementCellSelection.Selector.LastHovered));
                
                yield return _waitForMovementCellSelection;
            }
            
            if (_waitForMovementCellSelection.ResolvedCancelled)
            {
                yield break;
            }

            GridCell selectedGridCell = _waitForMovementCellSelection.SelectedObject;
            yield return Character.WalkToCoordinates(selectedGridCell.Coordinates);
        }

        private void OnGridCellHovered(GridCell prevGridCell, GridCell currGridCell)
        {
            _onSelectedGridCellSequence?.Kill();
            
            StopCellHover(prevGridCell);
            
            _onSelectedGridCellSequence = DOTween.Sequence()
                .Append(currGridCell.transform.DOLocalMoveY(OnSelectedGridCellRaise, OnSelectedGridCellAnimTime))
                .Join(currGridCell.transform.DOScale(new Vector3(OnSelectedGridCellScale, OnSelectedGridCellScale, 1f), OnSelectedGridCellAnimTime));
        }

        private void StopCellHover(GridCell gridCell)
        {
            if (gridCell == null)
            {
                return;
            }
            
            gridCell.transform.localPosition = gridCell.transform.localPosition.WithY(0f);
            gridCell.transform.localScale = Vector3.one;
        }

        public override void Cancel()
        {
            if (_waitForMovementCellSelection.IsWaiting)
            {
                _waitForMovementCellSelection.Selector.Cancel();
            }
        }

        public CharacterAbilityMovement(Character character) : base(character)
        {
        }
    }
}