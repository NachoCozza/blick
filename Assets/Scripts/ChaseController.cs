using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseController : MonoBehaviour {

    int hits;
    public int hitsToDie;
    public float timeToResetHits;
    Coroutine resetTimer;

    // Use this for initialization
    void Start() {
        hits = 0;
    }

    public void AddHit() {
        Debug.Log(" got damage");
        hits++;
        MoveEnemyCloser();
        if (resetTimer != null) {
            StopCoroutine(resetTimer);
        }
        resetTimer = StartCoroutine("ResetTimer");
        Debug.Log("hits taken " + hits) ;
    }

    IEnumerator ResetTimer() {
        yield return new WaitForSeconds(timeToResetHits);
        hits = 0;
    }

    void MoveEnemyCloser() {
        //ToDo
    }

    public bool MustDie() {
        return hits >= hitsToDie;
    }

    void ResetHits() {
        hits = 0;
    }
}
