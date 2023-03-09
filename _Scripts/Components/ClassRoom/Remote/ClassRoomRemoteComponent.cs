using System.Collections;
using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class ClassRoomRemoteComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject btPlay;
    private ClassRoomMediaType currentMediaType = ClassRoomMediaType.video;


    // Start is called before the first frame update
    void Start()
    {
        //ResetDefault();
    }

    private void ResetDefault()
    {
        SetActive(btPlay, true);
    }

    public void UpdateEvents(ClassRoomMediaType type, MapSchema<ProjectorModeState> state)
    {
        currentMediaType = type;
        SetActive(textCurrentPageSlide, currentMediaType == ClassRoomMediaType.slide);
        if (currentMediaType == ClassRoomMediaType.slide)
        {
            SetActive(btPlay, false);
        }
        else
        {
            state.ForEach((key, value) =>
            {
                if (value.mode.ToLower().Equals(ClassRoomMediaType.video.ToString()))
                {
                    InteractRemote((ClassRoomRemoteInteactionType)value.page_state);
                }
            });
        }
        
    }

    public void InteractRemote(ClassRoomRemoteInteactionType type)
    {
        switch (type)
        {
            case ClassRoomRemoteInteactionType.Play:
                {
                    SetActive(btPlay, false);
                    InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, "RemotePlay", RemotePlay, ActionKeyType.Down);
                    InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, "RemotePause", RemotePause, ActionKeyType.Down);
                    break;
                }

            case ClassRoomRemoteInteactionType.Pause:
                {
                    SetActive(btPlay, true);
                    InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, "RemotePause", RemotePause, ActionKeyType.Down);
                    InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, "RemotePlay", RemotePlay, ActionKeyType.Down);
                    break;
                }
 
            case ClassRoomRemoteInteactionType.Next:
                break;
            case ClassRoomRemoteInteactionType.Back:
                break;
            case ClassRoomRemoteInteactionType.Reset:
                SetActive(btPlay, false);
                break;
            default:
                break;
        }
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }
    [SerializeField] private GameObject textCurrentPageSlide;

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.ClassRoomUpdateUI, RegisterKeyRemoteTeacher);
    }

    private void OnDestroy()
    {
        UnregisterKeyIfTeacher();
        Observer.Instance.RemoveObserver(ObserverKey.ClassRoomUpdateUI, RegisterKeyRemoteTeacher);
    }

    private void RegisterKeyRemoteTeacher(object data)
    {
        if (data != null)
        {
            ClassRoomRole currentRole = (ClassRoomRole)data;
            if (currentRole == ClassRoomRole.teacher)
            {
                RegisterKeyIfTeacher();
            }

        }
    }
    private void RegisterKeyIfTeacher()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.P, "RemotePlay", RemotePlay, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Q, "RemoteBack", RemoteBack, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.E, "RemoteNext", RemoteNext, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.R, "RemoteReset", RemoteReset, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Alpha1, "RemoteSwitchVideo", RemoteSwitchVideo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Keypad1, "RemoteSwitchVideo", RemoteSwitchVideo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Alpha2, "RemoteSwitchSlide", RemoteSwitchSlide, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Keypad2, "RemoteSwitchSlide", RemoteSwitchSlide, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Alpha3, "EndQuiz", EndQuiz, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Keypad3, "EndQuiz", EndQuiz, ActionKeyType.Down);
    }

    private void UnregisterKeyIfTeacher()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, "RemotePause", RemotePause, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.P, "RemotePlay", RemotePlay, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Q, "RemoteBack", RemoteBack, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.E, "RemoteNext", RemoteNext, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.R, "RemoteReset", RemoteReset, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Alpha1, "RemoteSwitchVideo", RemoteSwitchVideo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Keypad1, "RemoteSwitchVideo", RemoteSwitchVideo, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Alpha2, "RemoteSwitchSlide", RemoteSwitchSlide, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Keypad2, "RemoteSwitchSlide", RemoteSwitchSlide, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Alpha3, "EndQuiz", EndQuiz, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Keypad3, "EndQuiz", EndQuiz, ActionKeyType.Down);
    }

    private void RemotePause()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Pause);
    }
    private void RemoteBack()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Back);
    }
    private void RemoteNext()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Next);
    }
    private void RemoteReset()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Reset);
        Observer.Instance.Notify(ObserverKey.ClassRoomResetVideo);
    }
    private void RemotePlay()
    {
        NetworkingManager.NetSend(EventName.INTERACT_REMOTE, ClassRoomRemoteInteactionType.Play);
    }
    private void RemoteSwitchSlide()
    {
        NetworkingManager.NetSend(EventName.SWITCH_PROJECTOR_MODE, ClassRoomMediaType.slide.ToString());
    }
    private void RemoteSwitchVideo()
    {
        NetworkingManager.NetSend(EventName.SWITCH_PROJECTOR_MODE, ClassRoomMediaType.video.ToString());
    }
    private void EndQuiz()
    {
        NetworkingManager.NetSend(EventName.END_QUIZ, 1);
    }
}
