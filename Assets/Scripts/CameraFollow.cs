using UnityEngine;

// Follows the player while keeping the camera view inside the map.
// Assumes the map is centered at world position (0, 0).
public class CameraFollow : MonoBehaviour
{
    // Drag Player into this field in the Inspector.
    [SerializeField] private Transform target;

    // The map's width and height in world units.
    [SerializeField] private Vector2 mapSize = new Vector2(40f, 30f);

    private Camera cam;

    void Start()
    {
        // Get the Camera component on this GameObject.
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate how much of the world the camera can see
        // on each side of its center.
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // Calculate how far the camera center can move.
        // If the view is larger than the map, keep it centered.
        float limitX = Mathf.Max(0f, mapSize.x / 2f - halfWidth);
        float limitY = Mathf.Max(0f, mapSize.y / 2f - halfHeight);

        // Follow the target without moving beyond these limits.
        float x = Mathf.Clamp(target.position.x, -limitX, limitX);
        float y = Mathf.Clamp(target.position.y, -limitY, limitY);

        transform.position = new Vector3(x, y, -10f);
    }
}