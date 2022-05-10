using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _enemiesText;
    [SerializeField] private TextMeshProUGUI _elapsedTimeText;
    [SerializeField] private TextMeshProUGUI _destroyedText;
    [SerializeField] private TextMeshProUGUI _bonusText;
    [SerializeField] private TextMeshProUGUI _messageText;

    public void SetScoreText(int score) {
        _scoreText.text = $"{score:N0}".ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
    }

    public void SetElapsedTime(int minutes, int seconds) {
        string min = minutes.ToString("00");
        string sec = seconds.ToString("00");
        _elapsedTimeText.text = $"{min}:{sec}";
    }

    public void SetElapsedTime(float elapsedTime) {
        int minutes = (int)Mathf.Floor(elapsedTime / 60);
        int seconds = (int)Mathf.Floor(elapsedTime % 60);
        SetElapsedTime(minutes, seconds);
    }

    public void SetBonusText(int bonus) {
        if (bonus < 1)
            bonus = 1;

        _bonusText.text = "x" + bonus.ToString("00");
    }

    public void SetWaveText(int wave) {        
        _waveText.text = $"{wave.ToString("000")}";
    }

    public void SetWaveText(string wave) {
        _waveText.text = $"{wave.Trim()}";
    }

    public void SetEnemiesText(int enemies) {
        _enemiesText.text = $"{enemies.ToString("000")}";
    }

    public void SetDestroyedText(int destroyed) {
        _destroyedText.text = $"{destroyed.ToString("000")}";
    }

    private void Start() {
        SetScoreText(0);
        SetElapsedTime(0);
        SetBonusText(0);
        SetWaveText("-");
        SetEnemiesText(0);
        SetDestroyedText(0);
    }
}
