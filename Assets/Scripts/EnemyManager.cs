using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour {
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject buggyPrefab;
    [SerializeField] private GameObject shootyTankPrefab;

    private List<GameObject> enemies;
    private int currentWave = 0;
    private Transform spawnpoint;
    private float timeToNextWave;
    private Transform attackPoint;
    private UIManager uiManager;

    private readonly float START_GAME_TIMER = 10f;
    private readonly float INTRA_WAVE_TIMER = 15f;

    public class WaveEvent : UnityEvent<int, bool> { }
    public WaveEvent OnWaveStarted;
    public WaveEvent OnWaveEnded;

    public class EnemyDeathEvent: UnityEvent<int>{ }
    public EnemyDeathEvent OnEnemyDeath;


    private void Awake() {
        enemies = new List<GameObject>();
        spawnpoint = GameObject.Find("EnemySpawnPoint").transform;
        attackPoint = GameObject.Find("AttackPoint").transform;
        uiManager = GameObject.Find("UI")?.GetComponent<UIManager>();
        timeToNextWave = START_GAME_TIMER;

        if (OnWaveStarted == null) { OnWaveStarted = new WaveEvent(); }
        if (OnWaveEnded == null) { OnWaveEnded = new WaveEvent(); }
        if (OnEnemyDeath == null) { OnEnemyDeath = new EnemyDeathEvent(); }
    }

    private void Update() {
        // if there are no enemies, and the timeToNextWave has ticked down, create a wave
        if (enemies.Count == 0) {
            timeToNextWave -= Time.deltaTime;
            uiManager.SetCountdown(timeToNextWave);
            if (timeToNextWave < 0) {
                CreateWave();
            }
            OnWaveStarted?.Invoke(currentWave, IsBossWave());
        } else {
            uiManager.SetCountdown(-1f);
        }
    }

    private void CreateWave() {
        uiManager.ClearMessage();
        // make sure enemies list is empty
        if (enemies.Count > 0) {
            foreach (GameObject enemy in enemies) {
                Destroy(enemy);
            }
            enemies.Clear();
        }

        // increment wave counter
        currentWave++;

        // add enemies
        // AddNormalSoldiers();
        AddBosses();
        AddFastAttack();
        AddShootyTanks();

        OnWaveStarted?.Invoke(currentWave, IsBossWave());
    }

    private void AddNormalSoldiers() {
        int numEnemies = 10;    // base        
        numEnemies += (currentWave - 1) / 2;  // add another enemy for every 2nd level

        for (int i = 0; i < numEnemies; i++) {
            Vector3 spawnAt = spawnpoint.position;
            spawnAt += new Vector3(Random.value * 2, (Random.value * 10) - 5, 0);

            GameObject enemy = Instantiate<GameObject>(soldierPrefab, spawnAt, Quaternion.identity);
            Soldier s = enemy.GetComponent<Soldier>();
            s.SetTarget(attackPoint);
            s.transform.parent = this.transform;
            s.OnEnemyDied.AddListener(EnemyDied);
            enemies.Add(enemy);
        }
    }

    private void AddBosses() {
        if (IsBossWave()) {
            int numBosses = currentWave / 5;

            for (int i = 0; i < numBosses; i++) {
                Vector3 spawnAt = spawnpoint.position;
                spawnAt += new Vector3(Random.value * 2, (Random.value * 8) - 4, 0);
                GameObject enemy = Instantiate<GameObject>(bossPrefab, spawnAt, Quaternion.identity);
                Soldier b = enemy.GetComponent<Soldier>();
                b.SetTarget(attackPoint);
                b.transform.parent = this.transform;
                b.OnEnemyDied.AddListener(EnemyDied);
                enemies.Add(enemy);
            }
        }
    }

    private void AddFastAttack() {
        // randomly spawn 1-3 super-fast buggies after level 5        
        if (currentWave > 5) {
            if (Random.value < 0.5) {
                int numberToSpawn = Random.Range(1, 3);
                for (int i = 0; i < numberToSpawn; i++) {
                    Vector3 spawnAt = spawnpoint.position;
                    spawnAt += new Vector3(Random.value * 2, (Random.value * 8) - 4, 0);
                    GameObject enemy = Instantiate<GameObject>(buggyPrefab, spawnAt, Quaternion.identity);
                    Soldier b = enemy.GetComponent<Soldier>();
                    b.SetTarget(attackPoint);
                    b.transform.parent = transform;
                    b.OnEnemyDied.AddListener(EnemyDied);
                    enemies.Add(enemy);
                    uiManager.SetMessage("A fast-attack enemy has joined the wave!");
                }
            }
        }
    }

    private void AddShootyTanks() {
        // randomly spawn a shooty tank
        if (currentWave > 0) {
            if (Random.value < 2) {
                Vector3 spawnAt = spawnpoint.position;
                spawnAt += new Vector3(Random.value * 2, (Random.value * 8) - 4, 0);
                GameObject enemy = Instantiate<GameObject>(shootyTankPrefab, spawnAt, Quaternion.identity);
                Soldier b = enemy.GetComponent<Soldier>();
                b.SetTarget(attackPoint);
                b.transform.parent = transform;
                b.OnEnemyDied.AddListener(EnemyDied);
                enemies.Add(enemy);
                uiManager.SetMessage("A shooty tank has joined the wave!");
            }
        }
    }

    public List<GameObject> GetEnemies() {
        return enemies;
    }

    public void EnemyDied(int points, GameObject enemy) {
        enemies.Remove(enemy);
        OnEnemyDeath?.Invoke(points);
        if (enemies.Count == 0) { 
            OnWaveEnded?.Invoke(-1, false);
            timeToNextWave = INTRA_WAVE_TIMER;
            uiManager.SetMessage($"Wave {currentWave} destroyed!");
        }
    }

    public int EnemyCount() { return enemies.Count; }

    private bool IsBossWave() { return currentWave % 5 == 0; }
}
