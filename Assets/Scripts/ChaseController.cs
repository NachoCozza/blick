using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseController : MonoBehaviour {

    int hits;
    public int hitsToDie;
    public float timeToResetHits;
    Coroutine resetTimer;
    public GameObject enemy;
    Vector3 firstPosition;
    Vector3 secondPosition;
    Vector3 currentMoveTo;
    Vector3 originalPosition;
    Vector3 finalPosition;


    bool hasToMove = false;

    // Use this for initialization
    void Start() {
        hits = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        originalPosition = enemy.transform.position;
        firstPosition = player.transform.position;
        secondPosition = player.transform.position;
        finalPosition = player.transform.position;

        firstPosition.z -= 15f;
        secondPosition.z -= 12.4f;

    }

    public void AddHit() {
        hits++;
        MoveEnemyCloser();
        if (resetTimer != null) {
            StopCoroutine(resetTimer);
        }
        resetTimer = StartCoroutine("ResetTimer");
    }

    IEnumerator ResetTimer() {
        yield return new WaitForSeconds(timeToResetHits);
        currentMoveTo = originalPosition;
        StartCoroutine("MoveTo");
        hits = 0;

    }

    void MoveEnemyCloser() {
        hasToMove = true;
        switch(hits) {
            case 1: default:
                currentMoveTo = firstPosition;
                break;
            case 2:
                currentMoveTo = secondPosition;
                break;
            case 3:
                currentMoveTo = finalPosition;
                break;
        }
        StartCoroutine("MoveTo");
    }

    public bool MustDie() {
        return hits >= hitsToDie;
    }

    void ResetHits() {
        hits = 0;
    }


    IEnumerator MoveTo()
    {
        float startTime = Time.time;
        Vector3 startPos = enemy.transform.position;
        while (Time.time - startTime <= 1) {
            enemy.transform.position = Vector3.Lerp(startPos, currentMoveTo, Time.time - startTime);
            yield return 1; 
        }
    }
		
}
