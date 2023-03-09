using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Time;
using UnityEngine;

public class SlotmachineColumn : MonoBehaviour
{
    private ObscuredFloat speed = 5.5f;
    Vector3 startPositionSpin = Vector3.zero;
    Vector3 endPositionSpin = Vector3.zero;
    private ObscuredBool isSpining = false;
    private Dictionary<string, float> lstPositions = new Dictionary<string, float>();
    [SerializeField] private SlotmachineColDetectItem slotmachineColDetectItem;
    private ObscuredString result = "";
    private Action actionDone = null;

    private void Awake()
    {
        if (slotmachineColDetectItem != null)
            slotmachineColDetectItem.actionDetect = OnDetectItem;
    }

    public void AddPosition(string key, float position_y)
    {
        if (string.IsNullOrEmpty(key) || lstPositions.ContainsKey(key)) return;
        lstPositions.Add(key, position_y);
    }

    public void SetEndPositionSpin(float y)
    {
        endPositionSpin = new Vector3(0, y, 0);
    }

    public void SetSpin(bool spin)
    {
        isSpining = spin;
        if (spin == false)
            ResetPosition();
    }

    private void Spin()
    {
        var step = speed * SpeedHackProofTime.deltaTime * 1000f;
        Vector3 current_position = transform.localPosition;
        current_position = Vector3.MoveTowards(current_position, endPositionSpin, step);
        transform.localPosition = current_position;
        if (Vector3.Distance(transform.localPosition, endPositionSpin) < 0.001f)
        {
            ResetPosition();
        }
    }


    public void ShowResult(int index, Action action_done)
    {
        result = ((SlotmachineItemType)index).ToString();
        actionDone = action_done;
        StartDetectItem(true);
    }

    private void OnDetectItem(string item_name)
    {
        if (string.IsNullOrEmpty(item_name) || !item_name.Contains(result)) return;
        StartDetectItem(false);
        float stop_position_y = 0;
        if (lstPositions.ContainsKey(item_name))
            stop_position_y = lstPositions[item_name];
        isSpining = false;
        LeanTween.moveLocalY(gameObject, -stop_position_y, 0.5f).setEase(LeanTweenType.easeOutElastic).setOnComplete(actionDone);
    }

    private void StartDetectItem(bool detect)
    {
        if (slotmachineColDetectItem != null)
            slotmachineColDetectItem.isStartDetect = detect;
    }

    private void FixedUpdate()
    {
        if (isSpining)
            Spin();
    }

    private void ResetPosition()
    {
        transform.localPosition = startPositionSpin;
    }
}
