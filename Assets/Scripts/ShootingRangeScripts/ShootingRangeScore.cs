using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ShootingRangeScore : MonoBehaviour
{
    // Constants to eliminate magic numbers
    private const int DEFAULT_POINTS_PER_RED_TARGET = 10;
    private const int DEFAULT_POINTS_DEDUCTED_PER_GREEN_TARGET = 5;
    private const int INITIAL_SCORE = 0;
    private const int MINIMUM_SCORE = 0;

    [Header("Score Settings")]
    [SerializeField] private int pointsPerRedTarget = DEFAULT_POINTS_PER_RED_TARGET;
    [SerializeField] private int pointsDeductedPerGreenTarget = DEFAULT_POINTS_DEDUCTED_PER_GREEN_TARGET;

    private TextMeshProUGUI scoreText;
    private int currentScore = INITIAL_SCORE;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        TargetEvents.OnScoreTargetHit += AddScore;
        TargetEvents.OnPenaltyTargetHit += DeductScore;
    }

    private void OnDisable()
    {
        TargetEvents.OnScoreTargetHit -= AddScore;
        TargetEvents.OnPenaltyTargetHit -= DeductScore;
    }

    private void Start()
    {
        UpdateDisplay();
    }

    private void AddScore()
    {
        currentScore += pointsPerRedTarget;
        UpdateDisplay();
    }

    private void DeductScore()
    {
        currentScore = Mathf.Max(MINIMUM_SCORE, currentScore - pointsDeductedPerGreenTarget);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}