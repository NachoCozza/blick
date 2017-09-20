using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Vector3 originalPosition;
    static PerspectiveController PERSPECTIVE;
    static Vector3 TOP = new Vector3(1, 0, 1);
    static Vector3 RIGHT = new Vector3(0, 1, 1);

    // Use this for initialization
    void Start() {
        if (PERSPECTIVE == null) {
            PERSPECTIVE = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();
        }
        originalPosition = transform.GetChild(0).position;
        AdjustCurrentPosition();
    }

    public void AdjustCurrentPosition() {
        Transform child = transform.GetChild(0);
        Vector3 newPos = originalPosition;
        newPos.z = child.position.z;
        switch (PERSPECTIVE.currentView) {
            case View.Persp:
                child.position = newPos;
                break;
            case View.Top:
                child.position = Vector3.Scale(newPos, TOP);
                break;
            case View.Right:
                child.position = Vector3.Scale(newPos, RIGHT);
                break;
        }
    }

    public Transform getTransform() {
        return transform.GetChild(0);
    }
}
