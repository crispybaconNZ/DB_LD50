using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Script to control the basic enemy soldier */

enum SoldierActivity {
    Walking,
    Attacking
}

public class Soldier : MonoBehaviour, IHealth {
    [SerializeField, Range(1, 100), Tooltip("Number of hitpoints enemy starts with (0-100)")] private int startingHealth = 10;
    [SerializeField, Range(1f, 5f), Tooltip("Speed enemy moves at (1-5)")] private float _speed = 1.0f;
    [SerializeField, Range(1, 20), Tooltip("Amount of melee damage done (1-20)")] private int _meleeDamage = 5;
    [SerializeField, Range(0.1f, 10f), Tooltip("Number of seconds between attacks (0.1-10)")] private float attackSpeed = 0.5f;
    [SerializeField, Range(10, 100), Tooltip("Number of points enemy is worth (10-100)")] private int pointsValue = 10;
    [SerializeField, Tooltip("Prefab for explosion")] private GameObject explosionPrefab;
    [SerializeField, Tooltip("Whether the enemy can shoot from range")] private bool _canShoot = false;
    [SerializeField, Tooltip("Prefab for bullet")] private GameObject bulletPrefab;
    [SerializeField, Range(0f, 10f), Tooltip("Maximum range the enemy will shoot from (0-10)")] private float _shootRange = 5f;
    [SerializeField, Tooltip("Where the enemy's bullets appear")] private GameObject firingPoint;
    [SerializeField, Tooltip("Prefab for enemy's floating text")] private GameObject _floatingTextPrefab;

    private Transform target_;   // where the enemy is trying to get to
    private DefenceBuilding attackTarget = null; // what the enemy is attacking
    private SoldierActivity status = SoldierActivity.Walking;
    private float timeSinceLastAttack = 0.0f;
    private int health;
    private PlayerManager _playerManager;

    public class EnemyDiedEvent : UnityEvent<int, GameObject> { }
    public EnemyDiedEvent OnEnemyDied;

    private void Awake() {
        if (OnEnemyDied == null) { OnEnemyDied = new EnemyDiedEvent(); }        
    }

    private void Start() {
        health = startingHealth;
        _playerManager = GameObject.Find("PlayerBase").GetComponent<PlayerManager>();
    }

    public void SetTarget(Transform target) {
        target_ = target;
    }

    void Update() {
        if (IsDead()) { Die(); }

        if (status == SoldierActivity.Walking) {
            // walks straight towards the target
            if (target_ != null) {
                Vector3 direction = this.transform.position - target_.position;
                direction.Normalize();
                this.transform.position -= direction * _speed * Time.deltaTime;
            } else {
                this.transform.position += Vector3.left * _speed * Time.deltaTime;
            }

            if (this.transform.position.x <= -10) { Destroy(this.gameObject); }  // destroy if gone off left-edge of play area

            if (_canShoot) {
                // get nearest target that's in range
                List<DefenceBuilding> targets = _playerManager.GetDefences();

                float maxDistance = 200f;
                foreach (DefenceBuilding target in targets) {
                    if (target == null) continue;
                    float distance = Vector3.Distance(target.transform.position, transform.position);
                    if (distance < maxDistance && distance <= _shootRange) {
                        maxDistance = distance;
                        attackTarget = target;
                    }
                }

                // change status to attacking
                status = SoldierActivity.Attacking;
            }
        } else if (status == SoldierActivity.Attacking) {
            if (attackTarget != null) {
                Debug.DrawLine(attackTarget.transform.position, transform.position, Color.yellow);

                if (_canShoot) {
                    // have a target so just shoot at it if enough time has elapsed
                    if (timeSinceLastAttack >= attackSpeed) {
                        ShootAttack();
                        timeSinceLastAttack = 0.0f;
                    } else {
                        timeSinceLastAttack += Time.deltaTime;
                    }
                } else { 
                    // limited to melee attacks
                    if (timeSinceLastAttack >= attackSpeed) {
                        attackTarget?.DoDamage(_meleeDamage);
                        timeSinceLastAttack = 0.0f;
                    } else {
                        timeSinceLastAttack += Time.deltaTime;
                    }
                }
            } else {
                // no attack target, so keep walking
                status = SoldierActivity.Walking;
                timeSinceLastAttack = attackSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("Defence")) {
            attackTarget = collision.transform.GetComponent<DefenceBuilding>();
            status = SoldierActivity.Attacking;
        }
    }

    public int DoDamage(int damage) {
        health -= damage;
        if (_floatingTextPrefab != null) {
            Utils.CreateFloatingText(-damage, gameObject, _floatingTextPrefab, Color.yellow);
        }
        return health;
    }

    public bool IsDead() { return health <= 0; }

    public void Die() {
        GameObject explosion = Instantiate(explosionPrefab, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity, GameObject.Find("UI").transform);
        Destroy(explosion, 11 / 30f);
        FindObjectOfType<AudioManager>().Play("explosion");
        OnEnemyDied?.Invoke(pointsValue, this.gameObject);
        if (_floatingTextPrefab != null) {
            Utils.CreateFloatingText(pointsValue, gameObject, _floatingTextPrefab);
        }
        Destroy(this.gameObject);
    }

    public int GetHealth() { return health; }
    public int GetStartingHealth() {  return startingHealth; }

    private void ShootAttack() {
        // attack the current target if it's within range
        if (attackTarget == null) { return; }
        float distance = Vector2.Distance(attackTarget.transform.position, this.transform.position);
        if (distance <= _shootRange) {
            if (timeSinceLastAttack >= attackSpeed) {
                Vector3 pos = firingPoint.transform.position;

                GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                bullet.transform.parent = this.transform;
                bullet.GetComponent<EnemyBulletController>().SetDirection((attackTarget.transform.position - pos).normalized);
                FindObjectOfType<AudioManager>().Play("gunfire");
                timeSinceLastAttack = 0f;
            } else {
                timeSinceLastAttack += Time.deltaTime;
            }
        }
    }
}
