using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestARSetupScreenUI : MonoBehaviour, IGameEventOnReanchorListener
    {
        [SerializeField]
        private Button _placeGroundPlaneButton = null;

        [SerializeField]
        private Button _resetToScanRoomButton = null;

        [SerializeField]
        private TMP_Text _statusText = null;

        [SerializeField]
        private GameObject _groundPlaneContentPrefab = null;

        [SerializeField]
        private UnityEvent _onGroundPlanePlaced = null;

        private GroundPlaneManager GroundPlaneManager => ARManager.Instance.GroundPlaneManager;

        private void Awake()
        {
            _placeGroundPlaneButton.onClick.AddListener(TryPlaceGroundPlane);
            _resetToScanRoomButton.onClick.AddListener(ResetScanRoom);   
            
            Level.RegisterOnLevelInitializedCallback(OnLevelInitialized);
        }

        private void Start()
        {
            ResetScanRoom();
        }

        private void OnLevelInitialized(Level level)
        {
            level.EventProcessor.AddListenerWithoutData<IGameEventOnReanchorListener>(this);
        }

        private void ResetScanRoom()
        {
            gameObject.SetActive(true);

            _placeGroundPlaneButton.gameObject.SetActive(false);
            _resetToScanRoomButton.gameObject.SetActive(false);

            GroundPlaneManager.DestroyGroundPlane();
            GroundPlaneManager.ScanRoomForPlanes(
                progress => SetStatusText($"scanning room: {progress}%"),
                () =>
                {
                    SetStatusText("placing ground plane");

                    _placeGroundPlaneButton.gameObject.SetActive(true);
                    _resetToScanRoomButton.gameObject.SetActive(false);

                    GroundPlaneManager.StartGroundPlanePlacement(_groundPlaneContentPrefab);
                },
                true);
        }

        private void TryPlaceGroundPlane()
        {
            if (GroundPlaneManager.TryPlaceGroundPlane())
            {
                SetStatusText("placed ground plane");

                _placeGroundPlaneButton.gameObject.SetActive(false);
                _resetToScanRoomButton.gameObject.SetActive(true);

                _onGroundPlanePlaced?.Invoke();
            }
            else
            {
                SetStatusText("failed to place ground plane");
            }
        }

        private void SetStatusText(string status)
        {
            _statusText.text = $"Status: {status}";
        }

        private void OnDestroy()
        {
            _placeGroundPlaneButton.onClick.RemoveListener(TryPlaceGroundPlane);
            _resetToScanRoomButton.onClick.RemoveListener(ResetScanRoom);
        }

        void IGameEventWithoutDataListener<IGameEventOnReanchorListener>.OnEvent()
        {
            ResetScanRoom();
        }
    }
}