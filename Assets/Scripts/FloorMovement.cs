using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour
{

    public float speed;
    ChunkManager chunkManager;

    // Use this for initialization
    void Start()
    {
        chunkManager = GetComponent<ChunkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] floors = chunkManager.GetChunks();
        foreach (GameObject f in floors)
        {
            f.transform.position = new Vector3(f.transform.position.x, f.transform.position.y, f.transform.position.z - speed * Time.deltaTime);
        }
    }

}
