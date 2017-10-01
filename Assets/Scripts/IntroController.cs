using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {

    public float waitTime = 1f;
    PlayerController player;
    GameObject enemy;
    public GameObject backgroundParent;
    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    IEnumerator Timer() {
        yield return new WaitForSeconds(waitTime);
        GetComponent<FloorMovement>().enabled = true;
        GetComponent<PerspectiveController>().enabled = true;
        enemy.layer = LayerMask.NameToLayer("Enemy");
        Destroy(backgroundParent);
        player.enabled = true;
    }
}
