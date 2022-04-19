using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


enum PlayerMode {
    Viewing,
    PlacingDefence,
    GameOver
}

public class PlayerManager : MonoBehaviour {
    [SerializeField] private Color canPlaceColour = Color.green;
    [SerializeField] private Color cantPlaceColour = Color.red;
    [SerializeField] private PlayerDataSO playerData;
    private int score;
    private float timeSinceLastUpdate;
    private float elapsedTime;
    private PlayerMode currentMode = PlayerMode.Viewing;
    private GameObject currentDefence = null;  // current defence being placed (if any)
    private Transform defencesContainer;
    public PlayerControls playerControls;
    private Camera cam;
    private List<DefenceBuilding> defences;
    private UIManager uiManager;
    private EnemyManager enemyManager;
    private DefenceBuilding defence = null;
    private DBGrid<GameObject> _grid;
    private int _enemiesDestroyed = 0;
    private int scorePeak;  // highest the player's score reaches 
   
    public class ScoreEvent : UnityEvent<int> { }
    public ScoreEvent OnScoreChanged;

    public class DefenceEvent : UnityEvent<GameObject> { }
    public DefenceEvent OnDefenceSelected;

    public class GameOverEvent : UnityEvent { }
    public GameOverEvent OnGameOver;

    public class BuildMenuOpenEvent : UnityEvent<bool> { }
    public BuildMenuOpenEvent OnBuildMenuOpenClose;

    private void Awake() {
        score = 100;
        scorePeak = 100;
        if (OnScoreChanged == null) { OnScoreChanged = new ScoreEvent(); }
        if (OnDefenceSelected == null) { OnDefenceSelected = new DefenceEvent(); }
        if (OnGameOver == null) { OnGameOver = new GameOverEvent(); }
        if (OnBuildMenuOpenClose == null) { OnBuildMenuOpenClose = new BuildMenuOpenEvent(); }


        playerControls = new PlayerControls();
        cam = Camera.main;
        defences = new List<DefenceBuilding>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        enemyManager = GameObject.Find("EnemyController").GetComponent<EnemyManager>();
        timeSinceLastUpdate = 2.0f;
        elapsedTime = 0f;

        _grid = new DBGrid<GameObject>(50, 10, 1f, new Vector3(-5f, -5f), () => { return null; });

        // add player base to the grid
        this.transform.position = new Vector3(-3.5f, 0.5f);
        _grid.SetValue(this.transform.position, this.gameObject);
        this.GetComponent<DefenceBuilding>().OnDefenceDestroyed.AddListener(GameOver);

        // add the starting turret to the grid
        GameObject startingTurret = GameObject.Find("Starting Turret");
        if (startingTurret != null) {
            _grid.SetValue(startingTurret.transform.position, startingTurret);
        }
        defencesContainer = GameObject.Find("DefencesContainer").transform;
    }

    private void Start() {
        enemyManager.OnEnemyDeath.AddListener(EnemyDied);
        this.OnDefenceSelected.AddListener(SetCurrentDefence);
    }

    public void AddToScore(int points) {
        if (points < 0) {
            SubtractFromScore(Mathf.Abs(points));
            return;
        }

        if (score > scorePeak) { scorePeak = score; }

        score += points;

        // send ScoreChanged event
        OnScoreChanged?.Invoke(score);
    }

    public void SubtractFromScore(int points) {
        score -= points;

        if (score < 0) { score = 0; }

        // send ScoreChanged event
        OnScoreChanged?.Invoke(score);

        // check for game over
        if (score == 0) { GameOver(); }
    }

    private void Update() {
        timeSinceLastUpdate -= Time.deltaTime;
        elapsedTime += Time.deltaTime;
        uiManager.UpdateElapsedTime(elapsedTime);
        if (scorePeak < score) { scorePeak = score; }

        /*
        if (timeSinceLastUpdate < 0.0f) {
            // reduce score by timer
            RemoveFromScore(2);
            timeSinceLastUpdate = 2.0f;
        }*/

        if (currentMode == PlayerMode.Viewing) {
            if (playerControls.Player.BuildMenu.WasPerformedThisFrame()) {
                OnBuildMenuOpenClose?.Invoke(true);
            }
        } else if (currentMode == PlayerMode.PlacingDefence) {
            OldDefenceMenuCode();
        } else if (currentMode == PlayerMode.GameOver) {

        }
    }

    private void OldDefenceMenuCode() {
        if (currentDefence != null) {
            Vector3 pos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pos = _grid.SnapToGridMidPoint(pos);
            if (defence == null) {
                defence = Instantiate(currentDefence, pos, Quaternion.identity).GetComponent<DefenceBuilding>();
            } else {
                defence.transform.position = pos;
            }
            SpriteRenderer sr = defence.GetComponent<SpriteRenderer>();

            // shade the sprite based on whether the grid square is occupied or not
            sr.color = _grid.GetValue(pos) == null ? canPlaceColour : cantPlaceColour;
            sr.sortingOrder = 100;


            if (playerControls.Player.Select.WasPerformedThisFrame()) {
                // player clicked the mouse               
                // add to grid based on world position, but only if that grid position is unoccupied
                if (_grid.GetValue(pos) == null) {
                    sr.color = Color.white;
                    sr.sortingOrder = 0;
                    _grid.SetValue(pos, currentDefence);

                    // snapped the currentDefence's position to the grid
                    pos = _grid.SnapToGridMidPoint(pos);

                    defence.OnDefenceDestroyed.AddListener(DefenceDestroyed);
                    SubtractFromScore(defence.cost);
                    defence.transform.SetParent(defencesContainer);
                    defences.Add(defence);


                    currentDefence = null;
                    currentMode = PlayerMode.Viewing;
                    uiManager.ClearMessage();
                    defence = null;
                }
            } else if (playerControls.Player.Cancel.WasPerformedThisFrame()) {
                // player clicked the right mouse button or pressed the Esc key so cancel placement
                currentDefence = null;
                currentMode = PlayerMode.Viewing;
                uiManager.ClearMessage();
                Destroy(defence.gameObject);
                defence = null;
            }
        }

    }

    public void SetCurrentDefence(GameObject defence) {
        currentDefence = defence;
        currentMode = PlayerMode.PlacingDefence;
    }

    public void GameOver(Vector3 _ = default) {
        uiManager.UpdateScore(score);
        uiManager.SetMessage(GameOverReason());
        currentMode = PlayerMode.GameOver;
        PopulatePlayerData();
        OnGameOver?.Invoke();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    public int GetScore() { 
        if (scorePeak < score) { scorePeak = score; }
        return score;
    }

    public void EnemyDied(int points) {
        AddToScore(points);
        _enemiesDestroyed++;
        uiManager.UpdateEnemiesDestroyed(_enemiesDestroyed);
    }

    public void DefenceDestroyed(Vector3 pos) {
        // a defence has been destroyed, so remove it from the grid and subtract its 
        // starting health from score
        DefenceBuilding defence = _grid.GetValue(pos).GetComponent<DefenceBuilding>();       
        _grid.SetValue(pos, null);
        SubtractFromScore(defence.startingHealth);
    }

    private void PopulatePlayerData() {
        if (playerData == null) { return; }

        playerData.enemiesDestroyed = _enemiesDestroyed;
        playerData.scorePeak = scorePeak;
        playerData.score = score;
        playerData.timeAlive = elapsedTime;
        playerData.reason = GameOverReason();
    }

    private string GameOverReason() {
        if (score <= 0) {
            return "You ran out of points!";
        } else if (GetComponent<DefenceBuilding>().GetHealth() <= 0) {
            return "Your base was destroyed!";
        } else {
            return "WTF? Something else caused your demise?!";
        }
    }
}
