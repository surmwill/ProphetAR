using ProphetAR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestARSetup : MonoBehaviour
{
    [SerializeField]
    private Button _placeGroundPlaneButton = null;

    [SerializeField]
    private Button _resetToScanRoomButton = null;

    [SerializeField]
    private TMP_Text _statusText = null;

    [SerializeField]
    private GameObject _groundPlaneContentPrefab = null;
    
    private void Start()
    {
        _placeGroundPlaneButton.onClick.AddListener(TryPlaceGroundPlane);
        _resetToScanRoomButton.onClick.AddListener(ResetToScanRoom);
        
        ResetToScanRoom();
    }

    private void ResetToScanRoom()
    {
        SetStatusText("scanning room");
        
        _placeGroundPlaneButton.gameObject.SetActive(false);
        _resetToScanRoomButton.gameObject.SetActive(false);
        
        ARManager.Instance.GroundPlaneManager.ScanRoomForPlanes(
            progress => Debug.Log($"TODELETE: {progress}"),
            () =>
            {
                SetStatusText("placing ground plane");
                _placeGroundPlaneButton.gameObject.SetActive(true);
                ARManager.Instance.GroundPlaneManager.StartGroundPlanePlacement(_groundPlaneContentPrefab);
            },
            true);
    }

    private void TryPlaceGroundPlane()
    {
        if (ARManager.Instance.GroundPlaneManager.TryPlaceGroundPlane())
        {
            SetStatusText("placed ground plane");
            _placeGroundPlaneButton.gameObject.SetActive(false);
            _resetToScanRoomButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log($"TODELETE: failed to place ground plane");  
        }
    }

    private void SetStatusText(string status)
    {
        _statusText.text = $"Status: {status}";
    }

    private void OnDestroy()
    {
        _placeGroundPlaneButton.onClick.RemoveListener(TryPlaceGroundPlane);
    }
}
