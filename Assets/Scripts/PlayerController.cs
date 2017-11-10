using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Character Movement Values
    public float jumpForce = 3f;
    public int health = 3;
    public float inmuneTime = 2f;

    PointsAndLevelManager points;
    ChaseController chaseController;
    CameraController camController;
    bool isGrounded = false;
    bool dead = false;
    float initY = -1f;
    float maxZ;
    public static bool inmune = false;

    Animator animator;
    Rigidbody rigid;


    void Start() {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -20f, 0);
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        points = gameController.GetComponent<PointsAndLevelManager>();
        chaseController = gameController.GetComponent<ChaseController>();
        camController = Camera.main.gameObject.GetComponent<CameraController>();
    }

    public void SetMaxZ(float maxZ) {
        this.maxZ = maxZ;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            DoJump();
        }

        if (transform.position.y < initY) {
            Die(DeathCause.Fall);
            animator.SetBool("Jumping", true);
        }
        if (transform.position.z < maxZ) {
            Die(DeathCause.Caught);
        }
    }

    void DoJump() {
        isGrounded = false;
        animator.SetBool("Jumping", true);
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Floor") {
			if (!isGrounded && animator != null) {
				animator.SetBool("Jumping", false);
			}
            if (initY == -1f) {
                initY = transform.position.y - 0.5f;
            }
            isGrounded = true;
        }
    }

    public void Damage(DeathCause cause) {
        inmune = true;
        Debug.Log("GOT DAMAGE");
        health--;
        points.ResetObstacles();
        chaseController.AddHit();
        if (!chaseController.MustDie()) {
            camController.DoShake(inmuneTime / 12);
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

    public bool IsDead() {
        return dead;
    }

    IEnumerator Inmune() {
        
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