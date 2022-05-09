using System;
using UnityEngine;

public class BaseUnitSO : ScriptableObject {
    public Faction Faction;

    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    public string Name;
    public string Description;
    public Sprite MenuSprite;    
}

[Serializable]
public struct Stats {
    public int Health;
    public int AttackPower;
    public int Speed;
    public int Cost;
}

public enum Faction {
    Defenders = 0,
    Attackers = 1
}