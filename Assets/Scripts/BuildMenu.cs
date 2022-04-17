using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenu : MonoBehaviour {
    public GameObject[] defenceBuildingPrefabs;
    [SerializeField] private TextMeshProUGUI _nameField;
    [SerializeField] private TextMeshProUGUI _nameShadowField;
    [SerializeField] private TextMeshProUGUI _descriptionField;
    [SerializeField] private TextMeshProUGUI _costField;
    [SerializeField] private TextMeshProUGUI _pageField;
    [SerializeField] private Image _imageField;

    private int currentlySelectedDefence = 0;   // keep track of the last defence the player had selected in the menu
    private PlayerManager _playerManager;
    private UIManager _uiManager;

    void Start() {
        _playerManager = GameObject.Find("PlayerBase").GetComponent<PlayerManager>();
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
            _costField.text = def.cost.ToString();
        } else {
            _costField.text = "Too much!";
        }
    }

    private void SetPageNumber() {
        _pageField.text = (currentlySelectedDefence + 1).ToString() + " of " + defenceBuildingPrefabs.Length.ToString();
    }

    private bool CanAfford() {
        return defenceBuildingPrefabs[currentlySelectedDefence].GetComponent<DefenceBuilding>().cost < _playerManager.GetScore();
    }
}
