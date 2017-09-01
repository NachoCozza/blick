using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour
{
    //ToDo optimize. unificar los 3 perspective
    public GameObject player;

    public GameObject cameraPerspective;
    public GameObject cameraRight;
    public GameObject cameraTop;

    GameObject[] chunks;

    Vector3 [][] originalPositions;


    BoxCollider2D playerCollider2D;
    BoxCollider playerCollider3D;
	Rigidbody playerRigidbody3D;
	Rigidbody2D playerRigidbody2D;
	bool is3D = false;

	ChunkManager chunkManager;

	public bool Is3D() { return is3D; }

    enum LastView { Persp, Top, Right };
    LastView lastView = LastView.Persp;

    void Start()
    {
		chunkManager = GetComponent<ChunkManager> ();
        chunks = chunkManager.GetChunks();
        originalPositions = new Vector3[chunks.Length][];
        StoreAllPositions();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeTo3D();
			return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeTo2D(true);
			return;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeTo2D(false);
			return;
        }
    }

    void ChangeTo3D()
    {
        chunks = chunkManager.GetChunks();
        cameraTop.SetActive(false);
        cameraRight.SetActive(false);
        cameraPerspective.SetActive(true);

        //Todo poner los bloques como estaban antes y alinear al jugador a un bloque posible
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++)
        {
            int childIdx = 0;
            foreach (Transform child in chunks[chunkIdx].transform)
            {
                Vector3 newPos = originalPositions[chunkIdx][childIdx];
                newPos.z = child.position.z;
                child.position = newPos;
                childIdx++;
            }
        }
        lastView = LastView.Persp;
    }

    void ChangeTo2D(bool right)
    {
        Vector3 multiplier;
        cameraPerspective.SetActive(false);
        if (right)
        {
            cameraRight.SetActive(true);
            cameraTop.SetActive(false);
            multiplier = new Vector3(0, 1, 1);
        }
        else
        {
            cameraTop.SetActive(true);
            cameraRight.SetActive(false);
            multiplier = new Vector3(1, 0, 1);
        }
        chunks = chunkManager.GetChunks();
        //itero todos los chunks, guardo la posicion y alineo todos los hijos
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx ++)
        {
            int childIdx = 0;
            foreach (Transform child in chunks[chunkIdx].transform)
            {
                Vector3 newPos = Vector3.Scale(originalPositions[chunkIdx][childIdx], multiplier);
                newPos.z = child.position.z;
                child.position = newPos;
                childIdx++; 
            }
        }
        lastView = right ? LastView.Right : LastView.Top;
    }

    public void StoreAllPositions()
    {
        chunks = chunkManager.GetChunks();
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++)
        {
            int childIdx = 0;
            originalPositions[chunkIdx] = new Vector3[chunks[chunkIdx].transform.childCount];
            foreach (Transform child in chunks[chunkIdx].transform)
            {
                originalPositions[chunkIdx][childIdx] = child.position;
                childIdx++;
            }
        }
    }	
}
