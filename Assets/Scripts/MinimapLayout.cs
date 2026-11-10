using UnityEngine;

// Attach to MinimapCamera.
// Keeps the minimap at a fixed aspect ratio in the top-right corner.
[RequireComponent(typeof(Camera))]
public class MinimapLayout : MonoBehaviour
{
    // Minimap width as a fraction of the screen's shorter side.
    [SerializeField] private float sizeFraction = 0.35f;

    private Camera minimapCamera;

    void Awake()
    {
        minimapCamera = GetComponent<Camera>();
    }

    void Update()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (screenWidth <= 0f || screenHeight <= 0f)
            return;

        // Using the shorter side keeps the pixel size consistent
        // when the same device rotates.
        float shortSide = Mathf.Min(screenWidth, screenHeight);
        float width = shortSide * sizeFraction;

        // Our map is 40 units wide and 30 units tall: a 4:3 ratio.
        float height = width * 3f / 4f;
        float margin = shortSide * 0.02f;

        // Convert pixel dimensions into normalized viewport coordinates.
        minimapCamera.rect = new Rect(
            1f - (width + margin) / screenWidth,
            1f - (height + margin) / screenHeight,
            width / screenWidth,
            height / screenHeight
        );
    }
}