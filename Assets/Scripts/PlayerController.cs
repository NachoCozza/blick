using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Character Movement Values
    public float slideTime = 1f;
    public float jumpForce = 3f;
    public float boundLanesPadding = 3f;

    float[] lanes = new float[3];
    int moveToLane = 1;
    int currentLane = 1;


    bool sliding, isGrounded = false;
    float initY = -1f;

    Animator animator;
    Rigidbody rigid;

    void Start() {
        rigid = GetComponent<Rigidbody>();
        GameObject levelManager = GameObject.FindGameObjectWithTag("GameController");
        ChunkManager chunkManager = levelManager.GetComponent<ChunkManager>();
        Bounds chunkBounds = chunkManager.GetChunks()[0].GetComponent<MeshRenderer>().bounds;
        lanes[0] = chunkBounds.center.x - (chunkBounds.extents.x - boundLanesPadding);
        lanes[1] = chunkBounds.center.x;
        lanes[2] = chunkBounds.center.x + (chunkBounds.extents.x - boundLanesPadding);

        animator = GetComponent<Animator>();
    }

    void Update() {
        if (true) //ToDo delete
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) && !sliding) {
                StartCoroutine("DoSlide");
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) {
                DoJump();
            }
        }
        else {
            if (Input.GetKeyDown(KeyCode.DownArrow) && currentLane + 1 < lanes.Length) {
                moveToLane = currentLane + 1;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentLane - 1 >= 0) {
                moveToLane = currentLane - 1;
            }

            if (currentLane != moveToLane) {
                Vector3 moveToPosition = transform.position;
                moveToPosition.x = lanes[moveToLane];
                transform.position = moveToPosition;
                currentLane = moveToLane;
            }
        }
        if (transform.position.y < initY) {
            Die(DeathCause.Fall);
        }
    }

    IEnumerator DoSlide() {
        sliding = true;
        animator.SetBool("IsSliding", sliding);
        yield return new WaitForSeconds(slideTime);
        sliding = false;
        animator.SetBool("IsSliding", sliding);
    }

    void DoJump() {
        isGrounded = false;
        animator.SetBool("Jumping", true);
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Floor") {
            if (initY == -1f) {
                initY = transform.position.y - 0.5f;
            }
            if (!isGrounded) {
                animator.SetBool("Jumping", false);
            }
            isGrounded = true;
        }
    }

    public void Die(DeathCause cause) {
        animator.SetTrigger("Die");
        //Debug.Log("Gotta die of " + cause.ToString());
        Time.timeScale = 0;
        //ToDo save high score, go to highscore scene.
    }

}