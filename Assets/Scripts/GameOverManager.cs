using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {
    [SerializeField] private PlayerDataSO playerData;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI peakScoreText;
    [SerializeField] private TextMeshProUGUI enemiesDestroyedText;
    [SerializeField] private TextMeshProUGUI timeAliveText;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;

    [SerializeField] private Image finalScoreBestImage;
    [SerializeField] private Image peakScoreBestImage;
    [SerializeField] private Image enemiesDestroyedBestImage;
    [SerializeField] private Image survivalTimeBestImage;

    void Start() {
        finalScoreText.text = $"{playerData.score:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        peakScoreText.text = $"{playerData.scorePeak:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        enemiesDestroyedText.text = $"{playerData.enemiesDestroyed:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        timeAliveText.text = Mathf.Floor(playerData.timeAlive / 60).ToString("00") + ":" + Mathf.Floor(playerData.timeAlive % 60).ToString("00");
        gameOverReasonText.text = playerData.reason;

        finalScoreBestImage.enabled = SetFinalScore();
        peakScoreBestImage.enabled = SetPeakScore();
        enemiesDestroyedBestImage.enabled = SetEnemiesDestroyed();
        survivalTimeBestImage.enabled = SetSurvivalTime();
    }

    public void TryAgain() {
        SceneManager.LoadScene("Game");
    }

    public void MainMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    private bool SetFinalScore() {
        int stored = PlayerPrefs.GetInt("Score");
        if (stored < playerData.score) {
            PlayerPrefs.SetInt("Score", playerData.score);
            return true;
        }
        return false;
    }

    private bool SetPeakScore() {
        int stored = PlayerPrefs.GetInt("PeakScore");
        if (stored < playerData.scorePeak) {
            PlayerPrefs.SetInt("PeakScore", playerData.scorePeak);
            return true;
        }
        return false;
    }

    private bool SetEnemiesDestroyed() {
        int stored = PlayerPrefs.GetInt("EnemiesDestroyed");
        if (stored < playerData.enemiesDestroyed) {
            PlayerPrefs.SetInt("EnemiesDestroyed", playerData.enemiesDestroyed);
            return true;
        }
        return false;

    }

    private bool SetSurvivalTime() {
        float stored = PlayerPrefs.GetFloat("TimeSurvived");
        if (stored < playerData.timeAlive) {
            PlayerPrefs.SetFloat("TimeSurvived", playerData.timeAlive);
            return true;
        }
        return false;
    }
}
