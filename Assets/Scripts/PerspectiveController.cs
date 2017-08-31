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

    GameObject[] floors;

    BoxCollider[] colliders3D;
    BoxCollider2D[] colliders2D;


    BoxCollider2D playerCollider2D;
    BoxCollider playerCollider3D;
	Rigidbody playerRigidbody3D;
	Rigidbody2D playerRigidbody2D;
	bool is3D = false;

	ChunkManager chunkManager;

	public bool Is3D() { return is3D; }

    void Start()
    {
		chunkManager = GetComponent<ChunkManager> ();
        floors = chunkManager.GetChunks();
        colliders3D = new BoxCollider[floors.Length];
        colliders2D = new BoxCollider2D[floors.Length];

        InstantiateAll3D();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeToPerspective();
			return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeToRight();
			return;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeToTop();
			return;
        }
    }

    void ChangeToPerspective()
    {
        StartCoroutine("Apply3D");
    }

    void ChangeToRight()
    {
        StartCoroutine("Apply2D", true);
    }

    void ChangeToTop()
    {
        StartCoroutine("Apply2D", false);
    }

    private void DestroyAll(BoxCollider[] arr)
    {
        int initLength = arr.Length;
        for (int i = 0; i < initLength; i++)
        {
            Destroy(arr[i]);
        }
        arr = new BoxCollider[initLength];
    }

    private void DestroyAll(BoxCollider2D[] arr)
    {
        int initLength = arr.Length;
        for (int i = 0; i < initLength; i++)
        {
            Destroy(arr[i]);
        }
        arr = new BoxCollider2D[initLength];
    }

	private void DestroyAll3D() {
		DestroyAll(colliders3D);
		Destroy(playerCollider3D);
		Destroy (playerRigidbody3D);
		cameraPerspective.SetActive(false);
	}

	private void DestroyAll2D() {
		DestroyAll(colliders2D);
		Destroy(playerCollider2D);
		Destroy (playerRigidbody2D);
		cameraRight.SetActive(false);
		cameraTop.SetActive(false);
	}

    private void InstantiateAll3D()
    {
		if (!is3D) {
			for (int i = 0; i < floors.Length; i++)
			{
				colliders3D[i] = floors[i].AddComponent<BoxCollider>();
			}
			playerCollider3D = player.AddComponent<BoxCollider>();
			playerRigidbody3D = player.AddComponent<Rigidbody> ();
			is3D = true;
		}
    }

    private void InstantiateAll2D()
    {
		if (is3D) {
			for (int i = 0; i < floors.Length; i++)
			{
				colliders2D[i] = floors[i].AddComponent<BoxCollider2D>();
			}
			
			playerCollider2D = player.AddComponent<BoxCollider2D>();
			playerRigidbody2D = player.AddComponent<Rigidbody2D> ();
			is3D = false;
		}
    }

    IEnumerator Apply2D(bool right)
    {
		floors = chunkManager.GetChunks ();
		DestroyAll3D ();
        if (right)
        {
            cameraTop.SetActive(false);
        }
        else
        {
            cameraRight.SetActive(false);
        }
        yield return new WaitForEndOfFrame();
        InstantiateAll2D();
        if (right)
        {
            cameraRight.SetActive(true);
        }
        else
        {
            cameraTop.SetActive(true);
        }
        //player.transform.position = new Vector3(player3D.transform.position.x, player2D.transform.position.y, player2D.transform.position.z);
    }

    IEnumerator Apply3D()
    {
		floors = chunkManager.GetChunks ();
		DestroyAll2D ();
		yield return new WaitForEndOfFrame();
        InstantiateAll3D();
        cameraPerspective.SetActive(true);
        //player.transform.position = new Vector3(player3D.transform.position.x, player2D.transform.position.y, player2D.transform.position.z);
    }
		
}
