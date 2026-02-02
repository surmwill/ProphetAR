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

        public override string Name => "Movement";

        protected override int DefaultMinNumActionPoints => 1;

        protected override AbilityCoroutine AbilityAsCoroutine => ExecuteAbility;
        
        private ARGridCellSelector GridCellSelector => ARManager.Instance.ARGridCellSelector;
        
        private const float OnSelectedGridCellRaise = 0.1f;
        private const float OnSelectedGridCellScale = 1.3f;
        private const float OnSelectedGridCellAnimTime = 1.4f; 

        private WaitForARObjectSelection<GridCell> _waitForMovementCellSelection;

        private GridCell _currHoveredGridCell;
        private Sequence _onHoveredGridCellSequence;

        private IEnumerator ExecuteAbility(Action onComplete, Action onCancelled)
        {
            (NavigationDestinationSet destinationSet, GridSlice area) = Character.GetMovementArea();

            using (Character.Grid.GridPainter.ShowMovementArea(destinationSet, area))
            using (new ShowARObjectSelectionUI(Character.Level, 
                       ARObjectSelectionUIOptionDataFactory.Cancel(() => TryCancel()), 
                       ARObjectSelectionUIOptionDataFactory.Default("Select", SelectCurrentCell)))
            {
                _waitForMovementCellSelection = GridCellSelector.StartObjectSelection(
                    onHovered: OnNewGridCellHovered, 
                    getObjectFromCollision: hitTransform => hitTransform.GetComponentInParent<GridCell>(),
                    isValidObject: gridCell => destinationSet.Destinations.ContainsKey((gridCell.Coordinates - area.TopLeft).ToTuple()),
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
                .Append(currGridCell.Content.transform.DOLocalMoveY(OnSelectedGridCellRaise, OnSelectedGridCellAnimTime))
                .Join(currGridCell.Content.transform.DOScale(new Vector3(OnSelectedGridCellScale, OnSelectedGridCellScale, 1f), OnSelectedGridCellAnimTime));
        }

        private void StopCurrentCellHover()
        {
            _onHoveredGridCellSequence?.Kill();
            
            if (_currHoveredGridCell == null)
            {
                return;
            }
            
            _currHoveredGridCell.Content.transform.localPosition = _currHoveredGridCell.Content.transform.localPosition.WithY(0f);
            _currHoveredGridCell.Content.transform.localScale = Vector3.one;
        }

        private void SelectCurrentCell()
        {
            if (GridCellSelector.LastHovered == null)
            {
                Debug.LogError("No grid cell has been hovered for selection");
                return;
            }

            GridCellSelector.Select();
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