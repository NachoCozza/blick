using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBackgroundTrigger : MonoBehaviour {
    public ChunkManager manager;
    bool doneFirst = false;
    void OnTriggerEnter(Collider c) {
        if (c.tag == "BackgroundTrigger") {
            if (doneFirst) {
                manager.ArrangeBackground();
            }
            else {
                doneFirst = true;
            }
        }
    }
}
