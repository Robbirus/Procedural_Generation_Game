using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Camera sourceCamera;
    [SerializeField] private Transform target;

    [Header("Zoom")]
    [SerializeField] private CinemachineCamera cameraOffset;
    [SerializeField] private int zoomSensitivity;
    [SerializeField] private float2 zoomRange;
    [SerializeField] private float zoomSmoothTime;

    [Header("Rotation")]
    [SerializeField] private int rotationSensitivity;

    private float zoomValue;
    private float zoomVelocity;
    private Vector2 lastMousePosition;

    void Start()
    {
        zoomValue = cameraOffset.transform.localPosition.z;
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();
        HandleRotation();
    }

    void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            zoomValue -= scroll * zoomSensitivity * Time.deltaTime;
            zoomValue = Mathf.Clamp(zoomValue, zoomRange.x, zoomRange.y);
        }

        Vector3 pos = cameraOffset.transform.localPosition;
        pos.z = Mathf.SmoothDamp(pos.z, zoomValue, ref zoomVelocity, zoomSmoothTime);
        cameraOffset.transform.localPosition = pos;
    }

    void HandleDrag()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 currentMousePosition = Mouse.current.position.ReadValue();
            Vector2 delta = currentMousePosition - lastMousePosition;

            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * 0.01f;

            target.Translate(move, Space.Self);

            lastMousePosition = currentMousePosition;
        }
    }

    void HandleRotation()
    {
        float rotation = 0f;

        if (Keyboard.current.qKey.isPressed)
            rotation = -1;

        if (Keyboard.current.eKey.isPressed)
            rotation = 1;

        if (rotation != 0)
        {
            target.Rotate(Vector3.up * rotation * rotationSensitivity * Time.deltaTime, Space.World);
        }
    }
}
