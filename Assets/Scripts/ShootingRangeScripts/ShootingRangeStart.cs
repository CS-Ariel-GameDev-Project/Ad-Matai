using System.Collections;
using UnityEngine;

// Attach this script to the Canvas.
// The start panel will call BeginRound when the player taps it.
public class ShootingRangeStart : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private ShootingRangeTimer timer;
    [SerializeField] private WaveSpawner spawner;
    [SerializeField] private ShooterController shooter;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject timerText;

    private bool hasStarted;

    // Called by the full-screen start panel when the player taps or clicks.
    public void BeginRound()
    {
        // Ignore repeated taps once the round has started.
        if (hasStarted)
            return;

        hasStarted = true;
        startPanel.SetActive(false);
        StartCoroutine(EnableRoundNextFrame());
    }

    private IEnumerator EnableRoundNextFrame()
    {
        // Let the start tap finish before accepting gameplay input.
        yield return null;
        // Reveal the HUD before enabling the components that update it.
        scoreText.SetActive(true);
        timerText.SetActive(true);
        timer.enabled = true;
        spawner.enabled = true;
        shooter.enabled = true;
    }
}