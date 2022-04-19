using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EntityDied : UnityEvent { }

public class Utils {
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, 
        Color colour = default(Color), TextAnchor textAnchor=TextAnchor.MiddleCenter, 
        TextAlignment textAlignment=TextAlignment.Center, int sortingOrder=0) {
        if (colour == null) { colour = Color.white; }

        return CreateWorldText(parent, text, localPosition, fontSize, (Color)colour, textAnchor, textAlignment, sortingOrder);
    }

    public static void Delay(int seconds) {
        if (seconds <= 0) { return; }

        float elapsedTime = 0f;
        while (elapsedTime <= seconds) {
            Debug.Log("Delay() elapsed time: " + elapsedTime);
            elapsedTime += Time.deltaTime;
        }

        return;
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color colour, TextAnchor textAnchor,
        TextAlignment textAlignment, int sortingOrder) {
        GameObject gameObject = new GameObject("World Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = colour;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
