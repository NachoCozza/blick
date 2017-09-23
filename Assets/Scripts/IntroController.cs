using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {

    public float waitTime = 1f;

    // Use this for initialization
    void Start() {
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    IEnumerator Timer() {
        yield return new WaitForSeconds(waitTime);
        GetComponent<FloorMovement>().enabled = true;
        GetComponent<PerspectiveController>().enabled = true;
    }
}
