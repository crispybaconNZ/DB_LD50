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
    [SerializeField, Tooltip("Reference to player manager")] private PlayerManager _playerManager;

    private int currentlySelectedDefence = 0;   // keep track of the last defence the player had selected in the menu
    
    private UIManager _uiManager;

    void Start() {
        // _playerManager = GameObject.Find("PlayerBase").GetComponent<PlayerManager>();
        _uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        DefenceBuilding db = defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<DefenceBuilding>();
        SetDefenceBuilding(db);
        SetPageNumber();
    }

    void Update() {
        SetDefenceBuilding(defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<DefenceBuilding>());
        SetPageNumber();

        if (_playerManager.playerControls.Player.ScrollDefences.WasPerformedThisFrame()) {            
            float scrollValue = _playerManager.playerControls.Player.ScrollDefences.ReadValue<Vector2>().y;
            if (scrollValue < 0) {
                currentlySelectedDefence++;
                if (currentlySelectedDefence >= defenceBuildingPrefabs.Length) { currentlySelectedDefence = 0; }
            } else if (scrollValue > 0) {
                currentlySelectedDefence--;
                if (currentlySelectedDefence < 0) {  currentlySelectedDefence = defenceBuildingPrefabs.Length - 1; }
            }
            SetDefenceBuilding(defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<DefenceBuilding>());
            SetPageNumber();
        }

        if (_playerManager.playerControls.Player.Select.WasPerformedThisFrame() && CanAfford()) {
            _playerManager.OnDefenceSelected.Invoke(defenceBuildingPrefabs[currentlySelectedDefence]);
            _uiManager.ShowBuildMenu(false);
        }
        
        if (_playerManager.playerControls.Player.Cancel.WasPerformedThisFrame() || _playerManager.playerControls.Player.BuildMenu.WasPerformedThisFrame()) {
            _uiManager.ShowBuildMenu(false);
        }
        
    }

    private void SetDefenceBuilding(DefenceBuilding def) {
        _nameField.text = def.name;
        _nameShadowField.text = def.name;
        _imageField.sprite = def.gameObject.GetComponent<SpriteRenderer>().sprite;
        _descriptionField.text = def._description;
        if (CanAfford()) {
            _costField.text = def.GetCost().ToString();
        } else {
            _costField.text = "N/A";
        }
    }

    private void SetPageNumber() {
        _pageField.text = (currentlySelectedDefence + 1).ToString() + " of " + defenceBuildingPrefabs.Length.ToString();
    }

    private bool CanAfford() {
        return defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<DefenceBuilding>().GetCost() < _playerManager.GetScore();
    }
}
