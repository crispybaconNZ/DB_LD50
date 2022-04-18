using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] private GameObject healthBarPrefab;
    private GameObject canvas;
    private GameObject healthBarInstance;
    [SerializeField] private float healthBarCorrection = -0.55f;
    private Slider slider;
    private Vector3 hostPosition;
    private bool isHostEnemy;

    void Start() {
        canvas = GameObject.Find("HealthBarContainer");        

        healthBarInstance = Instantiate(healthBarPrefab);
        healthBarInstance.transform.SetParent(canvas.transform, false);
        hostPosition = new Vector3(transform.position.x, transform.position.y + healthBarCorrection);
        isHostEnemy = GetComponent<Soldier>() != null;
        slider = healthBarInstance.GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = isHostEnemy ? GetComponent<Soldier>().startingHealth : GetComponent<DefenceBuilding>().startingHealth;
        
        healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(hostPosition);

        if (isHostEnemy) {
            GetComponent<Soldier>().OnEnemyDied.AddListener(EnemyDied);
            slider.value = GetComponent<Soldier>().GetHealth();
        } else {
            GetComponent<DefenceBuilding>().OnDefenceDestroyed.AddListener(DefenceBuildingDied);
            slider.value = GetComponent<DefenceBuilding>().GetHealth();
        }

    }

    void Update() {
        hostPosition = new Vector3(transform.position.x, transform.position.y + healthBarCorrection);
        healthBarInstance.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(hostPosition);
        if (isHostEnemy) {
            slider.value = GetComponent<Soldier>().GetHealth();
        } else {
            slider.value = GetComponent<DefenceBuilding>().GetHealth();
        }
    }

    private void EnemyDied(int x, GameObject _) {
        Destroy(healthBarInstance);
    }

    private void DefenceBuildingDied(Vector3 _) {
        Destroy(healthBarInstance);
    }
}
