/*
 * Adds health bars for enemies and defences.
 * Based on code from here: https://forum.unity.com/threads/2d-ui-healthbar-follow-player.708962/ by JapanDoudou_fr
 * 
 * Needs some improvements, it's still very heavily dependent on knowing the type of the host (whether it's got a DefenceBuilding
 * or Soldier component). It only needs to know this to get the health, the starting health, and listen for the correct events.
 * The health bits should be able to be extracted out with an interface or maybe a superclass of DefenceBuilding and Soldier that 
 * holds everything in common? (Interface is the more composition-friendly way to go.) Not sure about the death events though.
 * 
 * Update: created the IHealth interface, which both Soldier and DefenceBuilding implement, so that tidies that part up nicely. The only
 * part left at the two events, depending on what the type of the object is. Possibly solve by have a UnitDied event that takes no arguments,
 * and both objects invoke this at the same time as they invoke their current death events.
 * 
 * Update 2: No, that doesn't work, because still need to GetComponent<>() with the right type. Generics, maybe?
 */

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
        slider.maxValue = GetComponent<IHealth>().GetStartingHealth();
        slider.value = GetComponent<IHealth>().GetHealth();
        
        healthBarInstance.transform.position = Camera.main.WorldToScreenPoint(hostPosition);
       
        if (isHostEnemy) {
            GetComponent<Soldier>().OnEnemyDied.AddListener(EnemyDied);
        } else {
            GetComponent<DefenceBuilding>().OnDefenceDestroyed.AddListener(DefenceBuildingDied);
        }
    }

    void Update() {
        if (healthBarInstance == null || healthBarInstance.GetComponent<Transform>() == null) { return; }
        hostPosition = new Vector3(transform.position.x, transform.position.y + healthBarCorrection);
        healthBarInstance.GetComponent<Transform>().position = Camera.main.WorldToScreenPoint(hostPosition);
        slider.value = GetComponent<IHealth>().GetHealth();
    }

    private void EnemyDied(int x, GameObject _) {
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }

    private void DefenceBuildingDied(Vector3 _) {
        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }
}
