using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("TutorialFirstPlay", "PopupCanvas")]

public class TutorialFirstPlay : MonoBehaviour
{

    [SerializeField] private GameObject ob_TutorialUI;
    [SerializeField] private GameObject ob_Step1, ob_Step2, ob_Step3, ob_Step4;
    [SerializeField] private Image img_A, img_W, img_D, img_S;
    [SerializeField] private Image img_Space;
    [SerializeField] private Image img_Alt;
    [SerializeField] private Image img_Shift;
    private int current_Step = 0;
    private Action actionDone;


    private void SetActiveGameObject(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }
    private void SetActiveImg(Image image, bool active)
    {
        if (image != null)
            image.gameObject.SetActive(active);
    }

    private void DefaultActive()
    {
        if (ob_Step1 != null && ob_Step2 != null && ob_Step3 != null && ob_Step4 != null)
        {
            ob_Step1.gameObject.SetActive(true);
            ob_Step2.gameObject.SetActive(false);
            ob_Step3.gameObject.SetActive(false);
            ob_Step4.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        DefaultActive();
    }
    private void StepDone()
    {
        actionDone?.Invoke();
    }
    public void StateStepTutorial(int step)
    {
        switch (step)
        {
            case 0:
                {
                    Step1();
                    break;
                }
            case 1:
                {
                    Step2();
                    break;
                }
            case 2:
                {
                    Step3();
                    break;
                }
            case 3:
                {
                    Step4();
                    break;
                }
            case 4:
                {
                    SubmitDoneTutorial();
                    break;
                }
        }
    }
    private void Step1()
    {
        actionDone = () => { StateStepTutorial(1); };
        SetActiveGameObject(ob_Step1, true);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.W, "KeyW", KeyW, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.A, "KeyA", KeyA, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.S, "KeyS", KeyS, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.D, "KeyD", KeyD, ActionKeyType.Up);
    }
    private int count_key_step_one = 0;
    private void KeyW()
    {
        SetActiveImg(img_W, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.W, "KeyW", KeyW, ActionKeyType.Up);
        count_key_step_one++;
        if (count_key_step_one != 4) return;
        else StepOneDone();
    }
    private void KeyA()
    {
        SetActiveImg(img_A, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.A, "KeyA", KeyA, ActionKeyType.Up);

        count_key_step_one++;
        if (count_key_step_one != 4) return;
        else StepOneDone();
    }
    private void KeyS()
    {
        SetActiveImg(img_S, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.S, "KeyS", KeyS, ActionKeyType.Up);
        count_key_step_one++;
        if (count_key_step_one != 4) return;
        else StepOneDone();
    }
    private void KeyD()
    {
        SetActiveImg(img_D, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.D, "KeyD", KeyD, ActionKeyType.Up);
        count_key_step_one++;
        if (count_key_step_one != 4) return;
        else StepOneDone();
    }

    private void StepOneDone()
    {
        SetActiveGameObject(ob_Step1, false);
        StepDone();
    }

    private void Step2()
    {
        SetActiveGameObject(ob_Step2, true);
        actionDone = () => { StateStepTutorial(2); };
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Space, "KeySpaceUp", KeySpaceUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Space, "KeySpaceDown", KeySpaceDown, ActionKeyType.Down);
    }

    private void KeySpaceUp()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Space, "KeySpaceUp", KeySpaceUp, ActionKeyType.Up);
        SetActiveGameObject(ob_Step2, false);
        StepDone();
    }
    private void KeySpaceDown()
    {
        SetActiveImg(img_Space, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Space, "KeySpaceDown", KeySpaceDown, ActionKeyType.Up);
    }

    private void Step3()
    {
        SetActiveGameObject(ob_Step3, true);
        actionDone = () => { StateStepTutorial(3); };
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "ShiftUp", ShiftUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.RightShift, "ShiftUp", ShiftUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "ShiftDown", ShiftDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.RightShift, "ShiftDown", ShiftDown, ActionKeyType.Down);
    }

    private void ShiftDown()
    {
        SetActiveImg(img_Shift, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "ShiftDown", ShiftDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.RightShift, "ShiftDown", ShiftDown, ActionKeyType.Down);
    }
    private void ShiftUp()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "ShiftUp", ShiftUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.RightShift, "ShiftUp", ShiftUp, ActionKeyType.Up);
        SetActiveGameObject(ob_Step3, false);
        StepDone();
    }

    private void Step4()
    {
        SetActiveGameObject(ob_Step4, true);
        actionDone = () => { StateStepTutorial(4); };
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "AltUp", AltUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.RightAlt, "AltUp", AltUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftAlt, "AltDown", AltDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.RightAlt, "AltDown", AltDown, ActionKeyType.Down);
    }
    private void AltUp()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "AltUp", AltUp, ActionKeyType.Up);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.RightAlt, "AltUp", AltUp, ActionKeyType.Up);
        SetActiveGameObject(ob_Step4, false);
        StepDone();
    }
    private void AltDown()
    {
        SetActiveImg(img_Alt, true);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftAlt, "AltDown", AltDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.RightAlt, "AltDown", AltDown, ActionKeyType.Down);
    }

    public KeyControl DoneTutorial()
    {
        return PanelManager.Show<KeyControl>();
    }

    public void DestroyTutorialGameObject()
    {
        PanelManager.Hide<TutorialFirstPlay>(true);
    }
    private void SubmitDoneTutorial()
    {
        string token = UserDatas.token;
        string address = UserDatas.user_Data.info.address;
        TPRLAPI.instance.SubmitTutorialResponse(token, address, (done) =>
        {
            TPRLAPI.instance.GetUserProgress(token, address, (data) =>
            {
                Debug.LogError("data" + data);
                UserProgressResponse myDeserializedClass = JsonUtility.FromJson<UserProgressResponse>(data);
                if (myDeserializedClass.output != null)
                {
                    UserDatas.user_Data.info.is_tutorial_done = myDeserializedClass.output.is_tutorial_completed;
                    JSONObject dataObj = JSON.Parse(data).AsObject;
                    string main = "";
                    if (dataObj["output"]["main"] != null)
                        main = dataObj["output"]["main"].ToString();
                    if (string.IsNullOrEmpty(main))
                    {
                        UserDatas.user_Data.info.open_mission_daily = true;
                        UserDatas.user_Data.info.current_id_main_mission = -1;
                    }
                    else
                    {
                        UserDatas.user_Data.info.current_id_main_mission = myDeserializedClass.output.main.mission_id;
                    }

                    if (myDeserializedClass.output.daily != null)
                    {
                        int lengthDaily = myDeserializedClass.output.daily.Length;
                        UserDatas.missionDailyInfo = new RecordMissionDailyInfo[lengthDaily];
                        UserDatas.missionDailyInfoUsed = new RecordMissionDailyInfo[lengthDaily];
                        if (lengthDaily != 0)
                        {
                            for (int i = 0; i < lengthDaily; i++)
                            {
                                UserDatas.missionDailyInfo[i].mission_id = myDeserializedClass.output.daily[i].mission_id;
                                UserDatas.missionDailyInfo[i].mission_name = "Daily Mission";
                                UserDatas.missionDailyInfo[i].is_reward_received = myDeserializedClass.output.daily[i].is_reward_received;
                                if (UserDatas.missionDailyInfo[i].is_reward_received)
                                {
                                    UserDatas.missionDailyInfoUsed[i].mission_id = myDeserializedClass.output.daily[i].mission_id;
                                    UserDatas.missionDailyInfoUsed[i].mission_name = "Daily Mission";
                                    UserDatas.missionDailyInfoUsed[i].is_reward_received = myDeserializedClass.output.daily[i].is_reward_received;
                                }
                            }
                        }
                    }
                }
                Observer.Instance.Notify(ObserverKey.SetActiveMissionMain);
            });
        });
        UserDatas.user_Data.info.is_tutorial_done = true;
        DestroyTutorialGameObject();

    }
}