using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenu : MonoBehaviour {
    [Tooltip("Array of available defence buildings")] public GameObject[] defenceBuildingPrefabs;
    [SerializeField, Tooltip("Refence to TMPro text field for defence's name")] private TextMeshProUGUI _nameField;
    [SerializeField, Tooltip("Refence to TMPro text field for defence's name's shadow")] private TextMeshProUGUI _nameShadowField;
    [SerializeField, Tooltip("Refence to TMPro text field for defence's description")] private TextMeshProUGUI _descriptionField;
    [SerializeField, Tooltip("Refence to TMPro text field for defence's cost")] private TextMeshProUGUI _costField;
    [SerializeField, Tooltip("Refence to TMPro text field for page number")] private TextMeshProUGUI _pageField;
    [SerializeField, Tooltip("Reference to defence's image")] private Image _imageField;

    private int currentlySelectedDefence = 0;   // keep track of the last defence the player had selected in the menu
    
    private UIManager _uiManager;

    void Start() {
    }

    void Update() {
    }

    private void SetDefenceBuilding(DefenderSO def) {
        _nameField.text = def.Name;
        _nameShadowField.text = def.Name;
        _imageField.sprite = def.MenuSprite;
        _descriptionField.text = def.Description;
        if (CanAfford()) {
            _costField.text = def.BaseStats.Cost.ToString();
        } else {
            _costField.text = "N/A";
        }
    }

    private void SetPageNumber() {
        _pageField.text = (currentlySelectedDefence + 1).ToString() + " of " + defenceBuildingPrefabs.Length.ToString();
    }

    private bool CanAfford() {
        return defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<BaseUnit>().Stats.Cost < 100;
    }
}
