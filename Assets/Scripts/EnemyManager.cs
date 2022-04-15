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
    private float timeToNextWave = 5f;
    private Transform attackPoint;
    private UIManager uiManager;

    public class WaveEvent : UnityEvent<int> { }
    public WaveEvent OnWaveChanged;

    public class EnemyDeathEvent: UnityEvent<int>{ }
    public EnemyDeathEvent OnEnemyDeath;


    private void Awake() {
        enemies = new List<GameObject>();
        spawnpoint = GameObject.Find("EnemySpawnPoint").transform;
        attackPoint = GameObject.Find("AttackPoint").transform;
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        if (OnWaveChanged == null) { OnWaveChanged = new WaveEvent(); }
        if (OnEnemyDeath == null) { OnEnemyDeath = new EnemyDeathEvent(); }
    }

    private void Update() {
        // if there are no enemies, and the timeToNextWave has ticked down, create a wave
        if (enemies.Count == 0) {
            timeToNextWave -= Time.deltaTime;
            // Debug.Log("Time to next wave: " + timeToNextWave + " seconds");
            uiManager.SetCountdown((int)timeToNextWave);
            if (timeToNextWave < 0) {
                CreateWave();
            }
            OnWaveChanged?.Invoke(currentWave);
        } else {
            // Debug.Log("Enemies left: " + enemies.Count);
            uiManager.SetCountdown(-1);
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
        // add another enemy for every 2nd level
        numEnemies += (currentWave - 1) / 2;
        Debug.Log("Creating wave " + currentWave + " with " + numEnemies + " enemies");

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

        if (currentWave % 5 == 0) {
            // boss wave
            int numBosses = currentWave / 5;
            Debug.Log("Adding " + numBosses + " bosses");

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

        OnWaveChanged?.Invoke(currentWave);
    }

    public List<GameObject> GetEnemies() {
        return enemies;
    }

    public void EnemyDied(int points, GameObject enemy) {
        // Debug.Log("Enemy " + enemy + " died, worth " + points);
        enemies.Remove(enemy);
        OnEnemyDeath?.Invoke(points);
        if (enemies.Count == 0) { timeToNextWave = 10f; }
    }

    public int EnemyCount() {
        return enemies.Count;
    }
}
