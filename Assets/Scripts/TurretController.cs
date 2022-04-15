using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {
    [SerializeField] private GameObject bulletPrefab;

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Turret has detected something: " + collision);
    }
}
