using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SolarSystemCameraController : MonoBehaviour
{
    [Header("Target & Distance")]
    public Vector3 target = Vector3.zero;
    public float distance = 40f;
    public float minDistance = 5f;
    public float maxDistance = 100f;

    [Header("Speed Settings")]
    public float mouseSensitivity = 0.1f;
    public float zoomSpeed = 2f;
    
    [Header("Mobile Sensitivity")]
    public float touchSensitivity = 0.05f;
    public float touchZoomSensitivity = 0.01f;

    [Header("Pitch Limits")]
    public float minPitch = 5f;
    public float maxPitch = 85f;

    private float yaw = 0f;
    private float pitch = 30f;

    void Start()
    {
        // 1. Pastikan kamera menggunakan Skybox agar background angkasa 3D terlihat
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.Skybox;
        }

        // 2. Cari objek SolarSystemExplorer di scene untuk dijadikan target fokus
        SolarSystemExplorer explorer = FindFirstObjectByType<SolarSystemExplorer>();
        if (explorer != null)
        {
            target = explorer.transform.position;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(angles.x, minPitch, maxPitch);
        
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        // Selalu perbarui target posisi seandainya SolarSystemExplorer berpindah tempat
        SolarSystemExplorer explorer = FindFirstObjectByType<SolarSystemExplorer>();
        if (explorer != null)
        {
            target = explorer.transform.position;
        }

        // --- INPUT SYSTEM BARU (NEW INPUT SYSTEM) ---
        
        // 1. Mouse Input
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                yaw += mouseDelta.x * mouseSensitivity;
                pitch -= mouseDelta.y * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();
            if (Mathf.Abs(scrollDelta.y) > 0.01f)
            {
                float scrollAmount = scrollDelta.y * 0.01f;
                distance = Mathf.Clamp(distance - scrollAmount * zoomSpeed, minDistance, maxDistance);
            }
        }

        // 2. Touch Input (Untuk Perangkat Mobile)
        if (Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;
            List<UnityEngine.InputSystem.Controls.TouchControl> activeTouches = new List<UnityEngine.InputSystem.Controls.TouchControl>();
            
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.isPressed)
                {
                    activeTouches.Add(touches[i]);
                }
            }

            if (activeTouches.Count == 1)
            {
                Vector2 touchDelta = activeTouches[0].delta.ReadValue();
                yaw += touchDelta.x * touchSensitivity;
                pitch -= touchDelta.y * touchSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
            else if (activeTouches.Count == 2)
            {
                var touchZero = activeTouches[0];
                var touchOne = activeTouches[1];

                Vector2 touchZeroPos = touchZero.position.ReadValue();
                Vector2 touchOnePos = touchOne.position.ReadValue();
                Vector2 touchZeroDelta = touchZero.delta.ReadValue();
                Vector2 touchOneDelta = touchOne.delta.ReadValue();

                Vector2 touchZeroPrevPos = touchZeroPos - touchZeroDelta;
                Vector2 touchOnePrevPos = touchOnePos - touchOneDelta;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZeroPos - touchOnePos).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                distance = Mathf.Clamp(distance + deltaMagnitudeDiff * touchZoomSensitivity, minDistance, maxDistance);
            }
        }

        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 position = target - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;
    }
}
