using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour {

    public static float SPEED;
    public static float FIRST_LANE_SPEED;

    public float speed;
    public float firstLaneSpeed;
    public static int lastChunkIndex;
    ChunkManager chunkManager;
    Chunk[] chunks;
    PointsAndLevelManager points;

    private void Awake() {
        SPEED = speed;
        FIRST_LANE_SPEED = firstLaneSpeed;
    }

    void Start() {
        lastChunkIndex = 1;
        chunkManager = GetComponent<ChunkManager>();
        points = GetComponent<PointsAndLevelManager>();
        chunks = chunkManager.GetChunks();
        StartCoroutine("UpdateLastChunkIndex");
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!PointsAndLevelManager.gameOver) {
            foreach (Chunk c in chunks) {
                c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z - SPEED * Time.deltaTime);
            }
        }
    }

    public void SetNewSpeed(float newSpeed) {
        SPEED = newSpeed;
        chunkManager.CalculateNewInterval();
    }

    IEnumerator UpdateLastChunkIndex() {
        float interval = ChunkManager.chunkSize / SPEED - 0.01f;
        WaitForSeconds wait = new WaitForSeconds(interval);
        while (true) {
            yield return wait;
            points.AddChunk();
            lastChunkIndex++;
        }
    }

}
