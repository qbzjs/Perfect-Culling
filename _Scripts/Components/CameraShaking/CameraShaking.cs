using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    [SerializeField]
    private Transform camTransform;


    // How long the object should shake for.
    private float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float shakeAmount = 0.7f;
    private float decreaseFactor = 1.0f;

    float camZ,camX;
    private bool isStartShaking = false;

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.CameraShaking, ShakingCamera);

    }

    private void ShakingCamera(object data)
    {
        CameraShakingInfo info = (CameraShakingInfo)data;
        camZ = camTransform.localPosition.z;
        camX = camTransform.localPosition.x;
        shakeDuration = info.shake_duration;
        shakeAmount = info.shake_amount;
        decreaseFactor = info.decrease_factor;
        isStartShaking = true;

    }

    void Update()
    {
        if (isStartShaking == false || GameConfig.gameState == GameState.Pause) return;
        Vector3 start;
        Vector3 end;
        if (shakeDuration > 0)
        {
            start = camTransform.localPosition;
            end = new Vector3(start.x + Random.insideUnitSphere.x * shakeAmount, start.y + Random.insideUnitSphere.y * shakeAmount * 2 / 3, camZ);
            start = Vector3.Lerp(start, end, 0.1f);
            camTransform.localPosition = start;
            shakeDuration -= Time.deltaTime * decreaseFactor * GameConfig.gameSpeed;
            return;
        }

        start = camTransform.localPosition;
        end = new Vector3(camX, 0, camZ);
        start = Vector3.Lerp(start, end, 0.05f);
        camTransform.localPosition = start;
        if((end - start).sqrMagnitude < 0.001f)
        {
            isStartShaking = false;
            camTransform.localPosition = new Vector3(camX, 0, camZ);
        }
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.CameraShaking, ShakingCamera);
    }
}
