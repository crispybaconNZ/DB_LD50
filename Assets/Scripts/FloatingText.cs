using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour {
    [SerializeField, Tooltip("Colour to use for zero or positive values")] private Color _positiveColour = Color.green;
    [SerializeField, Tooltip("Colour to use for negative values")] private Color _negativeColour = Color.red;
    [SerializeField, Range(0.25f, 5.0f), Tooltip("How long the floating text remains on screen in seconds (0.25-5)")] private float _lifespan = 2.0f;
    [SerializeField, Range(-1f, 1f), Tooltip("Vertical offset from host to spawn the text at")] private float _yOffset = 0.55f;
    [SerializeField, Tooltip("Refence to the TMPro text field")] private TextMeshProUGUI _text;
    [SerializeField, Range(0.0f, 2.0f), Tooltip("Speed the text moves upwards at (0-2)")] private float _speed = 0.1f;

    private float _lifetime = 0.0f;

    void Start() {
        _lifetime = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y + _yOffset);
        _text.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    // Update is called once per frame
    void Update() {
        // see if time has expired
        _lifetime += Time.deltaTime;
        if (_lifetime >= _lifespan) {            
            Destroy(gameObject);
        }

        // still alive so move upwards
        transform.position = new Vector3(transform.position.x, transform.position.y + (_speed * Time.deltaTime));
        _text.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void SetValue(int value) {
        _text.color = (value <0) ? _negativeColour : _positiveColour;
        _text.text = value.ToString();
    }

    public void SetValueAndColour(int value, Color color) {
        _text.color = color;
        _text.text = value.ToString();
    }
}

