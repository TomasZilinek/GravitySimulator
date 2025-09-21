using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vCam;          // Assign in Inspector
    public InputActionReference zoomAction; // Assign in Inspector

    public float panSpeed = 20f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 100f;

    private Vector2 panInput;

    void Update()
    {
        float zoomInput = zoomAction.action.ReadValue<float>();
        if (Mathf.Abs(zoomInput) > 0.01f)
        {
            ZoomTowardsMouse(zoomInput);
        }

        // Optional panning
        // transform.position += (Vector3)panInput * panSpeed * Time.deltaTime;
    }

    private void ZoomTowardsMouse(float zoomInput)
    {
        Camera cam = Camera.main;

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
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 offset = new Vector3(
            (mouseWorld.x - transform.position.x) * (1 - newSize / oldSize),
            (mouseWorld.y - transform.position.y) * (1 - newSize / oldSize),
            0f
        );

        transform.position += offset;
    }
}
