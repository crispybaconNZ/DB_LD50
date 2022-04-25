using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {
    [SerializeField, Tooltip("Reference to TMPro text field for score")] private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Reference to TMPro text field for message")] private TextMeshProUGUI messageText;
    [SerializeField, Tooltip("Reference to TMPro text field for wave number")] private TextMeshProUGUI waveText;
    [SerializeField, Tooltip("Reference to TMPro text field for number of enemies in wave")] private TextMeshProUGUI enemiesText;
    [SerializeField, Tooltip("Reference to countdown timer")] private GameObject countdownTimer;
    [SerializeField, Tooltip("Reference to TMPro text field for game over text")] private TextMeshProUGUI gameOverText;
    [SerializeField, Tooltip("Reference to build menu")] private GameObject buildMenu;
    [SerializeField, Tooltip("Reference to TMPro text field for elapsed time")] private TextMeshProUGUI elapsedTimeText;
    [SerializeField, Tooltip("Reference to TMPro text field for number of enemies destroyed")] private TextMeshProUGUI enemiesDestroyedText;
    [SerializeField, Tooltip("Reference to player manager")] private PlayerManager _playerManager;
    [SerializeField, Tooltip("Reference to enemy manager")] private EnemyManager _enemyManager;

    private void Start() {        
        countdownTimer.SetActive(false);
        buildMenu.SetActive(false);
        UpdateScore(_playerManager.GetScore());
        gameOverText.enabled = false;

        _playerManager.OnBuildMenuOpenClose.AddListener(ShowBuildMenu);
        _playerManager.OnScoreChanged.AddListener(UpdateScore);
        _playerManager.OnGameOver.AddListener(ShowGameOver);

        _enemyManager.OnWaveStarted.AddListener(UpdateWave);
        _enemyManager.OnWaveEnded.AddListener(UpdateWave);
        _enemyManager.OnEnemyDeath.AddListener(UpdateEnemyCount);
    }

    public void UpdateScore(int score) { scoreText.text = $"{score:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")); }

    public void UpdateWave(int wave, bool isBossWave = false) {
        if (wave > 0) {
            waveText.text = (isBossWave ? "*" : "") + wave.ToString();
            UpdateEnemyCount(_enemyManager.EnemyCount());
        } else {
            waveText.text = "---";
            UpdateEnemyCount(0);
        }
    }

    public void UpdateEnemyCount(int count) {
        enemiesText.text = _enemyManager.EnemyCount().ToString();
    }


    public void SetMessage(string message) { messageText.text = message.Trim(); }
    public void ClearMessage() { SetMessage(""); }

    public void SetCountdown(float timeLeft) {
        if (timeLeft > 0f) {
            countdownTimer.SetActive(true);
            Slider slider = countdownTimer.GetComponent<Slider>();
            slider.value = timeLeft;
        } else {
            countdownTimer.SetActive(false);
        }
    }

    public void ShowBuildMenu(bool show) {
        buildMenu?.SetActive(show);
    }

    private void ShowGameOver() {
        gameOverText.enabled = true;
        StartCoroutine(GameOverScreenAfterDelay());
    }
    
    public void UpdateElapsedTime(float time) {
        elapsedTimeText.text = TimeFloatToString(time);
    }

    private string TimeFloatToString(float time) {
        string minutes = Mathf.Floor(time / 60).ToString("00");
        string seconds = Mathf.Floor(time % 60).ToString("00");
        return minutes + ":" + seconds;
    }

    public void UpdateEnemiesDestroyed(int count) {
        enemiesDestroyedText.text = $"{count:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")); 
    }

    IEnumerator GameOverScreenAfterDelay() {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Game Over");
    }
}
