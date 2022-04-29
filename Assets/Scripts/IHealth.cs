using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth {
    public int GetHealth();
    public int GetStartingHealth();

    public int DoDamage(int damage);
    public bool IsDead();
}
