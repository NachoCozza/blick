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

    public int backgroundChunksPerLane = 40;

    float backgroundInterval;

    Chunk[] chunks;
    GameObject[] backgroundFirstLaneArray;
    GameObject[] backgroundSecondLaneArray;
    float interval = 0f;
    PerspectiveController perspective;
    PointsAndLevelManager level;

    public static float chunkSize = 15f;
    public static int chunkGroupSize = 5;

    public Material firstBackgroundMaterial;
    public Material secondBackgroundMaterial;


    int allTimeSpawnedChunks = 0;
    bool mustInstantiateSecondBackground = true; //ToDo make a more elegant solution
    bool mustMoveSecondLane = false;

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
        backgroundFirstLaneArray = new GameObject[backgroundChunksPerLane];
        backgroundSecondLaneArray = new GameObject[backgroundChunksPerLane];
        CalculateNewInterval();
        float initZ = perspective.player.transform.position.z - chunkSize / 2;
        InstantiateNewChunks(0, initZ);
        InstantiateNewBackground();
        StartCoroutine("SpawnNextAndDeleteLast");
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
    public void CalculateNewInterval() {
        interval = (chunkSize * totalChunks) / (2 * FloorMovement.SPEED);
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


    IEnumerator SpawnNextAndDeleteLast() {
        yield return new WaitForSeconds(interval);
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
            yield return new WaitForSeconds(interval);

        }
    }

    public void ArrangeBackground() {
        float firstBackZ = backgroundFirstLaneArray[backgroundChunksPerLane - 1].transform.position.z;
        float secondBackZ = backgroundSecondLaneArray[backgroundChunksPerLane - 1].transform.position.z;
        for (int i = 0; i < backgroundChunksPerLane / 2; i++) {
            firstBackZ += 35;
            PositionBackgroundChunk(i, firstBackZ, backgroundFirstLaneArray);
            if (mustMoveSecondLane) {
                secondBackZ += 35;
                PositionBackgroundChunk(i, secondBackZ, backgroundSecondLaneArray);
            }
        }
        mustMoveSecondLane = !mustMoveSecondLane;
    }

    private void PositionBackgroundChunk(int idx, float lastZ, GameObject[] backgroundArray) {
        int moveIdx = idx + backgroundChunksPerLane / 2;
        GameObject aux = backgroundArray[idx];
        aux.transform.position = new Vector3(aux.transform.position.x, aux.transform.position.y, lastZ);
        backgroundArray[idx] = backgroundArray[moveIdx];
        backgroundArray[moveIdx] = aux;
    }


    void InstantiateNewBackground() {
        float lastZ = 0;
        for (int i = 0; i < backgroundChunksPerLane * 2; i++) {
            bool firstLane = i < backgroundChunksPerLane;
            if (i == backgroundChunksPerLane) {
                lastZ = 0;
            }
            lastZ = InstantiateBackgroundChunk(i, lastZ, firstLane);
        }
        Debug.Log(lastZ);
    }

    private float InstantiateBackgroundChunk(int idx, float lastZ, bool firstLane) {
        int prefabIdx = Random.Range(0, backgroundPrefabs.Length);
        float randomHeight;
        GameObject aux = Instantiate(backgroundPrefabs[prefabIdx]);
        Vector3 auxPos;
        Transform parent;
        Material currentMaterial;
        GameObject[] parentArray;
        if (firstLane) {
            randomHeight = Random.Range(-2, 2);
            auxPos = firstBackgroundLane.position;
            if (lastZ == 0) {
                lastZ = auxPos.z;
            }
            parent = firstBackgroundLane;
            currentMaterial = firstBackgroundMaterial;
            parentArray = backgroundFirstLaneArray;
            if (idx == 0 || idx == backgroundChunksPerLane / 2) {
                Rigidbody r = aux.AddComponent<Rigidbody>();
                r.isKinematic = true;
                r.useGravity = false;
                BoxCollider col = aux.AddComponent<BoxCollider>();
                col.isTrigger = true;
                aux.tag = "BackgroundTrigger";
            }
        }
        else {
            randomHeight = Random.Range(-10, 10);
            auxPos = secondBackgroundLane.position;
            if (lastZ == 0) {
                lastZ = auxPos.z;
            }
            parent = secondBackgroundLane;
            currentMaterial = secondBackgroundMaterial;
            parentArray = backgroundSecondLaneArray;
            idx -= backgroundChunksPerLane;
        }
        auxPos.y += randomHeight;
        auxPos.z = lastZ + 35;
        aux.transform.position = auxPos;
        aux.GetComponent<BackgroundChunk>().isFrontLane = firstLane;
        aux.GetComponent<MeshRenderer>().material = currentMaterial;
        aux.name += idx;
        aux.transform.parent = parent;
        parentArray[idx] = aux;
        return aux.transform.position.z;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            ArrangeBackground();
        }
    }

}
