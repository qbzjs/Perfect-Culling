using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[UIPanelPrefabAttr("Map", "PopupCanvas")]
public class Map : BasePanel
{
    private Transform playerTransform;
    private Vector2 offSetMap = new Vector2(136, -16);
    [SerializeField] private RectTransform playerRectTransform;

    [SerializeField] private RectTransform mapImageTransform;

    [SerializeField] private RectTransform missionRectTransform;

    private Vector3 missionTargetPosition;
    private string missionTargetName;

    private float currentScale = 1.35f;

    private float maxScale = 1.35f;

    private float minScale = 0.96f;

    int maskSizeX = 960;
    int maskSizeY = 540;
    public void SetPlayerTransform(Transform _player)
    {
        playerTransform = _player;
        UpdatePosition();
    }
    protected override void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.ChangeQuest, ChangeMissionPosition);
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.ChangeQuest, ChangeMissionPosition);
    }

    private void UpdatePosition()
    {
        Vector2 position = offSetMap - new Vector2(playerTransform.position.x, playerTransform.position.z);
        playerRectTransform.anchoredPosition = position;
    }
    public void SetMissionTargetPosition(Vector3 target_position)
    {
        missionTargetPosition = target_position;
        if (missionRectTransform != null)
            missionRectTransform.anchoredPosition = offSetMap - new Vector2(missionTargetPosition.x, missionTargetPosition.z);
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
    private void OnGUI()
    {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (currentScale < maxScale && scrollDelta.y > 0)
        {
            currentScale += scrollDelta.y * 0.01f;
        }
        else if(currentScale>minScale && scrollDelta.y < 0)
        {
            currentScale += scrollDelta.y * 0.01f;
        }
        mapImageTransform.localScale = currentScale * Vector3.one;
        ValidatePosition(mapImageTransform.anchoredPosition);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int width = Screen.width;
            int height = Screen.height;

            if (Input.mousePosition.x  < (width - maskSizeX) /2  || Input.mousePosition.x>(width+ maskSizeX) /2 ||
                Input.mousePosition.y > (height + maskSizeY) / 2|| Input.mousePosition.y > (height + maskSizeY) / 2)
            {
                return;
            }
            StartDragMap();
        }
        if (isDraging)
        {
            mousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(Screen.width, Screen.height) / 2;
            targetPosition = mousePoint + mouseOffSet;
            ValidatePosition(targetPosition);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopDragMap();
        }
    }
    private void ValidatePosition(Vector2 targetPosition)
    {
        if (targetPosition.x > (180 + 220/(maxScale - minScale) * (currentScale - minScale))){
            targetPosition = new Vector2((180 + 220 / (maxScale - minScale) * (currentScale - minScale)), targetPosition.y);
        }
        if (targetPosition.x < (-150 + 220 / (maxScale - minScale) * (maxScale-currentScale)))
        {
            targetPosition = new Vector2((-150 + 220 / (maxScale - minScale) * (maxScale - currentScale)), targetPosition.y);
        }
        if (targetPosition.y > (260 + 220 / (maxScale - minScale) * (currentScale - minScale)))
        {
            targetPosition = new Vector2(targetPosition.x, (260 + 220 / (maxScale - minScale) * (currentScale - minScale)));
        }
        if (targetPosition.y < (-480 + 220 / (maxScale - minScale) * (maxScale - currentScale)))
        {
            targetPosition = new Vector2(targetPosition.x, (-480 + 220 / (maxScale - minScale) * (maxScale - currentScale)));
        }
        mapImageTransform.anchoredPosition = targetPosition;
    }
    private Vector2 mouseOffSet;
    private Vector2 mousePoint;
    private Vector2 targetPosition;
    private bool isDraging = false;

    private void StartDragMap()
    {
        isDraging = true;
        mousePoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - new Vector2(Screen.width, Screen.height) / 2;
        mouseOffSet = mapImageTransform.anchoredPosition - mousePoint;
    }
    private void StopDragMap()
    {
        isDraging = false;
    }
}
