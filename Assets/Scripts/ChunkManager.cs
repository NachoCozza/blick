using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

    public GameObject[] ChunkGroupsEasy;
    public GameObject[] ChunkGroupsMedium;
    public GameObject[] ChunkGroupsHard;
    public GameObject[] ChunkGroupsImpossible;
    public int totalChunks;

    Chunk[] chunks;
    float interval = 0f;
    PerspectiveController perspective;
    PointsAndLevelManager level;

    public static float chunkSize = 15f;
    public static int chunkGroupSize = 5;

    int allTimeSpawnedChunks = 0;


    public Chunk[] GetChunks() {
        return chunks;
    }

    void Start() {
        allTimeSpawnedChunks = 0;
        perspective = GetComponent<PerspectiveController>();
        level = GetComponent<PointsAndLevelManager>();
        chunks = new Chunk[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2 + 2);
        float initZ = perspective.player.transform.position.z + chunkSize / 2;
        InstantiateNewChunks(0, initZ);
		StartCoroutine("SpawnNextAndDeleteLast");
    }


    void InstantiateNewChunks(int startIndex, float lastZ) {
        GameObject[] currentChunkGroups;
        for (int i = startIndex; i < totalChunks; i += chunkGroupSize) {
            Difficulty difficulty = level.GetDifficulty(allTimeSpawnedChunks);
            currentChunkGroups = GetCurrentChunkGroup(difficulty);
            int prefabIdx = Random.Range(0, currentChunkGroups.Length);
            if (i == 0) {
                prefabIdx = 0; //Start in center. prefab idx = 0 should be walkable in 0,0,0
            }
            ChunkGroup newChunkGroup = Instantiate(currentChunkGroups[prefabIdx]).GetComponent<ChunkGroup>();
            newChunkGroup.transform.position = new Vector3(newChunkGroup.transform.position.x, newChunkGroup.transform.position.y, lastZ + (i / chunkGroupSize * chunkSize * chunkGroupSize));
            for (int k = 0; k < chunkGroupSize; k++) {
                chunks[k + i] = newChunkGroup.AddChunkBehaviour(k);
            }
            allTimeSpawnedChunks += chunkGroupSize;
        }
    }

    GameObject[] GetCurrentChunkGroup(Difficulty difficulty) {
        switch (difficulty) {
            case Difficulty.Easy:
            default:
                return ChunkGroupsEasy;
            case Difficulty.Medium:
                return ChunkGroupsMedium;
            case Difficulty.Hard:
                return ChunkGroupsHard;
            case Difficulty.Impossible:
                return ChunkGroupsImpossible;
        }
    }

    IEnumerator SpawnNextAndDeleteLast() {
        WaitForSeconds wait = new WaitForSeconds(interval);
        yield return wait;
        while (true) {
            int newIndex = totalChunks / 2;
            float lastZ = 0;
            for (int i = 0; i < newIndex; i++) {
                Chunk toDelete = chunks[i];
                lastZ = toDelete.transform.position.z;
                Destroy(toDelete.gameObject);
                chunks[i] = chunks[newIndex + i];
            }
            lastZ += chunkSize;
            FloorMovement.lastChunkIndex -= newIndex;
            InstantiateNewChunks(newIndex, lastZ);
            //perspective.SetChunks(this.chunks);
            yield return wait;

        }
    }

}
