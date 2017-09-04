using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour
{

    public float speed;
    ChunkManager chunkManager;
    int lastChunkIndex = 0;
    Transform[] chunks;

    // Use this for initialization
    void Start()
    {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform f in chunks)
        {
            f.transform.position = new Vector3(f.transform.position.x, f.transform.position.y, f.transform.position.z - speed * Time.deltaTime);
        }

    }

}
