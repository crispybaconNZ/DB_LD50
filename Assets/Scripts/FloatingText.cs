using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour {
    [SerializeField] private Color _positiveColour;
    [SerializeField] private Color _negativeColour;
    [SerializeField, Range(0.25f, 5.0f)] private float _lifespan = 2.0f;     // how long the floating text "survives" on screen
    [SerializeField, Range(-1f, 1f)] private float _yOffset = 0.55f;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Range(0.0f, 2.0f)] private float _speed = 0.1f;

    private float _lifetime = 0.0f;
    private Vector3 _worldPosition;

    void Start() {
        _lifetime = 0;
        _worldPosition = transform.position;
        _text.transform.position = Camera.main.WorldToScreenPoint(_worldPosition);
    }

    // Update is called once per frame
    void Update() {
        // see if time has expired
        _lifetime += Time.deltaTime;
        if (_lifetime >= _lifespan) {            
            Destroy(gameObject);
        }

        // still alive so move upwards
        _worldPosition = new Vector3(_worldPosition.x, _worldPosition.y + _yOffset + (_speed * Time.deltaTime));
        transform.position = _worldPosition;    // just move the GameObject, the text will move with it
    }

    public void SetValue(int value) {
        _text.color = (value <0) ? _negativeColour : _positiveColour;
        _text.text = value.ToString();
    }

    public void SetValue(string value) {
        _text.color = _positiveColour;
        _text.text = value.Trim();
    }
}

