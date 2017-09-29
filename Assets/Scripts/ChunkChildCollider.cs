using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkChildCollider : MonoBehaviour {

    Chunk chunk;

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Player") {
            chunk.CollisionWithPlayer();
        }
    }

    public void SetChunk(Chunk c) {
        chunk = c;
    }
}
