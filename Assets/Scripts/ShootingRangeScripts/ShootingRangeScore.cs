using UnityEngine;
using TMPro;

// Tracks points from target hits and displays the current score.
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

    // Listen only while the score component is active.
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

    // Show the initial score before the first target is hit.
    private void Start()
    {
        UpdateDisplay();
    }

    private void AddScore()
    {
        currentScore += pointsPerRedTarget;
        UpdateDisplay();
    }

    // Apply the configured penalty without letting the score drop below zero.
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

    // The timer reads this value when it captures the final result.
    public int GetCurrentScore()
    {
        return currentScore;
    }
}