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

        protected override AbilityCoroutine AbilityAsCoroutine => ExecuteAbility;
        
        private const float OnSelectedGridCellRaise = 0.3f;
        private const float OnSelectedGridCellScale = 1.3f;
        private const float OnSelectedGridCellAnimTime = 1.4f; 

        private WaitForARObjectSelection<GridCell> _waitForMovementCellSelection;

        private GridCell _currHoveredGridCell;
        private Sequence _onHoveredGridCellSequence;

        private IEnumerator ExecuteAbility(Action onComplete, Action onCancelled)
        {
            (NavigationDestinationSet destinations, GridSlice area) = Character.GetMovementArea();

            using (Character.Grid.GridPainter.ShowMovementArea(destinations, area))
            {
                _waitForMovementCellSelection = ARManager.Instance.ARGridCellSelector.StartObjectSelection(
                    onHovered: OnNewGridCellHovered, 
                    getObjectFromCollision: hitTransform => hitTransform.GetComponentInParent<GridCell>(),
                    isValidObject: gridCell =>
                    {
                        Debug.Log(gridCell.Coordinates);
                        return area.ContainsCoordinates(gridCell.Coordinates);
                    },
                    onSelected: _ => StopCurrentCellHover(),
                    onCancelled: StopCurrentCellHover);
                
                
                
                yield return _waitForMovementCellSelection;
            }
            
            if (_waitForMovementCellSelection.ResolvedCancelled)
            {
                onCancelled?.Invoke();
                yield break;
            }

            GridCell selectedGridCell = _waitForMovementCellSelection.SelectedObject;
            yield return Character.WalkToCoordinates(selectedGridCell.Coordinates);
            
            onComplete?.Invoke();
        }

        private void OnNewGridCellHovered(GridCell prevGridCell, GridCell currGridCell)
        {
            StopCurrentCellHover();

            _currHoveredGridCell = currGridCell;
            _onHoveredGridCellSequence = DOTween.Sequence()
                .Append(currGridCell.transform.DOLocalMoveY(OnSelectedGridCellRaise, OnSelectedGridCellAnimTime))
                .Join(currGridCell.transform.DOScale(new Vector3(OnSelectedGridCellScale, OnSelectedGridCellScale, 1f), OnSelectedGridCellAnimTime));
        }

        private void StopCurrentCellHover()
        {
            _onHoveredGridCellSequence?.Kill();
            
            if (_currHoveredGridCell == null)
            {
                return;
            }
            
            _currHoveredGridCell.transform.localPosition = _currHoveredGridCell.transform.localPosition.WithY(0f);
            _currHoveredGridCell.transform.localScale = Vector3.one;
        }

        public override bool TryCancel()
        {
            if (_waitForMovementCellSelection.IsWaiting)
            {
                _waitForMovementCellSelection.Selector.Cancel();
                return true;
            }
            
            Debug.LogWarning("Cannot cancel mid-movement");
            return false;
        }

        public CharacterAbilityMovement(Character character) : base(character) { }
    }
}