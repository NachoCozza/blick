using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

    public GameObject[] chunkGroupPrefabs;
    public int totalChunks;

    Chunk[] chunks;
    float interval = 0f;
    float chunkSize = 15f;
    PerspectiveController perspective;
    int chunkGroupSize = 5;


    public Chunk[] GetChunks() {
        return chunks;
    }

    public float GetChunkSize() {
        return chunkSize;
    }

    void Awake() {
        perspective = GetComponent<PerspectiveController>();
        chunks = new Chunk[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2 + 2);
        float initZ = perspective.player.transform.position.z + chunkSize / 2;
        InstantiateNewChunks(0, initZ);
    }


    void InstantiateNewChunks(int startIndex, float lastZ) {
        for (int i = startIndex; i < totalChunks; i += chunkGroupSize) {
            int prefabIdx = Random.Range(0, chunkGroupPrefabs.Length);
            if (i == 0) {
                prefabIdx = 0; //Start in center. prefab idx = 0 should be walkable in 0,0,0
            }
            GameObject newChunkGroup = Instantiate(chunkGroupPrefabs[prefabIdx]);
            newChunkGroup.transform.position = new Vector3(newChunkGroup.transform.position.x, newChunkGroup.transform.position.y, lastZ + (i / chunkGroupSize * chunkSize * chunkGroupSize));
            for (int k = 0; k < chunkGroupSize; k++) {
                chunks[k + i] = newChunkGroup.transform.GetChild(k).gameObject.AddComponent<Chunk>();
            }
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
