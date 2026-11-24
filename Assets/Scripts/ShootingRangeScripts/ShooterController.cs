using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Plays the muzzle flash for mouse clicks and touchscreen presses.
[RequireComponent(typeof(SpriteRenderer))]
public class ShooterController : MonoBehaviour
{
    // Constants to eliminate magic numbers
    private const float DEFAULT_FLASH_DURATION = 0.08f;

    [Header("Shooter Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite firingSprite;

    [Header("Flash Duration")]
    [SerializeField] private float flashDuration = DEFAULT_FLASH_DURATION;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void Update()
    {
        // Detects mouse click or touch input directly via New Input System
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            TriggerMuzzleFlash();
        }
    }

    // Restart the flash on rapid presses so an earlier routine cannot cut it short.
    public void TriggerMuzzleFlash()
    {
        if (firingSprite == null || spriteRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(MuzzleFlashRoutine());
    }

    // Briefly swap to the firing pose before returning to the idle pose.
    private IEnumerator MuzzleFlashRoutine()
    {
        spriteRenderer.sprite = firingSprite;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.sprite = idleSprite;
        flashCoroutine = null;
    }
}