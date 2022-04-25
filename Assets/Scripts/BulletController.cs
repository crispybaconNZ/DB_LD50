using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    [SerializeField, Range(1, 10), Tooltip("Damage done by this bullet (1-10)")] private int damage = 1;
    [SerializeField, Range(1, 20), Tooltip("Speed of the bullet")] private float speed = 10f;
    private Vector3 direction = Vector3.zero;

    public int GetDamage() { return damage; }
    public void SetDirection(Vector3 direction) { this.direction = direction; }

    private void Update() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.deltaTime, LayerMask.GetMask("Enemies"));

        if (hit.collider != null) {
            Soldier s = hit.collider.GetComponent<Soldier>();
            s.TakeHit(damage);
            Destroy(this.gameObject);
        } else {
            this.transform.position += direction * speed * Time.deltaTime;

            if (Mathf.Abs(this.transform.position.y) > 10 || Mathf.Abs(this.transform.position.x) > 60) {
                // destroy bullet if it goes out of bounds
                Destroy(gameObject);
            }
        }
    }
}
