using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    public GameObject[] chunkPrefabs; //ToDo
    public int totalChunks;

    GameObject[] chunks;
    float interval = 0f;
    float chunkSize = 15f;
    PerspectiveController perspective;


    public GameObject[] GetChunks()
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
        chunks = new GameObject[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2);
        float initZ = perspective.player.transform.position.z;
        for (int i = 0; i < totalChunks; i++)
        {
            int prefabIdx = Random.Range(0, chunkPrefabs.Length);
            GameObject newChunk = Instantiate(chunkPrefabs[prefabIdx]);
            newChunk.transform.position = new Vector3(newChunk.transform.position.x, newChunk.transform.position.y, initZ + (i * chunkSize));//negrada
            chunks[i] = newChunk;
        }
    }

    private void Start()
    {
        StartCoroutine("SpawnNextAndDeleteLast");
    }


    GameObject InstantiateNewChunk(float zFromDeletedChunk)
    {
        return InstantiateNewChunk(zFromDeletedChunk, perspective.Is3D());
    }

    GameObject InstantiateNewChunk(float zFromDeletedChunk, bool is3D)
    {
        int prefabIdx = Random.Range(0, chunkPrefabs.Length);
        GameObject res = Instantiate(chunkPrefabs[prefabIdx]);
        res.transform.position = new Vector3(res.transform.position.x, res.transform.position.y, zFromDeletedChunk + totalChunks * chunkSize);
        return res;
    }

    IEnumerator SpawnNextAndDeleteLast()
    {
        yield return new WaitForSeconds(interval);
        while (true)
        {
            int newIndex = totalChunks / 2;
            float lastZ = 0;
            for (int i = 0; i < totalChunks / 2; i++)
            {
                GameObject toDelete = chunks[i];
                lastZ = toDelete.transform.position.z;
                Destroy(toDelete);
                chunks[i] = chunks[newIndex + i];
                GameObject newChunk = InstantiateNewChunk(lastZ);
                chunks[newIndex + i] = newChunk;
            }
            perspective.StoreAllPositions(newIndex);
            perspective.AdjustNewChunks(newIndex);
            yield return new WaitForSeconds(interval);

        }
    }

}
