using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {

    public float waitTime = 1f;
    PlayerController player;
    GameObject enemy;
    public GameObject backgroundParent;

    public GameObject firstTutorialText;
    public GameObject secondTutorialText;
    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        string hadTutorial = PlayerPrefs.GetString("hadTutorial");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        StartCoroutine("Timer");
        if (hadTutorial == null || hadTutorial == "") {
            StartCoroutine("Tutorial");
        }

    }

    IEnumerator Tutorial() {
        firstTutorialText.SetActive(true);
        yield return new WaitForSeconds(13);
        firstTutorialText.SetActive(false);
        secondTutorialText.SetActive(true);
        yield return new WaitForSeconds(5);
        Destroy(firstTutorialText);
        Destroy(secondTutorialText);
        PlayerPrefs.SetString("hadTutorial", "done");
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
