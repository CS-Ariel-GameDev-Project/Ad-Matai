using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera;

    [Header("Background Settings")]
    [SerializeField] private bool centerOnCamera = true;

    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        FitBackground();
    }

    private void LateUpdate()
    {
        FitBackground();
    }

    private void FitBackground()
    {
        if (spriteRenderer == null)
            return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null ||
            !targetCamera.orthographic ||
            spriteRenderer.sprite == null)
            return;

        // Visible camera dimensions in world units
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * targetCamera.aspect;

        // Original sprite dimensions in world units
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Scale uniformly to cover the entire screen
        float scale = Mathf.Max(
            cameraWidth / spriteSize.x,
            cameraHeight / spriteSize.y
        );

        transform.localScale = new Vector3(scale, scale, 1f);

        // Center background without changing its depth
        if (centerOnCamera)
        {
            transform.position = new Vector3(
                targetCamera.transform.position.x,
                targetCamera.transform.position.y,
                transform.position.z
            );
        }
    }
}