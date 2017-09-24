using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Character Movement Values
    public float slideTime = 1f;
    public float jumpForce = 3f;
    public int health = 3;

    bool sliding, isGrounded = false;
	bool dead = false;
    float initY = -1f;

    Animator animator;
    Rigidbody rigid;


    void Start() {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity = new Vector3(0, -20f, 0);
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
		Debug.Log (" got damage");
        if (health == 0) {
            Die(cause);
        }
    }

    public void Die(DeathCause cause) {
		if (!dead) {
			dead = true;
			animator.SetTrigger("Die");
			GameObject.FindGameObjectWithTag("GameController").GetComponent<PointsAndLevelManager> ().GameOver(cause);
		}
    }

}