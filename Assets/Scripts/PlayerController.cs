using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Character Movement Values
    public float slideTime = 1f;
    public float jumpStrength = 1.0f;
    public float boundLanesPadding = 3f;

    float[] lanes = new float[3];
    int moveToLane = 1;
    int currentLane = 1;

    bool sliding, jumping = false;

    PerspectiveController perspective;
    Animator animator;

    void Start()
    {
        GameObject levelManager = GameObject.FindGameObjectWithTag("GameController");
        ChunkManager chunkManager = levelManager.GetComponent<ChunkManager>();
        Bounds chunkBounds = chunkManager.GetChunks()[0].GetComponent<MeshRenderer>().bounds;
        lanes[0] = chunkBounds.center.x - (chunkBounds.extents.x - boundLanesPadding);
        lanes[1] = chunkBounds.center.x;
        lanes[2] = chunkBounds.center.x + (chunkBounds.extents.x - boundLanesPadding);

        animator = GetComponent<Animator>();
        perspective = levelManager.GetComponent<PerspectiveController>();
    }

    void Update()
    {
        if (perspective.currentView == PerspectiveController.View.Right)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) && !sliding)
            {
                StartCoroutine("DoSlide");
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && !jumping)
            {
                DoJump();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) && currentLane + 1 < lanes.Length)
            {
                moveToLane = currentLane + 1;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentLane - 1 >= 0)
            {
                moveToLane = currentLane - 1;
            }

            if (currentLane != moveToLane)
            {
                Vector3 moveToPosition = transform.position;
                moveToPosition.x = lanes[moveToLane];
                transform.position = moveToPosition;
                currentLane = moveToLane;
            }
        }


    }

    IEnumerator DoSlide()
    {
        animator.SetBool("Sliding", true);
        yield return new WaitForSeconds(slideTime);
        animator.SetBool("Sliding", false);
    }

    void DoJump()
    {

    }

}