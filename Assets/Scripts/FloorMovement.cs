using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMovement : MonoBehaviour
{

    public float speed;
    public static int lastChunkIndex = 0;
    ChunkManager chunkManager;
    Transform[] chunks;
    bool started = false;
    // Use this for initialization
    void Start()
    {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
        StartCoroutine("StartTimer");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (started)
        {
            foreach (Transform f in chunks)
            {
                f.transform.position = new Vector3(f.transform.position.x, f.transform.position.y, f.transform.position.z - speed * Time.deltaTime);
            }

        }
    }

    IEnumerator StartTimer()
    {
        //for (int i = 0; i < 3; i ++)
        //{
        //    //Todo timer 
        //    yield return new WaitForSeconds(1);
        //}
        yield return null;
        started = true;
        StartCoroutine("UpdateLastChunkIndex");
        chunkManager.StartCoroutine("SpawnNextAndDeleteLast");
    }

    IEnumerator UpdateLastChunkIndex()
    {
        float interval = chunkManager.GetChunkSize() / speed - 0.01f;
        WaitForSeconds wait = new WaitForSeconds(interval);
        while (true)
        {
            yield return wait;
            lastChunkIndex++;
            //Time.timeScale = 0;
            //yield return new WaitForSecondsRealtime(2);
            //Time.timeScale = 1;
        }
    }

}
