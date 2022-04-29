using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponSO : ScriptableObject { 
    public GameObject weaponPrefab;

    public float range;
    [Tooltip("This weapon fires a projectile")] public bool isProjectile;
    public GameObject projectilePrefab;

}
