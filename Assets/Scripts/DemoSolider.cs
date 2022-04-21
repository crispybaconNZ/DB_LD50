using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSolider : MonoBehaviour {

    private float speed = 2f;
    
    void Update() {
        float newX = transform.position.x - speed * Time.deltaTime;
        if (newX <= -10) { newX = 10; }
        transform.position = new Vector3(newX, transform.position.y);
    }
}
