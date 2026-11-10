using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Target : MonoBehaviour, IPointerDownHandler
{
    // Constants to eliminate magic numbers
    private const float DEFAULT_SHATTER_DISPLAY_TIME = 0.2f;

    [Header("Target Settings")]
    [Tooltip("Check true for GreenTarget (penalty), false for RedTarget (score).")]
    [SerializeField] private bool isPenaltyTarget = false;
    [SerializeField] private float shatterDisplayTime = DEFAULT_SHATTER_DISPLAY_TIME;

    [Header("Visual Feedback")]
    [Tooltip("Assign RedBrokenTarget sprite here for RedTarget prefab.")]
    [SerializeField] private Sprite brokenSprite;

    private SpriteRenderer spriteRenderer;
    private Collider2D targetCollider;
    private bool isHit = false;

    public bool IsPenaltyTarget => isPenaltyTarget;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetCollider = GetComponent<Collider2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ProcessHit();
    }

    private void ProcessHit()
    {
        if (isHit) return;
        isHit = true;

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        if (isPenaltyTarget)
        {
            TargetEvents.TriggerPenaltyHit();
        }
        else
        {
            TargetEvents.TriggerScoreHit();
        }

        if (brokenSprite != null && spriteRenderer != null)
        {
            StartCoroutine(ShatterRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShatterRoutine()
    {
        spriteRenderer.sprite = brokenSprite;
        yield return new WaitForSeconds(shatterDisplayTime);
        Destroy(gameObject);
    }
}