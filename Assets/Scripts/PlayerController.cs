using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Character Movement Values
    public float slideTime = 1f;
    public float jumpForce = 3f;
    public int health = 3;
    public float inmuneTime = 2f;

    PointsAndLevelManager points;
    ChaseController chaseController;
    bool sliding, isGrounded = false;
    bool dead = false;
    float initY = -1f;
    float maxZ;
    bool inmune = false;

    Animator animator;
    Rigidbody rigid;


    void Start() {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -20f, 0);
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        points = gameController.GetComponent<PointsAndLevelManager>();
        chaseController = gameController.GetComponent<ChaseController>();
    }

    public void SetMaxZ(float maxZ) {
        this.maxZ = maxZ;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !sliding) {
            StartCoroutine("DoSlide");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) {
            DoJump();
        }

        if (transform.position.y < initY) {
            Die(DeathCause.Fall);
        }
        if (transform.position.z < maxZ) {
            Die(DeathCause.Caught);
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

    public void Damage(DeathCause cause) {
        health--;
        animator.SetTrigger("DamageCollision");
        points.ResetObstacles();
        chaseController.AddHit();
        if (!chaseController.MustDie()) {
            StartCoroutine("Inmune");
        }
        else {
            Die(cause);
        }
    }

    public void Die(DeathCause cause) {
        if (!dead) {
            dead = true;
            animator.SetTrigger("Die");
            GameObject.FindGameObjectWithTag("GameController").GetComponent<PointsAndLevelManager>().GameOver(cause);
        }
    }

    IEnumerator Inmune() {
        inmune = true;
        WaitForSeconds wait = new WaitForSeconds(inmuneTime / 6);
        SkinnedMeshRenderer mesh = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        Color originalColor = mesh.material.color;
        for (int i = 0; i < 3; i++) {
            mesh.material.color = Color.red;
            yield return wait;
            mesh.material.color = originalColor;
            yield return wait;
        }
        inmune = false;
    }


    public bool IsInmune() {
        return inmune;
    }

}