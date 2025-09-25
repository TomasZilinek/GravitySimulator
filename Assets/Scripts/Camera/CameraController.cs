using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Security.Cryptography;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vCam;          // Assign in Inspector
    public InputActionReference zoomAction; // Assign in Inspector

    public float panSpeed = 20f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 100f;

    private Camera mainCamera;
    private Vector3 dragOrigin;
    private Vector3 dragDifference;

    private bool isDragging;

    private Vector3 MousePosition => mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!isDragging)
        {
            return;
        }

        dragDifference = MousePosition - transform.position;
        transform.position = dragOrigin - dragDifference;
    }

    public void OnMouseZoom(InputAction.CallbackContext context)
    {
        float zoomInput = context.ReadValue<float>();
        
        ZoomTowardsMouse(zoomInput);
    }

    public void OnMouseDrag(InputAction.CallbackContext context)
    {
        isDragging = context.started || context.performed;

        if (context.started)
        {
            dragOrigin = MousePosition;
        }
    }

    private void ZoomTowardsMouse(float zoomInput)
    {
        // Mouse position in screen coordinates (0 to 1)
        Vector2 mouseNormalized = new Vector2(
            Mouse.current.position.ReadValue().x / Screen.width,
            Mouse.current.position.ReadValue().y / Screen.height
        );

        float oldSize = vCam.Lens.OrthographicSize;

        // Apply zoom
        float newSize = oldSize - zoomInput * zoomSpeed * Time.deltaTime;
        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        vCam.Lens.OrthographicSize = newSize;

        // Compute proportional offset
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 offset = new Vector3(
            (mouseWorld.x - transform.position.x) * (1 - newSize / oldSize),
            (mouseWorld.y - transform.position.y) * (1 - newSize / oldSize),
            0f
        );

        transform.position += offset;
    }
}
