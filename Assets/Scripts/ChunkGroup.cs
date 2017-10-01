using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGroup : MonoBehaviour {

    public GameObject[] chunkPrefabs;
    private GameObject[] chunks;
    // Use this for initialization
    void Awake() {
        if (chunkPrefabs.Length != ChunkManager.chunkGroupSize) {
            Debug.LogError("Chunk group cannot have different than " + ChunkManager.chunkGroupSize + " chunks");
        }
        chunks = new GameObject[chunkPrefabs.Length];
        Vector3 auxPos;
        for (int i = 0; i < chunkPrefabs.Length; i ++) {
            chunks[i] = Instantiate(chunkPrefabs[i]);
			chunks[i].transform.parent = transform;
            chunks[i].transform.localPosition = Vector3.zero;
            if (i > 0) {
                auxPos = chunks[i].transform.localPosition;
                auxPos.z = chunks[i - 1].transform.localPosition.z + ChunkManager.chunkSize;
                chunks[i].transform.localPosition = auxPos;
            }
        }
    }

    public Chunk AddChunkBehaviour(int idx) {
        string auxName = chunks[idx].name;
        View view = View.Persp;
        if (auxName.StartsWith(("P"))) {
            view = View.Persp;
        }
        if (auxName.StartsWith("R")) {
            view = View.Right;
        }
        if (auxName.StartsWith(("T"))) {
            view = View.Top;
        }
        Chunk c = chunks[idx].AddComponent<Chunk>();
        c.myView = view;
        return c;
    }

}
