using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class UIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private GameObject countdownTimer;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private GameObject buildMenu;

    private PlayerManager _playerManager;
    private EnemyManager _enemyManager;

    private void Awake() {
        _playerManager = GameObject.Find("PlayerBase").GetComponent<PlayerManager>();
        _enemyManager = GameObject.Find("EnemyController").GetComponent<EnemyManager>();
    }

    private void Start() {
        countdownTimer.SetActive(false);
        buildMenu.SetActive(false);
        scoreText.text = _playerManager.GetScore().ToString();
        gameOverText.enabled = false;

        _playerManager.OnBuildMenuOpenClose.AddListener(ShowBuildMenu);
        _playerManager.OnScoreChanged.AddListener(UpdateScore);
        _playerManager.OnGameOver.AddListener(ShowGameOver);

        _enemyManager.OnWaveStarted.AddListener(UpdateWave);
        _enemyManager.OnWaveEnded.AddListener(UpdateWave);
        _enemyManager.OnEnemyDeath.AddListener(UpdateEnemyCount);
    }

    public void Update() {

    }

    public void UpdateScore(int score) { scoreText.text = score.ToString(); }

    public void UpdateWave(int wave) {
        if (wave > 0) {
            waveText.text = wave.ToString();
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
    }
    
}
