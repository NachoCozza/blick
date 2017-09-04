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

    enum View { Persp, Top, Right };
    View currentView = View.Persp;

    void Start()
    {
		chunkManager = GetComponent<ChunkManager> ();
        chunks = chunkManager.GetChunks();
        originalPositions = new Vector3[chunks.Length][];
        StoreAllPositions(0);
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
        IterateChunksAndArrange(0, -1 * Vector3.one);
        currentView = View.Persp;
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
        IterateChunksAndArrange(0, multiplier);
        currentView = right ? View.Right : View.Top;
    }

    public void StoreAllPositions(int startIdx)
    {
        chunks = chunkManager.GetChunks();
        for (int chunkIdx = startIdx; chunkIdx < chunks.Length; chunkIdx++)
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

    private void IterateChunksAndArrange(int startIndex, Vector3 multiplier)
    {
        for (int chunkIdx = startIndex; chunkIdx < chunks.Length; chunkIdx++)
        {
            int childIdx = 0;
            Vector3 newPos;
            foreach (Transform child in chunks[chunkIdx].transform)
            {
                if (multiplier == -Vector3.one)
                {
                    newPos = originalPositions[chunkIdx][childIdx];
                }
                else
                {
                    newPos = Vector3.Scale(originalPositions[chunkIdx][childIdx], multiplier);
                }
                newPos.z = child.position.z;
                child.position = newPos;
                childIdx++;
            }
        }
    }

    public void AdjustNewChunks(int startIndex)
    {
        Vector3 multiplier = currentView == View.Right ? new Vector3(0, 1, 1) : new Vector3(1, 0, 1);
        IterateChunksAndArrange(startIndex - 1, multiplier);
    }
}
