using System;
using System.Collections;
using DG.Tweening;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    public class CharacterAbilityMovement : CharacterAbility
    {
        public override string Uid => nameof(CharacterAbilityMovement);

        public override int MinNumActionPoints => 1;

        protected override CharacterAbilityCoroutine AbilityCoroutine => ExecuteAbility;
        
        private const float OnSelectedGridCellRaise = 0.3f;
        private const float OnSelectedGridCellScale = 1.3f;
        private const float OnSelectedGridCellAnimTime = 1.4f; 

        private WaitForARObjectSelection<GridCell> _waitForMovementCellSelection;

        private Sequence _onSelectedGridCellSequence;

        private IEnumerator ExecuteAbility(Action onComplete, Action onCancelled)
        {
            (NavigationDestinationSet destinations, GridSlice area) = Character.GetMovementArea();

            using (Character.Grid.GridPainter.ShowMovementArea(destinations, area))
            {
                _waitForMovementCellSelection = ARManager.Instance.ARGridCellSelector.StartObjectSelection(
                    onHovered: OnGridCellHovered, 
                    isValidForSelection: gridCell => area.ContainsCoordinates(gridCell.Coordinates),
                    onSelected: _ => StopCellHover(),
                    onCancelled: StopCellHover);
                
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
            StopCellHover();
            _onSelectedGridCellSequence = DOTween.Sequence()
                .Append(currGridCell.transform.DOLocalMoveY(OnSelectedGridCellRaise, OnSelectedGridCellAnimTime))
                .Join(currGridCell.transform.DOScale(new Vector3(OnSelectedGridCellScale, OnSelectedGridCellScale, 1f), OnSelectedGridCellAnimTime));
        }

        private void StopCellHover()
        {
            _onSelectedGridCellSequence?.Kill();

            GridCell hoveredGridCell = _waitForMovementCellSelection.Selector.LastHovered;
            if (hoveredGridCell == null)
            {
                return;
            }
            
            hoveredGridCell.transform.localPosition = hoveredGridCell.transform.localPosition.WithY(0f);
            hoveredGridCell.transform.localScale = Vector3.one;
        }

        public override void Cancel()
        {
            if (_waitForMovementCellSelection.IsWaiting)
            {
                _waitForMovementCellSelection.Selector.Cancel();
            }
            else
            {
                Debug.LogWarning("Cannot cancel mid-movement");
            }
        }

        public CharacterAbilityMovement(Character character) : base(character) { }
    }
}