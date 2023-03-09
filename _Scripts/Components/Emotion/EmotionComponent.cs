using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmotionComponent : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityManager entityManager
    {
        get
        {
            if (_entityManager == null)
                _entityManager = GetComponent<EntityManager>();
            return _entityManager;
        }
    }
    private Animator _animator;
    private Animator animator
    {
        get
        {
            if (_animator == null && entityManager != null)
                _animator = entityManager.animator;
            return _animator;
        }
    }
    private void RegisterEvent()
    {
        if (_isPlayer)
        {
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.T, "PlayEmotion", PlayEmotion, ActionKeyType.Up);
        }
    }
    private void OnDestroy()
    {
        if (!_isPlayer) return;
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.T, "PlayEmotion", PlayEmotion, ActionKeyType.Up);
    }
    private ObscuredBool _isPlayer;
    public void SetUpBehaviour(ObscuredBool isPlayer)
    {
        this._isPlayer = isPlayer;
        RegisterEvent();
    }
    private void PlayEmotion()
    {
        if (!(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name is "Walking" or "Running"))
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResult);

            string emotion_name = "";
            foreach (var raycast in raycastResult)
            {
                switch (raycast.gameObject.name)
                {
                    case "Emotion 1":
                        {
                            emotion_name = "Hello";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION1;
                            break;
                        }
                    case "Emotion 2":
                        {
                            emotion_name = "Dizzy Idle";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION2;
                            break;
                        }
                    case "Emotion 3":
                        {
                            emotion_name = "Clap 2";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION3;
                            break;
                        }
                    case "Emotion 4":
                        {
                            emotion_name = "Victory";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION4;
                            break;
                        }
                    case "Emotion 5":
                        {
                            emotion_name = "Dance";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION5;
                            break;
                        }
                    case "Emotion 6":
                        {
                            emotion_name = "Falling 1";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION6;
                            break;
                        }
                    case "Emotion 7":
                        {
                            emotion_name = "Sit Move";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION7;
                            break;
                        }
                    case "Emotion 8":
                        {
                            emotion_name = "Death";
                            entityManager.networkAnimationStatus = NetworkAnimationValue.EMOTION8;
                            break;
                        }
                }
            }
            animator.Play(emotion_name);
        }
        ClosePopupEmotion();
    }
    public void PlayNetworkEmotion(int value)
    {
        string emotion_name = "";
        switch (value)
        {
            case (int)NetworkAnimationValue.EMOTION1:
                {
                    emotion_name = "Hello";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION2:
                {
                    emotion_name = "Dizzy Idle";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION3:
                {
                    emotion_name = "Clap 2";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION4:
                {
                    emotion_name = "Victory";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION5:
                {
                    emotion_name = "Dance";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION6:
                {
                    emotion_name = "Falling 1";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION7:
                {
                    emotion_name = "Sit Move";
                    break;
                }
            case (int)NetworkAnimationValue.EMOTION8:
                {
                    emotion_name = "Death";
                    break;
                }
        }
        animator.Play(emotion_name);
    }

    private void ClosePopupEmotion()
    {
        Ultis.SetActiveCursor(false);
        GameConfig.gameBlockInput = false;
        PanelManager.Hide<PopupEmotion>();
    }
}
