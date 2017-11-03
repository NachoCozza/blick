using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

    public GameObject initialChunkGroup;
    public GameObject tutorialChunkGroup;

    public GameObject[] chunkGroupsEasy;
    public GameObject[] chunkGroupsMedium;
    public GameObject[] chunkGroupsHard;


    public GameObject[] backgroundPrefabs;
    public int totalChunks;

    public Transform firstBackgroundLane;
    public Transform secondBackgroundLane;
    public float firstLaneMovementSpeed;
    public float secondLaneMovementSpeed;

    float backgroundInterval;

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
    bool mustInstantiateSecondBackground = true; //ToDo make a more elegant solution

    TutorialController tutorial;


    public Chunk[] GetChunks() {
        return chunks;
    }

    void Start() {
        allTimeSpawnedChunks = 0;
        perspective = GetComponent<PerspectiveController>();
        level = GetComponent<PointsAndLevelManager>();
        tutorial = GetComponent<TutorialController>();
        chunks = new Chunk[totalChunks];
        interval = (chunkSize / GetComponent<FloorMovement>().speed) * (totalChunks / 2);
        backgroundInterval = (35 / firstLaneMovementSpeed) * (56);
        float initZ = perspective.player.transform.position.z + chunkSize / 2;
        InstantiateNewChunks(0, initZ);
        InstantiateNewBackground();
        StartCoroutine("SpawnNextAndDeleteLast");
        StartCoroutine("SpawnBackground");
    }


    void InstantiateNewChunks(int startIndex, float lastZ) {
        GameObject[] currentChunkGroups;
        for (int i = startIndex; i < totalChunks; i += chunkGroupSize) {
            Difficulty difficulty = level.GetDifficulty(allTimeSpawnedChunks);
            currentChunkGroups = GetCurrentChunkGroup(difficulty);
            ChunkGroup newChunkGroup;
            if (i == 0) {
                newChunkGroup = Instantiate(initialChunkGroup).GetComponent<ChunkGroup>();
            }
            else {
                if (i == chunkGroupSize && !tutorial.Finished()) {
                    newChunkGroup = Instantiate(tutorialChunkGroup).GetComponent<ChunkGroup>();
                }
                else {
                    int prefabIdx = Random.Range(0, currentChunkGroups.Length);
                    newChunkGroup = Instantiate(currentChunkGroups[prefabIdx]).GetComponent<ChunkGroup>();
                }
            }
            newChunkGroup.transform.position = new Vector3(newChunkGroup.transform.position.x, newChunkGroup.transform.position.y, lastZ);
            for (int k = 0; k < chunkGroupSize; k++) {
                chunks[k + i] = newChunkGroup.AddChunkBehaviour(k);
            }
            if (i > 0) {
                allTimeSpawnedChunks += chunkGroupSize;
            }
            lastZ += chunkGroupSize * chunkSize;
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
                switch (random) {
                    case 0:
                        return chunkGroupsMedium;
                    case 1:
                    default:
                        return chunkGroupsHard;
                }
        }
    }

    IEnumerator SpawnNextAndDeleteLast() {
        WaitForSeconds wait = new WaitForSeconds(interval);
        yield return wait;
        while (true) {
            int newIndex = totalChunks / 2;
            for (int i = 0; i < newIndex; i++) {
                Chunk toDelete = chunks[i];
                Destroy(toDelete.gameObject);
                chunks[i] = chunks[newIndex + i];
            }
            float lastZ = chunks[newIndex - 1].transform.position.z + chunkSize;
            FloorMovement.lastChunkIndex -= newIndex;
            InstantiateNewChunks(newIndex, lastZ);
            //perspective.SetChunks(this.chunks);
            yield return wait;

        }
    }

    IEnumerator SpawnBackground() {
        WaitForSeconds wait = new WaitForSeconds(backgroundInterval);
        yield return wait;
        while (true) {
            InstantiateNewBackground();
            yield return wait;
        }
    }

    void InstantiateNewBackground() {
        float secondZ = 0;
        float firstZ = 0;
        //TODO optimize to only one loop
        if (backgroundInstances != null && backgroundInstances.Length > 0) {
            int upTo = backgroundChunksPerLane;
            if (mustInstantiateSecondBackground) {
                upTo *= 2;
            }
            for (int i = 0; i < backgroundChunksPerLane * 2; i++) {
                if (backgroundInstances[i] != null) {
                    Destroy(backgroundInstances[i]);
                }
                backgroundInstances[i] = backgroundInstances[i + backgroundChunksPerLane * 2];
            }
            firstZ = backgroundInstances[backgroundChunksPerLane - 1].transform.position.z;
            secondZ = backgroundInstances[backgroundChunksPerLane * 2 - 1].transform.position.z;
        }
        else {
            backgroundInstances = new GameObject[backgroundChunksPerLane * 4];
        }

        for (int i = backgroundChunksPerLane * 2; i < (backgroundChunksPerLane * 3); i++) {
            InstantiateBackgroundChunk(i, firstZ, true);
            if (mustInstantiateSecondBackground) {
                InstantiateBackgroundChunk(i + backgroundChunksPerLane, secondZ, false);
                secondZ += 35;
            }
            firstZ += 35;
        }
        mustInstantiateSecondBackground = !mustInstantiateSecondBackground;
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
