using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class InteractComponent : MonoBehaviour
{
    private ObscuredString hitObName = "";
    private ResponseInteraction [] currentResponseInteraction = null;
    private ResponseInteraction chooseResponseInteraction = null;

    private ObscuredBool isMe = true;

    private void RegisterEvent()
    {
        if (isMe)
            InputRegisterEvent.Instance.RegisterEvent(KeyCode.F, "Interact", Interact, ActionKeyType.Down);
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.CurrentChooseInteract, ChooseResponseInteraction);
        if (isMe)
            InputRegisterEvent.Instance.RemoveEventKey(KeyCode.F, "Interact", Interact, ActionKeyType.Down);
    }

    public void UpdateInfo()
    {

    }

    private void FixedUpdate()
    {
        if (!isMe) return;
        CheckHit();
    }

    private void CheckHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        string hit_name;
        if (Physics.Raycast(ray, out hit, 8f))
        {
            hit_name = hit.transform.name;
            if (!hitObName.Equals(hit_name))
            {
                if (!string.IsNullOrEmpty(hit_name))
                {
                    currentResponseInteraction = hit.transform.gameObject.GetComponents<ResponseInteraction>();
                    if (currentResponseInteraction.Length>=1)
                    {
                        Observer.Instance.Notify(ObserverKey.RayCastHitObject, currentResponseInteraction);
                        hitObName = hit.transform.name;
                    }

                }
            }
        }
        else
        {
            hit_name = "";
            if (!string.IsNullOrEmpty(hitObName))
            {
                Observer.Instance.Notify(ObserverKey.RayCastHitObject, null);
                hitObName = hit_name;
            }
        }
    }
    public void SetUpBehaviour(ObscuredBool is_me)
    {
        this.isMe = is_me;
        RegisterEvent();
    }
    private void ChooseResponseInteraction(object data)
    {
        chooseResponseInteraction = (ResponseInteraction)data;
    }
    private void Interact()
    {
        if (currentResponseInteraction == null) return;
        GameObject ob = new GameObject();
        InteractionManager interaction_manager = ob.AddComponent<InteractionManager>();
        interaction_manager.Init(gameObject, chooseResponseInteraction);
    }
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.CurrentChooseInteract, ChooseResponseInteraction);
    }

}
