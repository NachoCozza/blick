using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

    public GameObject[] chunkGroupsEasy;
    public GameObject[] chunkGroupsMedium;
    public GameObject[] chunkGroupsHard;

    public GameObject[] backgroundPrefabs;
    public int totalChunks;

    public Transform firstBackgroundLane;
    public Transform secondBackgroundLane;
    public float firstLaneMovementSpeed;
    public float secondLaneMovementSpeed;

    public int backgroundChunksPerLane = 40;

    Chunk[] chunks;
    GameObject[] backgroundInstances;
    float interval = 0f;
    PerspectiveController perspective;
    PointsAndLevelManager level;

    public static float chunkSize = 15f;
    public static int chunkGroupSize = 5;

    public Material firstBackgroundMaterial;
    public Material secondBackgroundMaterial;

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
        InstantiateNewBackground();
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
                return chunkGroupsEasy;
            case Difficulty.Medium:
                return chunkGroupsMedium;
            case Difficulty.Hard:
                return chunkGroupsHard;
            case Difficulty.Impossible:
                int random = Random.Range(0, 1);
                switch(random) {
                    case 0:
                        return chunkGroupsMedium;
                    case 1: default:
                        return chunkGroupsHard;
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
            InstantiateNewBackground(); 
            //perspective.SetChunks(this.chunks);
            yield return wait;

        }
    }

    void InstantiateNewBackground() {
        float lastZ = 0;
        if (backgroundInstances != null && backgroundInstances.Length > 0) {
            for (int i = 0; i < backgroundChunksPerLane * 2; i ++) {
                Destroy(backgroundInstances[i]);
                backgroundInstances[i] = backgroundInstances[i + backgroundChunksPerLane];
			}
            lastZ = backgroundInstances[backgroundChunksPerLane - 1].transform.position.z;
        }
        else {
            backgroundInstances = new GameObject[backgroundChunksPerLane * 4];
        }


        //Debug.Log(backgroundInstances.Length);

        for (int i = backgroundChunksPerLane * 2; i < (backgroundChunksPerLane * 3); i++)
        {
            InstantiateBackgroundChunk(i, lastZ, true);
            InstantiateBackgroundChunk(i + backgroundChunksPerLane, lastZ, false);
            lastZ += 35;   
        }
    }

    private void InstantiateBackgroundChunk(int idx, float lastZ, bool firstLane) {
		int prefabIdx = Random.Range(0, backgroundPrefabs.Length);
        float randomHeight;
		GameObject aux = Instantiate(backgroundPrefabs[prefabIdx]);
        Vector3 auxPos;
        Transform parent;
        Material currentMaterial;
        float movementSpeed;
        if (firstLane) {
            randomHeight = Random.Range(-2, 2);
			auxPos = firstBackgroundLane.position;
            movementSpeed = firstLaneMovementSpeed;
            parent = firstBackgroundLane;
            currentMaterial = firstBackgroundMaterial;
        }
        else {
            randomHeight = Random.Range(-10, 10);
            auxPos = secondBackgroundLane.position;
            movementSpeed = secondLaneMovementSpeed;
            parent = secondBackgroundLane;
            currentMaterial = secondBackgroundMaterial;
        }
		auxPos.y += randomHeight;
		auxPos.z = lastZ;
		aux.transform.position = auxPos;
        aux.AddComponent<BackgroundChunk>().movementSpeed = movementSpeed;
        aux.GetComponent<MeshRenderer>().material = currentMaterial;
        aux.name += idx;
        aux.transform.parent = parent;
		backgroundInstances[idx] = aux;
		
    }

}
