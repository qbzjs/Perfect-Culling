using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapPlace : MonoBehaviour
{
    public Button teleportButton;
    public TMP_Text placeNameText;
    private string placeName;

    private void Awake()
    {
        if (placeNameText != null)
        {
            placeName = placeNameText.text;
        }
        if (teleportButton != null)
        {
            teleportButton.onClick.AddListener(OnTeleportButtonClick);
        }
    }
    public void OnTeleportButtonClick()
    {
        Observer.Instance.Notify(ObserverKey.MapTeleport,placeName);
    }
}
