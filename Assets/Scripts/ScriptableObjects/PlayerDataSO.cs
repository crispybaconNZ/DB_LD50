using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "ScriptableObjects")]
public class PlayerDataSO : ScriptableObject {
    // class to contain player data
    public int score;
    public int scorePeak;
    public int enemiesDestroyed;
    public float timeAlive;
    public string reason;

    public string TimeAliveToString() {
        return Mathf.Floor(timeAlive / 60).ToString("00") + ":" + Mathf.Floor(timeAlive % 60).ToString("00");
    }
}
