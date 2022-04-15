using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour {
    private PlayerControls playerControls;
    private Transform cam;
    [SerializeField] private float camSpeed = 1.0f;
    private const float minX = 3.89f;
    private const float maxX = 16.13f;

    private void Awake() {
        playerControls = new PlayerControls();
        cam = GetComponent<Transform>();
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    void Start() {

    }

    void Update() {
        float move = playerControls.Player.Move.ReadValue<float>();
        if (move != 0) {
            move = move * Time.deltaTime * camSpeed;
            cam.position = cam.position + new Vector3(move, 0, 0);
            if (cam.position.x < minX) { cam.position = new Vector3(minX, cam.position.y, cam.position.z); }
            if (cam.position.x > maxX) { cam.position = new Vector3(maxX, cam.position.y, cam.position.z); }
        }
        
    }
}
