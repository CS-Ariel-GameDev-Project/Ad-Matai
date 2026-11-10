using System;
using UnityEngine;
using TMPro;

public class ShootingRangeTimer : MonoBehaviour
{
    public static event Action OnTimerExpired;

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
    }

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