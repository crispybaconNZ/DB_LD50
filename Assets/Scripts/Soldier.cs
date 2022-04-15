using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Script to control the basic enemy soldier */

enum SoldierActivity {
    Walking,
    Attacking
}

public class Soldier : MonoBehaviour {
    [SerializeField] private int health = 10;
    [SerializeField] private float speed_ = 1.0f;
    [SerializeField] private int damage_ = 5;
    [SerializeField] private float attackSpeed = 0.5f;   // seconds between attacks
    [SerializeField] private int pointsValue = 10;

    private Transform target_;   // where the enemy is trying to get to
    private DefenceBuilding attackTarget = null; // what the enemy is attacking
    private SoldierActivity status = SoldierActivity.Walking;
    private float timeSinceLastAttack = 0.0f;

    public class EnemyDiedEvent : UnityEvent<int, GameObject> { }
    public EnemyDiedEvent OnEnemyDied;

    private void Awake() {
        if (OnEnemyDied == null) { OnEnemyDied = new EnemyDiedEvent(); }
    }

    public void SetTarget(Transform target) {
        target_ = target;
    }

    void Update() {
        if (health <= 0) { Die(); }

        if (status == SoldierActivity.Walking) {
            // walks straight towards the target
            if (target_ != null) {
                Vector3 direction = this.transform.position - target_.position;
                direction.Normalize();
                this.transform.position -= direction * speed_ * Time.deltaTime;
            } else {
                this.transform.position += Vector3.left * speed_ * Time.deltaTime;
            }

            if (this.transform.position.x <= -5) { Destroy(this.gameObject); }
        } else if (status == SoldierActivity.Attacking) {
            if (attackTarget != null) {
                if (timeSinceLastAttack >= attackSpeed) {
                    attackTarget.DoDamage(damage_);
                    timeSinceLastAttack = 0.0f;
                } else {
                    timeSinceLastAttack += Time.deltaTime;
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

    public void TakeHit(int damage) {
        health -= damage;
    }

    public void Die() {
        OnEnemyDied?.Invoke(pointsValue, this.gameObject);
        Destroy(this.gameObject);
    }
}
