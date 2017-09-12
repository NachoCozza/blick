using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    public GameObject[] chunkPrefabs;
    public int totalChunks;

    Transform[] chunks;
    float interval = 0f;
    float chunkSize = 15f;
    PerspectiveController perspective;


    public Transform[] GetChunks()
    {
        return chunks;
    }

    public float GetChunkSize()
    {
        return chunkSize;
    }

    void Awake()
    {
        perspective = GetComponent<PerspectiveController>();
        chunks = new Transform[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2 + 2);
        float initZ = perspective.player.transform.position.z + chunkSize / 2;
        for (int i = 0; i < totalChunks; i++)
        {
            int prefabIdx = Random.Range(0, chunkPrefabs.Length);
            GameObject newChunk = Instantiate(chunkPrefabs[prefabIdx]);
            newChunk.transform.position = new Vector3(newChunk.transform.position.x, newChunk.transform.position.y, initZ + (i * chunkSize));//negrada
            chunks[i] = newChunk.transform;
        }
    }


    GameObject InstantiateNewChunk(float zFromDeletedChunk)
    {
        int prefabIdx = Random.Range(0, chunkPrefabs.Length);
        GameObject res = Instantiate(chunkPrefabs[prefabIdx]);
        res.transform.position = new Vector3(res.transform.position.x, res.transform.position.y, zFromDeletedChunk + totalChunks * chunkSize);
        return res;
    }

    IEnumerator SpawnNextAndDeleteLast()
    {
        WaitForSeconds wait = new WaitForSeconds(interval);
        yield return wait;
        while (true)
        {
            int newIndex = totalChunks / 2;
            float lastZ = 0;
            for (int i = 0; i < totalChunks / 2; i++)
            {
                Transform toDelete = chunks[i];
                lastZ = toDelete.transform.position.z;
                Destroy(toDelete.gameObject);
                chunks[i] = chunks[newIndex + i];
                GameObject newChunk = InstantiateNewChunk(lastZ);
                chunks[newIndex + i] = newChunk.transform;
            }
            perspective.AdjustNewChunks(newIndex);
            FloorMovement.lastChunkIndex = 0;
            yield return wait;

        }
    }

}
