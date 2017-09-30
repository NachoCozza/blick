using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {

    public float waitTime = 1f;
    PlayerController player;
    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    IEnumerator Timer() {
        yield return new WaitForSeconds(waitTime);
        GetComponent<FloorMovement>().enabled = true;
        GetComponent<PerspectiveController>().enabled = true;
        player.enabled = true;
    }
}
