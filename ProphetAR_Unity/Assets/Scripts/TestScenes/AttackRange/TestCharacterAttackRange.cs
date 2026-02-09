using System;
using GridOperations;
using UnityEngine;

namespace ProphetAR
{
    [RequireComponent(typeof(Character))]
    public class TestCharacterAttackRange : MonoBehaviour
    {
        private IDisposable _gridCellPainter;
        
        private Character _character;

        private void Awake()
        {
            _character = GetComponent<Character>();
        }

        private void Start()
        {
            OnCoordinatesChanged();
        }

        private void Update()
        {
            // Get position of character, flatten to XZ plane, transform point into local grid position, transform local grid position into cell
            _character.transform.localPosition = transform.localPosition.WithY(0f);

            // Using cell, create navigation destination set with infinite movement (all cells), take that navigation set
            if (_character.Grid.TryGetContainingCell(_character.transform, out GridCell gridCell) && gridCell.Coordinates != _character.GridTransform.Coordinates)
            {
                _character.GridTransform.MoveToImmediate(gridCell.Coordinates);
                OnCoordinatesChanged();
            }
        }

        private void OnDestroy()
        {
            _gridCellPainter?.Dispose();
        }

        private void OnCoordinatesChanged()
        {
            _gridCellPainter?.Dispose();
            NavigationDestinationSet locations = _character.GridTransform.GetPathsFrom(int.MaxValue, _character.Grid.GetExpensiveGlobalSlice());
            _gridCellPainter = _character.Grid.GridPainter.ShowAttackableArea(AttackRange.FromNavigationDestinations(locations, _character.Grid.GetExpensiveGlobalSlice()));
        }
    }
}