using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour
{
    //ToDo optimize. unificar los 3 perspective

    bool is3D = true;

    public GameObject player;

    public GameObject cameraPerspective;
    public GameObject cameraRight;
    public GameObject cameraTop;

    GameObject[] floors;

    BoxCollider[] colliders3D;
    BoxCollider2D[] colliders2D;


    BoxCollider2D playerCollider2D;
    //ToDo lo mismo que los colliders3d y 2d pero con rigidbody
    BoxCollider playerCollider3D;

    void Start()
    {
        floors = GetComponent<ChunkManager>().GetChunks();
        colliders3D = new BoxCollider[floors.Length];
        colliders2D = new BoxCollider2D[floors.Length];

        InstantiateAll3D();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeToPerspective();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeToRight();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ChangeToTop();
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

    private void InstantiateAll3D()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            colliders3D[i] = floors[i].GetComponent<BoxCollider>();
        }
        playerCollider3D = player.GetComponent<BoxCollider>();
    }

    private void InstantiateAll2D()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            colliders2D[i] = floors[i].AddComponent<BoxCollider2D>();
        }

        playerCollider2D = player.AddComponent<BoxCollider2D>();
    }

    IEnumerator Apply2D(bool right)
    {
        DestroyAll(colliders3D);
        Destroy(playerCollider3D);
        cameraPerspective.SetActive(false);
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
        is3D = false;
    }

    IEnumerator Apply3D()
    {
        DestroyAll(colliders2D);
        Destroy(playerCollider2D);
        cameraRight.SetActive(false);
        cameraTop.SetActive(false);
        yield return new WaitForEndOfFrame();
        InstantiateAll3D();
        cameraPerspective.SetActive(true);
        //player.transform.position = new Vector3(player3D.transform.position.x, player2D.transform.position.y, player2D.transform.position.z);
        is3D = true;
    }

    /* IEnumerator ChangeTo3D()
     {
         Destroy(floor2D);
         Destroy(floor2D2);
         yield return new WaitForEndOfFrame();

         camera3D.SetActive(true);
         player3D.SetActive(true);

         camera2D.SetActive(false);
         player2D.SetActive(false);
         Destroy(floor2D);
         floor3D = floor.AddComponent<BoxCollider>();
         floor3D2 = floor2.AddComponent<BoxCollider>();

         player3D.transform.position = new Vector3(player2D.transform.position.x, player3D.transform.position.y, player3D.transform.position.z);
         is3D = true;
     }*/
}
