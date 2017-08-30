using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

	public GameObject [] floors; //ToDo borrar y que sea todo dinamico
	public GameObject [] chunkPrefabs; //ToDo
    public int totalChunks;

	GameObject [] chunks;
	float interval = 0f;
    float chunkSize = 10f;

    // Use this for initialization
    void Awake() {
        chunks = new GameObject[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2);
        float initZ = GetComponent<PerspectiveController>().player.transform.position.z;
        for (int i = 0; i < totalChunks; i++)
        {
            int prefabIdx = Random.Range(0, chunkPrefabs.Length); //ToDo quen levante todos
            GameObject newChunk = Instantiate(chunkPrefabs[prefabIdx]);
            Debug.Log("Z" + initZ + (i * chunkSize));
            newChunk.transform.position = new Vector3(newChunk.transform.position.x, newChunk.transform.position.y, initZ + (i * chunkSize));
            Debug.Log(newChunk.transform.position);
            chunks[i] = newChunk;
        }
        Debug.Log(interval);
	}

    private void Start()
    {
        StartCoroutine("SpawnNextAndDeleteLast");
    }

    public GameObject [] GetChunks() {
        return chunks;
	}

    public float GetChunkSize()
    {
        return chunkSize;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SpawnNextAndDeleteLast() {
        yield return new WaitForSeconds(interval);
        while (true) {
            int newIndex = totalChunks / 2;
            float lastZ = 0;
            for (int i =0; i < totalChunks / 2; i ++)
            {
                GameObject toDelete = chunks[i];
                lastZ = toDelete.transform.position.z;
                Destroy(toDelete);
                chunks[i] = chunks[newIndex + i];
                int prefabIdx = Random.Range(0, chunkPrefabs.Length); //ToDo quen levante todos
                GameObject newChunk = Instantiate(chunkPrefabs[prefabIdx]);
                newChunk.transform.position = new Vector3(newChunk.transform.position.x, newChunk.transform.position.y, lastZ + totalChunks * chunkSize);
                chunks[newIndex + i] = newChunk;
            }
			yield return new WaitForSeconds (interval);
            
		}
	}

}
