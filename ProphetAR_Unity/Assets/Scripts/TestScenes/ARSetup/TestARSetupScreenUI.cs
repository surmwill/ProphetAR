using ProphetAR;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestARSetupScreenUI : MonoBehaviour
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
    
    private void Start()
    {
        _placeGroundPlaneButton.onClick.AddListener(TryPlaceGroundPlane);
        _resetToScanRoomButton.onClick.AddListener(ResetToScanRoom);
        
        ResetToScanRoom();
    }

    private void ResetToScanRoom()
    {
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
        _resetToScanRoomButton.onClick.RemoveListener(ResetToScanRoom);
    }
}
