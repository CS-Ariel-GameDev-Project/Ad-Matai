using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
// Displays the final score on the Canvas when the timer ends the round.
public class ShootingRangeEnd : MonoBehaviour
{
    [SerializeField] private ShootingRangeTimer timer;
    [SerializeField] private ShooterController shooter;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text finalScoreText;

    // The Canvas can listen while the result panel is still hidden.
    private void OnEnable()
    {
        if (timer != null)
            timer.RoundFinished += ShowResult;
    }

    // Remove the listener when this view is disabled or the scene reloads.
    private void OnDisable()
    {
        if (timer != null)
            timer.RoundFinished -= ShowResult;
    }

    private void ShowResult(int finalScore, int requiredScore)
    {
        // Stop shooting and display the result reported by the timer.
        shooter.enabled = false;
        resultTitle.text = finalScore >= requiredScore ? "YOU WIN" : "GAME OVER";
        finalScoreText.text = $"Final Score: {finalScore} / {requiredScore}";
        endPanel.SetActive(true);
    }
    // Reload the shooting range to reset the round.
    public void PlayAgain()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }
}