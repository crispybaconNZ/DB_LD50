using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DefenceBuilding : MonoBehaviour, IHealth {
    [SerializeField, Range(0, 20), Tooltip("Hit points defence will start with (0-20)")] private int startingHealth = 10;
    public string _name = "building";
    public string _description = "a building";

    [SerializeField, Range(0f, 20f), Tooltip("Maximum distance this defence will start attacking (0-20)")] private float attackRadius = 1.5f;
    [SerializeField, Range(0.1f, 10f), Tooltip("Number of seconds between attacks (0.1-10)")] private float attackRate = 0.5f;
    [SerializeField, Tooltip("Whether this defence can attack enemies")] private bool canAttack = false;
    [SerializeField, Tooltip("Whether this defence is triggered by an enemy")] private bool isTriggered = false;
    [SerializeField, Tooltip("Location defence's bullets appear from")] private GameObject firePoint = null;
    [SerializeField, Tooltip("Prefab for the defence's bullets")] private GameObject bulletPrefab;
    [SerializeField, Tooltip("Prefab for an explosion")] private GameObject explosionPrefab;
    [SerializeField, Tooltip("Prefab for defence's floating text")] private GameObject _floatingTextPrefab;
    [SerializeField, Tooltip("Whether this defence shows floating score decrease on death")] private bool _showDeathFloatingText = true;
    [SerializeField, Range(0, 20), Tooltip("Damage done to an enemy on contact")] private int contactDamage = 0;
    [SerializeField, Range(1, 100), Tooltip("Purchasee cost of the defence")] private int cost = 10;
    private int _currentHealth;
    
    private List<GameObject> targetList;
    private GameObject target;
    private EnemyManager enemyManager;
    private float timeSinceLastAttack;

    public class DefenceDestroyed : UnityEvent<Vector3> { }
    public DefenceDestroyed OnDefenceDestroyed;

    private void Awake() {
        if (OnDefenceDestroyed == null) { OnDefenceDestroyed = new DefenceDestroyed(); }
    }

    void Start() {
        enemyManager = GameObject.Find("EnemyController").GetComponent<EnemyManager>();
        _currentHealth = startingHealth;
        enemyManager.OnWaveStarted.AddListener(GetEnemies);
        timeSinceLastAttack = attackRate;
        GetEnemies(0, false);
    }

    public int DoDamage(int damage) {
        // do the specifed amount of damage to the building
        _currentHealth -= damage;
        if (_currentHealth <= 0) {
            // create an explosion at this location, and destroy it once it's done
            GameObject explosion = Instantiate(explosionPrefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, GameObject.Find("UI").transform);
            Destroy(explosion, 11 / 30f);
            FindObjectOfType<AudioManager>().Play("explosion");
            OnDefenceDestroyed?.Invoke(transform.position);
            if (_floatingTextPrefab != null && _showDeathFloatingText) {
                Utils.CreateFloatingText(-GetStartingHealth(), gameObject, _floatingTextPrefab);
            }
            Destroy(gameObject);
            return 0;
        } else {
            // not dead, so show damage in a difference colour
            if (_floatingTextPrefab != null) {
                GameObject ft = Utils.CreateFloatingText(-damage, gameObject, _floatingTextPrefab, Color.yellow);
            }
            return _currentHealth;
        }
    }

    public bool IsDead() {
        return _currentHealth <= 0;
    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }

    private void Update() {
        if (target != null) {
            // gizmo: draw a line from the defence to its current target
            Debug.DrawLine(this.transform.position, target.transform.position, Color.magenta);
        }

        if (isTriggered) {
            // this type of defence building waits for an enemy to get close then delivers a lot of damage to the target
            SetNearestTarget(true);
            TriggerAttack();
        } else if (canAttack && targetList.Count > 0) {
            SetNearestTarget();
            ShootAttack();
        } else {
            target = null;
        }
    }

    private void SetNearestTarget(bool changeTarget = false) {
        // if changeTarget is true, select a new target; if false, on select a new target if we don't currently have one
        if (this.target == null || changeTarget) {
            // don't have a current target or always pick closest, so pick closest
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
    }

    private void GetEnemies(int i, bool _) { targetList = enemyManager.GetEnemies(); }

    private void ShootAttack() {
        // attack the current target if it's within range
        if (target == null) { return; }
        float distance = Vector2.Distance(target.transform.position, this.transform.position);
        if (distance <= attackRadius) {
            if (timeSinceLastAttack >= attackRate) {
                Vector3 pos = firePoint.transform.position;

                GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                bullet.transform.parent = this.transform;
                bullet.GetComponent<BulletController>().SetDirection((target.transform.position - pos).normalized);
                FindObjectOfType<AudioManager>().Play("gunfire");
                timeSinceLastAttack = 0f;
            } else {
                timeSinceLastAttack += Time.deltaTime;
            }
        }
    }
    
    private void TriggerAttack() {
        if (target == null) { return; }
        float distance = Vector2.Distance(target.transform.position, this.transform.position);
        
        if (distance <= attackRadius) {
            Soldier s = target.GetComponent<Soldier>();
            s.DoDamage(contactDamage);
            DoDamage(1);
        }
    }

    public int GetHealth() { return _currentHealth; }
    public int GetStartingHealth() { return startingHealth; }
    public int GetCost() { return cost; }

    public void EnableHealthBar() { GetComponent<HealthBar>().enabled = true; }
}
