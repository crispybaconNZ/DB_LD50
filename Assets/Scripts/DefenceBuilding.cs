using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DefenceBuilding : MonoBehaviour {
    public int startingHealth = 10;
    public string _name = "building";
    public string _description = "a building";
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackRate = 0.5f;    // number of seconds between shots
    [SerializeField] private bool canAttack = false;
    [SerializeField] private GameObject firePoint = null;
    [SerializeField] private GameObject bulletPrefab;
    private int _currentHealth;
    public int cost = 10;

    private List<GameObject> targetList;
    private GameObject target;
    private EnemyManager enemyManager;
    private float timeSinceLastAttack;

    public const int TURRET_COST = 50;
    public const int BARRIER_COST = 20;

    public class DefenceDestroyed : UnityEvent<Vector3> { }
    public DefenceDestroyed OnDefenceDestroyed;

    private void Awake() {
        enemyManager = GameObject.Find("EnemyController").GetComponent<EnemyManager>();
        if (OnDefenceDestroyed == null) { OnDefenceDestroyed = new DefenceDestroyed(); }
        _currentHealth = startingHealth;
    }

    void Start() {
        enemyManager.OnWaveChanged.AddListener(GetEnemies);
        timeSinceLastAttack = attackRate;
        GetEnemies(0);
    }

    public void DoDamage(int damage) {
        // do the specifed amount of damage to the building
        _currentHealth -= damage;

        if (_currentHealth <= 0) {
            OnDefenceDestroyed?.Invoke(this.transform.position);
            Destroy(this.gameObject);
        }
    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }

    private void Update() {
        if (canAttack && targetList.Count > 0) {
            if (this.target == null) {
                // don't have a current target, so pick closest
                float minDistance = 100f;

                foreach (GameObject t in targetList) {
                    if (t != null) {
                        float d = Vector2.Distance(t.transform.position, this.transform.position);
                        if (d < minDistance) {
                            minDistance = d;
                            target = t;
                        }
                    }
                }
            }
            Attack();
        } else {
            target = null;
        }
    }

    private void GetEnemies(int i) {
        targetList = enemyManager.GetEnemies();
    }

    private void Attack() {
        // attack the current target if it's within range
        if (target == null) { return; }
        float distance = Vector2.Distance(target.transform.position, this.transform.position);
        if (distance <= attackRadius) {
            if (timeSinceLastAttack >= attackRate) {
                Vector3 pos = firePoint.transform.position;

                GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                bullet.transform.parent = this.transform;
                bullet.GetComponent<BulletController>().SetDirection((target.transform.position - pos).normalized);

                timeSinceLastAttack = 0f;
            } else {
                timeSinceLastAttack += Time.deltaTime;
            }
        }
    }

    public int GetHealth() { return _currentHealth; }
}
