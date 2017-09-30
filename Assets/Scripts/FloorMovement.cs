using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour {

    public float speed;
    public static int lastChunkIndex = 0;
    ChunkManager chunkManager;
    Chunk[] chunks;
    PointsAndLevelManager points;

    void Start() {
        lastChunkIndex = 0;
        chunkManager = GetComponent<ChunkManager>();
        points = GetComponent<PointsAndLevelManager>();
        chunks = chunkManager.GetChunks();
        StartCoroutine("UpdateLastChunkIndex");
    }

    // Update is called once per frame
    void FixedUpdate() {
		if (!PointsAndLevelManager.gameOver) {
			foreach (Chunk c in chunks) {
				c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z - speed * Time.deltaTime);
			}
		}
    }


    IEnumerator UpdateLastChunkIndex() {
        float interval = ChunkManager.chunkSize / speed - 0.01f;
        WaitForSeconds wait = new WaitForSeconds(interval);
        while (true) {
            yield return wait;
            points.AddChunk();
            lastChunkIndex++;
        }
    }

}
