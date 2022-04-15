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
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button defencesButton;
    [SerializeField] private GameObject defenceCardPrefab;

    [SerializeField] private DefenceBuilding[] defences;

    private GameObject defenceUIcanvas;
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
    // private Button barrierPurchaseButton;
    // private Button turretPurchaseButton;

    private readonly Color enabledButtonColour = new Color(0f, 1f, 0.15f);
    private readonly Color disabledButtonColour = new Color(0.5f, 0.5f, 0.5f);

    private readonly int[] columns = new int[] { 0, 250, 475 };
    private readonly int[] rows = new int[] { 65, -150 };

    public class DefenceSelected : UnityEvent<DefenceBuilding> { }
    public DefenceSelected OnDefenceSelected;

    private void Awake() {
        if (OnDefenceSelected == null) { OnDefenceSelected = new DefenceSelected(); }
    }

    private void Start() {
        playerManager = GameObject.Find("PlayerBase").GetComponent<PlayerManager>();
        playerManager.OnScoreChanged.AddListener(UpdateScore);
        playerManager.OnGameOver.AddListener(DisplayGameOver);

        enemyManager = GameObject.Find("EnemyController").GetComponent<EnemyManager>();
        enemyManager.OnWaveChanged.AddListener(UpdateWave);
        enemyManager.OnEnemyDeath.AddListener(SetEnemyCount);

        defencesButton.onClick.AddListener(OnDefencesButtonClicked);
        defenceUIcanvas = GameObject.Find("DefencesUI");

        /* barrierPurchaseButton = GameObject.Find("BarrierPurchaseButton").GetComponent<Button>();
        barrierPurchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cost: " + DefenceBuilding.BARRIER_COST;
        turretPurchaseButton = GameObject.Find("TurretPurchaseButton").GetComponent<Button> ();
        turretPurchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Cost: " + DefenceBuilding.TURRET_COST; */
        SetUpDefenceCards();

        // turn off text fields not needed on start-up
        defenceUIcanvas.SetActive(false);
        gameOverText.enabled = false;
        waveText.enabled = false;
        enemiesText.enabled = false;

        UpdateScore(playerManager.GetScore());
    }

    private void SetUpDefenceCards() {
        Debug.Log("# defences: " + defences.Length);
        int index = 0;
        foreach (DefenceBuilding building in defences) {
            int row = index / 3;
            int column = index % 3;
            Debug.Log("Row: " + row + ", Column: " + column);
            Vector3 pos = new Vector3(columns[column], rows[row]);

            GameObject prefab = Instantiate<GameObject>(defenceCardPrefab, pos, Quaternion.identity, GameObject.Find("Buildings").transform);
            prefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = building._name;
            prefab.transform.GetChild(2).GetComponent<Image>().sprite = building.GetComponent<SpriteRenderer>().sprite;
            prefab.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = building._description;
            prefab.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().text = "Cost: " + building.cost;          

            index++;
        }
    }

    public void Update() {

    }

    public void UpdateScore(int score) {
        scoreText.text = score.ToString();
    }

    public void UpdateWave(int wave) {
        if (wave > 0) {
            waveText.text = "Wave: " + wave.ToString();
            waveText.enabled = true;
            SetEnemyCount();
        } else {
            waveText.enabled = false; 
        }
    }

    public void OnDefencesButtonClicked() {
        defenceUIcanvas.SetActive(true);
        defencesButton.enabled = false; // don't want to click on it again

        // enable purchase buttons and set colour depending on cost vs. score
        // icky, nasty not-nice hard coupling going on here ... avert your eyes
        // changed score-cost comparison from >= to >, so player can't lose by spending all their points
        // barrierPurchaseButton.enabled = playerManager.GetScore() > DefenceBuilding.BARRIER_COST;
        // barrierPurchaseButton.GetComponent<Image>().color = barrierPurchaseButton.enabled ? enabledButtonColour : disabledButtonColour;
        // turretPurchaseButton.enabled = playerManager.GetScore() > DefenceBuilding.TURRET_COST;
        // turretPurchaseButton.GetComponent<Image>().color = turretPurchaseButton.enabled ? enabledButtonColour : disabledButtonColour;  
    }

    public void OnDefencesDialogCloseButtonClicked() {
        defenceUIcanvas.SetActive(false);
        defencesButton.enabled = true;
    }

    public void SetMessage(string message) { messageText.text = message.Trim(); }
    public void ClearMessage() { SetMessage(""); }

    public void SetCountdown(int timeLeft) {
        if (timeLeft >= 0) {
            countdownText.text = "Time to next wave: " + timeLeft.ToString() + " seconds";
        } else {
            countdownText.text = "";
        }
    }

    public void DisplayGameOver() {
        gameOverText.enabled = true;
    }

    public void SetEnemyCount(int _ = 0) {
        if (enemyManager.EnemyCount() > 0) {
            enemiesText.text = "Enemies: " + enemyManager.EnemyCount();
            enemiesText.enabled = true;
        } else {
            enemiesText.enabled = false;
        }
    }

    
}
