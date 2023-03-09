using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class VehicleMove : MonoBehaviour
{
    [SerializeField] private GameObject ob_ParticleMove;
    [SerializeField] private GameObject ob_ParticleWind;
    [SerializeField] private Volume volume_MaxSpeed;
    [SerializeField] private float time_ZoomIn;
    [SerializeField] private float time_ZoomOut;

    private VolumeProfile volume_Profile;
    [SerializeField] private float booster_Border = 0.4f;
    private Vector3 input_User;
    [SerializeField] private Rigidbody rb_Vehicle;
    [SerializeField] private float current_Speed;
    private float velocity_MaxNormal = 20;
    private float velocity_Change = 0;
    [SerializeField] private float velocity_Max = 30;
    [SerializeField] private GameObject[] ob_Cam;
    private bool is_Run = false;
    private float current_Angle = 0;
    [SerializeField] private float max_Angle = 50;
    private int current_IndexCam;
    private int previous_IndexCam;

    private bool is_Sprint = false;
    private void Awake()
    {
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "SprintDown", SprintDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RegisterEvent(KeyCode.LeftShift, "SprintUp", SprintUp, ActionKeyType.Up);
    }
    private void Start()
    {
        if (volume_MaxSpeed != null)
        {
            volume_Profile = volume_MaxSpeed.sharedProfile;
        }
        velocity_Change = velocity_MaxNormal;
        current_Angle = transform.rotation.eulerAngles.y;
        if (ob_Cam != null && ob_Cam.Length > 0)
        {
            current_IndexCam = 0;
            previous_IndexCam = current_IndexCam;
            ob_Cam[0].SetActive(true);
            ob_Cam[1].SetActive(false);
            ob_Cam[2].SetActive(false);
        }
    }
    private void OnDestroy()
    {

        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "SprintDown", SprintDown, ActionKeyType.Down);
        InputRegisterEvent.Instance.RemoveEventKey(KeyCode.LeftShift, "SprintUp", SprintUp, ActionKeyType.Up);
    }
    private void SprintDown()
    {
        velocity_Change = velocity_Max;
        is_Sprint = true;
    }
    private void SprintUp()
    {
        velocity_Change = velocity_MaxNormal;
        is_Sprint = false;
    }
    private void SetActiveCamByMovement(int index)
    {
        if (ob_Cam.Length > 0 && ob_Cam != null)
        {
            ob_Cam[previous_IndexCam].SetActive(false);
            ob_Cam[index].SetActive(true);
            previous_IndexCam = current_IndexCam;
            current_IndexCam = index;
        }
    }
    public void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        input_User = new Vector3(input.x, 0, input.y);
    }
    private void FixedUpdate()
    {
        Move(input_User.z);
        Rotate(input_User.x, input_User.z);
    }
    private void Move(float z_axis)
    {
        if (Mathf.Abs(z_axis) < 0.01f)
        {
            FadeVolumeComponent(time_ZoomIn, false);
            SetActiveParticle(ob_ParticleMove, false);
            SetActiveParticle(ob_ParticleWind, false);
            SetActiveCamByMovement(0);
            is_Run = false;
        }
        if (Mathf.Abs(z_axis) > 0.2f)
        {
            is_Run = true;
            if (rb_Vehicle.velocity.magnitude > velocity_Change)
                return;
            else
            {
                if (is_Sprint)
                {
                    SetActiveParticle(ob_ParticleMove, true);
                    FadeVolumeComponent(time_ZoomOut, true);
                    SetActiveCamByMovement(2);
                    SetActiveParticle(ob_ParticleWind, true);
                }
                else
                {
                    SetActiveParticle(ob_ParticleMove, false);
                    SetActiveParticle(ob_ParticleWind, false);
                    FadeVolumeComponent(time_ZoomIn, false);
                    SetActiveCamByMovement(1);
                }
                rb_Vehicle.AddForce(current_Speed * z_axis * 2 * Time.deltaTime * transform.forward, ForceMode.Impulse);
            }
        }
    }

    private void Rotate(float x_axis, float z_axis)
    {
        if (is_Run && x_axis != 0 && Mathf.Abs(z_axis) > 0.2f)
        {
            current_Angle += max_Angle * Time.deltaTime * x_axis;
            transform.localRotation = Quaternion.Euler(Vector3.up * current_Angle);
        }
    }

    private void SetActiveParticle(GameObject ob_particle, bool active)
    {
        if (ob_particle != null)
            ob_particle.SetActive(active);
    }
    private void FadeVolumeComponent(float time, bool is_max_speed)
    {
        if (volume_Profile != null)
        {
            UnityEngine.Rendering.Universal.Vignette vignette;
            UnityEngine.Rendering.Universal.ChromaticAberration chromatic;
            if (!volume_Profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out vignette))
            {
                vignette = volume_Profile.Add<UnityEngine.Rendering.Universal.Vignette>();
            }
            if (!volume_Profile.TryGet<UnityEngine.Rendering.Universal.ChromaticAberration>(out chromatic))
            {
                chromatic = volume_Profile.Add<UnityEngine.Rendering.Universal.ChromaticAberration>();
            }

            float intensity_vignette = vignette.intensity.value;
            float smoothness_vignette = vignette.smoothness.value;
            float intensity_chromatic = chromatic.intensity.value;

            if (!is_max_speed)
            {
                vignette.intensity.value = Mathf.Lerp(intensity_vignette, 0, time * Time.deltaTime);
                vignette.smoothness.value = Mathf.Lerp(smoothness_vignette, 0, time * Time.deltaTime);
                chromatic.intensity.value = Mathf.Lerp(intensity_chromatic, 0, time * Time.deltaTime);
            }
            else
            {
                vignette.intensity.value = Mathf.Lerp(intensity_vignette, booster_Border, time * Time.deltaTime);
                vignette.smoothness.value = Mathf.Lerp(smoothness_vignette, 0.3f, time * Time.deltaTime);
                chromatic.intensity.value = Mathf.Lerp(intensity_chromatic, 0.15f, time * Time.deltaTime);

            }
        }
    }
}
