using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void TriggerMuzzleFlash()
    {
        if (firingSprite == null || spriteRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        spriteRenderer.sprite = firingSprite;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.sprite = idleSprite;
        flashCoroutine = null;
    }
}