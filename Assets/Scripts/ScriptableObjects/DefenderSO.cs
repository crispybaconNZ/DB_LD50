using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Defender")]
public class DefenderSO : BaseUnitSO {
    public DefenderType Type;
}

public enum DefenderType {
    Turret = 0, 
    Barrier = 1,
    SniperTower = 2, 
    Mine = 3
}