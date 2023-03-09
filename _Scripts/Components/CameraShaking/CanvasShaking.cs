using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public struct CanvasShakingInfo
    {
        public float shakeAmount;
        public float shakeTime;
    }
    public class CanvasShaking : MonoBehaviour
    {
        public float shakeAmount = 5f;

        private float shakeTime = 0.0f;
        private Vector3 initialPosition;
        private bool isScreenShaking = false;

        //Code adapted from Camera Vibration in Canvas Based Unity Game · Yuno's Wonderland

        // Use this for initialization
        void Start ()
        {
            initialPosition = this.transform.position;
            Observer.Instance.AddObserver(ObserverKey.CanvasShaking, StartShaking);
        }

        private void OnDestroy()
        {
            Observer.Instance.RemoveObserver(ObserverKey.CanvasShaking, StartShaking);
        }

        public void StartShaking(object data)
        {
            if(data == null) return;
            CanvasShakingInfo canvasShakingInfo = (CanvasShakingInfo)data;
            ScreenShakeForTime(canvasShakingInfo);
        }
        // Update is called once per frame
        void Update () {
            if(shakeTime > 0)
            {
                this.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
                shakeTime -= Time.deltaTime;
            }
            else if(isScreenShaking)
            {
                isScreenShaking = false;
                shakeTime = 0.0f;
                this.transform.position = initialPosition;

            }
        }

        public void ScreenShakeForTime(CanvasShakingInfo canvasShakingInfo)
        {
            shakeTime = canvasShakingInfo.shakeTime;
            shakeAmount = canvasShakingInfo.shakeAmount;
            isScreenShaking = true;
        }

        public static CanvasShakingInfo CreateCanvasShakingInfo(float shakeTime, float shakeAmount = 3f)
        {
            CanvasShakingInfo canvasShakingInfo = new CanvasShakingInfo();
            canvasShakingInfo.shakeTime = shakeTime;
            canvasShakingInfo.shakeAmount = shakeAmount;
            return canvasShakingInfo;
        }
    }
}