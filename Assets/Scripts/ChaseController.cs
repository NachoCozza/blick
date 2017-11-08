using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseController : MonoBehaviour {

    int hits;
    public int hitsToDie;
    public float timeToResetHits;
    Coroutine resetTimer;
    public GameObject enemy;

    public float distanceToFirstHit = 15f;
    public float distanceToSecondHit = 12.4f;

    Vector3 currentMoveTo;
    Vector3 originalPosition;
    Vector3 finalPosition;

    Transform player;

    bool hasDied = false;

    // Use this for initialization
    void Start() {
        hits = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = enemy.transform.position;
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
        if (!hasDied) {
            yield return new WaitForSeconds(timeToResetHits);
            currentMoveTo = originalPosition;
            StartCoroutine("MoveTo");
            hits = 0;

        }
        yield return 0;

    }



    void MoveEnemyCloser() {
        switch (hits) {
            case 1:
            default:
                currentMoveTo = GetPosition(distanceToFirstHit);
                break;
            case 2:
                currentMoveTo = GetPosition(distanceToSecondHit);
                break;
            case 3:
                hasDied = true;
                currentMoveTo = GetPosition();
                break;
        }
        StartCoroutine("MoveTo");
    }

    private Vector3 GetPosition() {
        return player.position;
    }

    private Vector3 GetPosition(float distance) {
        return new Vector3(player.position.x, player.position.y, player.position.z - distance);
    }

    public bool MustDie() {
        return hits >= hitsToDie;
    }

    void ResetHits() {
        hits = 0;
    }


    IEnumerator MoveTo() {
        float startTime = Time.time;
        Vector3 startPos = enemy.transform.position;
        while (Time.time - startTime <= 1) {
            enemy.transform.position = Vector3.Lerp(startPos, currentMoveTo, Time.time - startTime);
            yield return 1;
        }
    }

}
