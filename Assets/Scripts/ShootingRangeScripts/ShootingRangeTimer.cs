using System;
using UnityEngine;
using TMPro;

// Counts down the round, applies time penalties, and reports the final result.
public class ShootingRangeTimer : MonoBehaviour
{
    // Notify gameplay listeners to clean up before the result is displayed.
    public static event Action OnTimerExpired;
    // Send the final score and required score to the result panel.
    public event Action<int, int> RoundFinished;
    // Constants declared at class level
    private const float DEFAULT_TIME_REMAINING = 60f;
    private const float DEFAULT_PENALTY_DEDUCTION = 5f;
    private const int DEFAULT_REQUIRED_SCORE = 350;
    private const int SECONDS_PER_MINUTE = 60;
    private const float ZERO_TIME = 0f;

    [Header("Timer Settings")]
    [SerializeField] private float timeRemaining = DEFAULT_TIME_REMAINING;
    [SerializeField] private float penaltyTimeDeduction = DEFAULT_PENALTY_DEDUCTION;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerIsRunning = false;

    private void Awake()
    {
        if (timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        TargetEvents.OnPenaltyTargetHit += DeductTime;
    }

    private void OnDisable()
    {
        TargetEvents.OnPenaltyTargetHit -= DeductTime;
    }

    // The start panel controls when this component is first enabled.
    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!timerIsRunning) return;

        if (timeRemaining > ZERO_TIME)
        {
            timeRemaining -= Time.deltaTime;
            UpdateDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = ZERO_TIME;
            timerIsRunning = false;
            UpdateDisplay(timeRemaining);
            TriggerTimerExpired();
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // A penalty can end the round immediately if it uses up the remaining time.
    public void DeductTime()
    {
        timeRemaining = Mathf.Max(ZERO_TIME, timeRemaining - penaltyTimeDeduction);
        UpdateDisplay(timeRemaining);

        if (timeRemaining <= ZERO_TIME && timerIsRunning)
        {
            timerIsRunning = false;
            TriggerTimerExpired();
        }
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    // Capture the score and goal before notifying cleanup and result listeners.
    private void TriggerTimerExpired()
    {
        Debug.Log("Time's Up!");

        ShootingRangeScore scoreComponent = FindFirstObjectByType<ShootingRangeScore>();
        int finalScore = scoreComponent != null ? scoreComponent.GetCurrentScore() : 0;

        WaveSpawner spawner = FindFirstObjectByType<WaveSpawner>();
        int requiredScore = spawner != null ? spawner.TargetScoreToPass : DEFAULT_REQUIRED_SCORE;

        if (finalScore >= requiredScore)
        {
            Debug.Log($"STAGE PASSED! Final Score: {finalScore} (Required: {requiredScore})");
        }
        else
        {
            Debug.Log($"STAGE FAILED! Final Score: {finalScore} (Required: {requiredScore})");
        }

        OnTimerExpired?.Invoke();
        RoundFinished?.Invoke(finalScore, requiredScore);
    }

    // Format the remaining time as minutes and whole seconds.
    private void UpdateDisplay(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / SECONDS_PER_MINUTE);
        float seconds = Mathf.FloorToInt(timeToDisplay % SECONDS_PER_MINUTE);

        if (timerText != null)
        {
            timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }
}