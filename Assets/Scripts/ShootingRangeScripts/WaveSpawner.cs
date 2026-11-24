
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates target waves and clears the range when the round ends.
public class WaveSpawner : MonoBehaviour
{
    // Constants to eliminate magic numbers
    private const int DEFAULT_TOTAL_WAVES = 1000;
    private const int DEFAULT_MIN_TARGETS_PER_WAVE = 2;
    private const int DEFAULT_MAX_TARGETS_PER_WAVE = 4;
    private const float DEFAULT_GREEN_PENALTY_CHANCE = 0.33f;
    private const float DEFAULT_DELAY_BETWEEN_WAVES = 1.0f;
    private const int DEFAULT_TARGET_SCORE_TO_PASS = 350;

    private const float DEFAULT_SPAWN_CENTER_X = 0f;
    private const float DEFAULT_SPAWN_CENTER_Y = 0f;
    private const float DEFAULT_SPAWN_AREA_WIDTH = 15f;
    private const float DEFAULT_SPAWN_AREA_HEIGHT = 6f;

    private const float DEFAULT_MIN_SPAWN_DISTANCE = 1.5f;
    private const int DEFAULT_MAX_SPAWN_ATTEMPTS = 50;

    private const float HALF_DIVISOR = 2f;
    private const int EXCLUSIVE_RANGE_OFFSET = 1;
    private const float ZERO_Z = 0f;

    [Header("Prefabs")]
    [SerializeField] private GameObject redTargetPrefab;
    [SerializeField] private GameObject greenTargetPrefab;

    [Header("LiveOps Remote Config")]
    [SerializeField] private RangeRemoteConfig remoteConfig;

    [Header("Procedural Wave Settings")]
    [SerializeField] private int totalWaves = DEFAULT_TOTAL_WAVES;
    [SerializeField] private int minTargetsPerWave = DEFAULT_MIN_TARGETS_PER_WAVE;
    [SerializeField] private int maxTargetsPerWave = DEFAULT_MAX_TARGETS_PER_WAVE;
    [SerializeField] private float greenPenaltyChance = DEFAULT_GREEN_PENALTY_CHANCE;
    [SerializeField] private float delayBetweenWaves = DEFAULT_DELAY_BETWEEN_WAVES;

    [Header("Pass Condition")]
    [SerializeField] private int targetScoreToPass = DEFAULT_TARGET_SCORE_TO_PASS;
    public int TargetScoreToPass => targetScoreToPass;

    [Header("Spawn Bounds (Dirt Area)")]
    [SerializeField] private Vector2 spawnCenter = new Vector2(DEFAULT_SPAWN_CENTER_X, DEFAULT_SPAWN_CENTER_Y);
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(DEFAULT_SPAWN_AREA_WIDTH, DEFAULT_SPAWN_AREA_HEIGHT);

    [Header("Overlap Prevention")]
    [SerializeField] private float minSpawnDistance = DEFAULT_MIN_SPAWN_DISTANCE;
    [SerializeField] private int maxSpawnAttempts = DEFAULT_MAX_SPAWN_ATTEMPTS;

    private bool isSpawning = true;
    private List<GameObject> activeRedTargets = new List<GameObject>();

    // Subscribe when the start panel enables spawning.
    private void OnEnable()
    {
        ShootingRangeTimer.OnTimerExpired += StopSpawning;
    }

    private void OnDisable()
    {
        ShootingRangeTimer.OnTimerExpired -= StopSpawning;
    }

    // Wait for LiveOps configuration before spawning the first wave.
    private IEnumerator Start()
    {
        if (remoteConfig != null && remoteConfig.isActiveAndEnabled)
        {
            yield return new WaitUntil(() => remoteConfig.IsReady);

            // Apply LiveOps settings before the shooting range starts.
            ApplyRemoteConfig();
        }

        if (isSpawning)
        {
            StartCoroutine(SpawnWavesRoutine());
        }
    }

    // Apply LiveOps parameters fetched from Unity Remote Config.
    // Only wave size and green target probability are overridden.
    private void ApplyRemoteConfig()
    {
        if (remoteConfig == null)
        {
            return;
        }

        minTargetsPerWave = remoteConfig.MinWaveSize;
        maxTargetsPerWave = remoteConfig.MaxWaveSize;
        greenPenaltyChance = remoteConfig.GreenTargetChance;

        Debug.Log(
            $"LiveOps applied to WaveSpawner: " +
            $"Min={minTargetsPerWave}, " +
            $"Max={maxTargetsPerWave}, " +
            $"Green={greenPenaltyChance}"
        );
    }

    // Stop future waves and remove targets from the current wave.
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();

        // Disable hits immediately and remove every remaining target.
        Target[] remainingTargets =
            FindObjectsByType<Target>(FindObjectsSortMode.None);

        foreach (Target target in remainingTargets)
        {
            target.GetComponent<Collider2D>().enabled = false;
            Destroy(target.gameObject);
        }
    }

    // Create one wave at a time and advance when its red targets are gone.
    private IEnumerator SpawnWavesRoutine()
    {
        for (int wave = 0; wave < totalWaves && isSpawning; wave++)
        {
            activeRedTargets.Clear();
            int targetCount = Random.Range(minTargetsPerWave, maxTargetsPerWave + EXCLUSIVE_RANGE_OFFSET);

            for (int i = 0; i < targetCount; i++)
            {
                SpawnTarget();
            }

            // Wait until ALL RED targets from the current wave are destroyed
            yield return new WaitUntil(() => AllRedTargetsCleared());

            // Clear remaining green targets on wave end
            ClearRemainingGreenTargets();

            // Pause before spawning the next wave
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    // Destroyed Unity objects compare equal to null after removal.
    private bool AllRedTargetsCleared()
    {
        activeRedTargets.RemoveAll(t => t == null);
        return activeRedTargets.Count == 0;
    }

    // Clear leftover penalty targets before starting the next wave.
    private void ClearRemainingGreenTargets()
    {
        Target[] allTargets = FindObjectsByType<Target>(FindObjectsSortMode.None);
        foreach (Target target in allTargets)
        {
            if (target != null && target.IsPenaltyTarget)
            {
                Destroy(target.gameObject);
            }
        }
    }

    // Choose a target type and track red targets for the wave completion check.
    private void SpawnTarget()
    {
        bool isGreen = Random.value < greenPenaltyChance;
        GameObject prefabToSpawn = isGreen ? greenTargetPrefab : redTargetPrefab;
        if (prefabToSpawn == null) return;

        Vector3 spawnPosition = GetValidSpawnPosition();
        GameObject spawned = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        if (!isGreen)
        {
            activeRedTargets.Add(spawned);
        }
    }

    // Try positions inside the configured spawn area until one has clearance.
    private Vector3 GetValidSpawnPosition()
    {
        Vector3 position = Vector3.zero;
        bool validPositionFound = false;

        float halfWidth = spawnAreaSize.x / HALF_DIVISOR;
        float halfHeight = spawnAreaSize.y / HALF_DIVISOR;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float randomX = Random.Range(spawnCenter.x - halfWidth, spawnCenter.x + halfWidth);
            float randomY = Random.Range(spawnCenter.y - halfHeight, spawnCenter.y + halfHeight);
            position = new Vector3(randomX, randomY, ZERO_Z);

            Collider2D hit = Physics2D.OverlapCircle(position, minSpawnDistance);
            if (hit == null)
            {
                validPositionFound = true;
                break;
            }
        }

        // Use a random fallback if none of the attempts has enough clearance.
        if (!validPositionFound)
        {
            float randomX = Random.Range(spawnCenter.x - halfWidth, spawnCenter.x + halfWidth);
            float randomY = Random.Range(spawnCenter.y - halfHeight, spawnCenter.y + halfHeight);
            position = new Vector3(randomX, randomY, ZERO_Z);
        }

        return position;
    }

    // Show the spawn area in the Scene view while tuning the bounds.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnCenter, spawnAreaSize);
    }
}
