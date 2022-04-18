using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour {
    private List<GameObject> enemies;
    private int currentWave = 0;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject bossPrefab;
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
        // make sure enemies list is empty
        if (enemies.Count > 0) {
            foreach (GameObject enemy in enemies) {
                Destroy(enemy);
            }
            enemies.Clear();
        }

        // increment wave counter
        currentWave++;

        // add soldiers
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

        if (IsBossWave()) {
            // boss wave
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

        OnWaveStarted?.Invoke(currentWave, IsBossWave());
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
        }
    }

    public int EnemyCount() { return enemies.Count; }

    private bool IsBossWave() { return currentWave % 5 == 0; }
}
