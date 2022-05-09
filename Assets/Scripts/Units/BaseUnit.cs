using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour {
    public Stats Stats { get; private set; }
    public virtual void SetStats(Stats stats) => Stats = stats;

    public virtual void TakeDamage(int damage) {

    }
}
