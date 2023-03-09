using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerByMouse : MonoBehaviour
{
    private void Awake()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Mouse0, "StartRollMouse", StartRollMouse, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Mouse0, "StayRollMouse", StayRollMouse, ActionKeyType.Stay);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.Mouse0, "EndRollMouse", EndRollMouse, ActionKeyType.Up);
    }

    private void OnDestroy()
    {
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Mouse0, "StartRollMouse", StartRollMouse, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Mouse0, "StayRollMouse", StayRollMouse, ActionKeyType.Stay);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.Mouse0, "EndRollMouse", EndRollMouse, ActionKeyType.Up);
    }


    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        
    }

    private bool _checkPress = false;
    private Vector2 beginClick;
    
    private void StartRollMouse()
    {
        beginClick = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    private void EndRollMouse()
    {
        _checkPress = false;
        _aTimeStartCor = true;
        
    }

    private bool _aTimeStartCor = true;
    private void StayRollMouse()
    {
        if (beginClick.x > 1220 * Screen.width / 1920 && beginClick.x < Screen.width)
        {

            if (_aTimeStartCor)
            {
                _aTimeStartCor = false;
                StartCoroutine(Press());
            }

            if (_checkPress)
            {
                RotateModel();
            }
        }

    }
    IEnumerator Press()
    {
        yield return new WaitForSeconds(0.1f);
        _checkPress = true;
    }
    private void RotateModel()
    {
        
        float _distance = (beginClick.x - Input.mousePosition.x)/2;
        transform.eulerAngles = new Vector3(0, _distance, 0);
        
    }
}


