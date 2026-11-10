using UnityEngine;


// Keeps objects at the same pixel size on the same device
// when switching between landscape and portrait.
[RequireComponent(typeof(Camera))]
public class CameraOrientationSize : MonoBehaviour
{
    // The camera's desired Size when the screen is in landscape.
    [SerializeField] private float landscapeSize = 7f;

    private Camera cam;

    void Awake()
    {
        // Get the Camera component on this GameObject.
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Screen dimensions are measured in pixels.
        // The shorter side remains the same when the device rotates.
        float shortSide = Mathf.Min(Screen.width, Screen.height);

        if (shortSide <= 0f)
            return;

        // In landscape, the ratio is 1, so Size equals landscapeSize.
        // In portrait, the taller screen requires a larger Size.
        // This keeps the number of pixels per world unit consistent.
        float heightRatio = Screen.height / shortSide;

        cam.orthographicSize = landscapeSize * heightRatio;
    }
}