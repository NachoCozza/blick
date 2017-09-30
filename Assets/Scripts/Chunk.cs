using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Vector3 originalPosition;
    static PerspectiveController PERSPECTIVE;
    static Vector3 TOP = new Vector3(1, 0, 1);
    static Vector3 RIGHT = new Vector3(0, 1, 1);
    public View myView;

    int STANDING_INSTANCE_ID;

    // Use this for initialization
    void Start() {
        if (PERSPECTIVE == null) {
            PERSPECTIVE = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();
        }
        originalPosition = GetTransform().position;
        AdjustCurrentPosition(View.Unknwn, PERSPECTIVE.currentView, true);
        transform.GetChild(0).gameObject.AddComponent<ChunkChildCollider>().SetChunk(this);
    }

    public void AdjustCurrentPosition(View oldView, View newView, bool isStart) {
        Transform child = GetTransform();
        Vector3 newPos = originalPosition;
        newPos.z = child.position.z;
        switch (oldView) {
            case View.Persp:
                if (newView == View.Right && !isStart) {
                    child.position = Vector3.Scale(newPos, RIGHT);
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


    public void MovePlayer(GameObject player)
    {
        Vector3 aux = GetTransform().position;
        if (myView != View.Top)
        {
            aux.y = player.transform.position.y;
        }
        aux.z = player.transform.position.z;
        player.transform.position = aux;

    }
    public void CollisionWithPlayer() {
        STANDING_INSTANCE_ID = GetInstanceID();
    }

    public void MovePlayer(GameObject player, View oldView) {
        if (oldView == myView || IsStandingOnMe()) {
            Vector3 aux = GetTransform().position;
            if (myView != View.Top) {
                aux.y = player.transform.position.y;
            }
            aux.z = player.transform.position.z;
            player.transform.position = aux;
        }
    }

    bool IsStandingOnMe() {
        return GetInstanceID() == STANDING_INSTANCE_ID;
    }

}
