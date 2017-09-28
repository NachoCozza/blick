﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Vector3 originalPosition;
    static PerspectiveController PERSPECTIVE;
    static Vector3 TOP = new Vector3(1, 0, 1);
    static Vector3 RIGHT = new Vector3(0, 1, 1);
    public View myView;

    // Use this for initialization
    void Start() {
        if (PERSPECTIVE == null) {
            PERSPECTIVE = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();
        }
        originalPosition = GetTransform().position;
        AdjustCurrentPosition(View.Unknwn, PERSPECTIVE.currentView, true);
    }

    public void AdjustCurrentPosition(View oldView, View newView, bool isStart) {
        //Debug.Log(isStart);
        //Debug.Log("My name is " + name + " and my view is " + view);
        Transform child = GetTransform();
        Vector3 newPos = originalPosition;
        newPos.z = child.position.z;
        switch (oldView) {
            case View.Persp:
                if (newView == View.Right && !isStart) {
                    child.position = Vector3.Scale(newPos, RIGHT);
                    //child.position = newPos;    
                }
                if (newView == View.Top) {
                    if (isStart) {
                        if (myView == View.Top || myView == View.Right) {
                            child.position = newPos;
						}
                    }
                    else {
                        if (myView == View.Top) {
                            child.position = Vector3.Scale(newPos, TOP);
                        }
                    }
                }
                break;
            case View.Top:
                if (newView == View.Persp) {
                    child.position = newPos;
                }
                if (newView == View.Right) {
                    if (isStart && (myView == View.Persp || myView == View.Top)) {
                        child.position = newPos;
                    }
                    if (!isStart && myView == View.Right) {
                        child.position = Vector3.Scale(newPos, RIGHT);
                    }
                }
                break;
            case View.Right:
                if (newView == View.Persp && isStart) {
                    child.position = newPos;
                }
                if (newView == View.Top) {
                    if (isStart && (myView == View.Persp || myView == View.Right)) {
                        child.position = newPos;
                    }
                    if (!isStart && myView == View.Top) {
                        child.position = Vector3.Scale(newPos, TOP);
                    }
                }
                break;
        }
    }

    public Transform GetTransform() {
        return transform.GetChild(0);
    }

    public void MovePlayer(GameObject player) {
        //ToDo do it when you have to, based on the code in adjustCurrentPosiion
		Vector3 aux = GetTransform().position;
		aux.z = player.transform.position.z;
		player.transform.position = aux;
    }

}
