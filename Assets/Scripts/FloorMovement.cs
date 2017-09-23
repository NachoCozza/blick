using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour {

    public float speed;
    public static int lastChunkIndex = 0;
    ChunkManager chunkManager;
    Chunk[] chunks;
    bool started = false;
    // Use this for initialization
    void Start() {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
        StartCoroutine("UpdateLastChunkIndex");
        chunkManager.StartCoroutine("SpawnNextAndDeleteLast");
    }

    // Update is called once per frame
    void FixedUpdate() {

        foreach (Chunk c in chunks) {
            c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z - speed * Time.deltaTime);
        }


    }

    IEnumerator UpdateLastChunkIndex() {
        float interval = ChunkManager.chunkSize / speed - 0.01f;
        WaitForSeconds wait = new WaitForSeconds(interval);
        while (true) {
            yield return wait;
            lastChunkIndex++;
        }
    }

}
