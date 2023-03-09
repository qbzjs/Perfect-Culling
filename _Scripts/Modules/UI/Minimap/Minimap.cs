using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("Minimap", "PopupCanvasInteract")]
public class Minimap : BasePanel
{
    public Transform playerTransform;
    private Vector2 offSetMiniMap = new Vector2(136, -16);
    [SerializeField] private RectTransform playerRectTransform;
    [SerializeField] private RectTransform playerDirection;
    [SerializeField] private RectTransform mapImageTransform;
    [SerializeField] private RectTransform missionRectTransform;
    [SerializeField] private Image missionTargetImage;
    [SerializeField] private Button bigMapButton;
    public Vector3 missionTargetPosition;
    public string missionTargetName;
    private float mapSize = 2050f;
    public void SetPlayerTransform(Transform _player)
    {
        playerTransform = _player;
    }
    private void Update()
    {
        UpdatePosition();
    }
    private void UpdatePosition()
    {
        Vector2 position = offSetMiniMap - new Vector2(playerTransform.position.x, playerTransform.position.z);
        playerRectTransform.anchoredPosition = position;
        mapImageTransform.pivot = new Vector2((mapSize / 2 + position.x) / mapSize, (position.y + mapSize / 2) / mapSize);
        mapImageTransform.anchoredPosition = Vector2.zero;
    }
    protected override void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.ChangeQuest, ChangeMissionPosition);
        Observer.Instance.AddObserver(ObserverKey.CameraRotation, RotationDirection);
        bigMapButton.onClick.AddListener(OnBigMapButtonClick);
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.ChangeQuest, ChangeMissionPosition);
        Observer.Instance.RemoveObserver(ObserverKey.CameraRotation, RotationDirection);
    }

    private void RotationDirection(object data)
    {
        Quaternion rotation = (Quaternion)data;
        playerDirection.rotation = rotation;
    }
    public void SetMissionTargetPosition(Vector3 target_position)
    {
        missionTargetPosition = target_position;
        if (missionRectTransform != null)
            missionRectTransform.anchoredPosition = offSetMiniMap - new Vector2(missionTargetPosition.x, missionTargetPosition.z);
    }
    protected void ChangeMissionPosition(object data)
    {
        if (data == null)
        {
            if (missionRectTransform != null)
                missionRectTransform.gameObject.SetActive(false);
        }
        else
        {
            if (missionRectTransform != null)
                missionRectTransform.gameObject.SetActive(true);
            RecordMissionInteractionInfo record_mission = (RecordMissionInteractionInfo)data;
            Vector3 targetPosition = new Vector3(record_mission.target_position[0], record_mission.target_position[1], record_mission.target_position[2]);
            SetMissionTargetPosition(targetPosition);
        }
    }

    Map map = null;

    public void SetActiveMap()
    {
        bool isActive = (map == null || !map.gameObject.activeSelf);
        if (isActive)
        {
            ActiveBigMap();
        }
        else
        {
            DeactiveBigMap();
        }
        GameConfig.gameBlockInput = isActive;
    }
    private void OnBigMapButtonClick()
    {
        ActiveBigMap();
    }
    private void ActiveBigMap()
    {
        GameConfig.gameBlockInput = true;
        Ultis.SetActiveCursor(true);
        Dictionary<KeyCode, List<string>> listAvailableKey = new Dictionary<KeyCode, List<string>>();
        List<string> m_action_names = new List<string>();
        m_action_names.Add("SetActiveMap");
        listAvailableKey.Add(KeyCode.M, m_action_names);
        listAvailableKey.Add(KeyCode.Mouse0, null);
        InputRegisterEvent.Instance.SetlstKeyAvailable(listAvailableKey);
        map = PanelManager.Show<Map>();
        map.SetPlayerTransform(playerTransform);
        map.SetMissionTargetPosition(missionTargetPosition);
    }
    private void DeactiveBigMap()
    {
        Ultis.SetActiveCursor(false);
        PanelManager.Hide<Map>();
    }
}
