using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsAndLevelManager : MonoBehaviour {

    [HideInInspector]
    public int points;
    public Difficulty currentDifficulty;
    int pointRate;

    int chunksPassed = 0;

    // Use this for initialization
    void Start() {
        currentDifficulty = Difficulty.Easy;
    }

    // Update is called once per frame
    void Update() {
        points += pointRate;
    }

    public Difficulty GetDifficulty(int forChunks) {
        Difficulty response = Difficulty.Easy;
        if (chunksPassed >= 4) {
            response = Difficulty.Medium;
        }
        if (chunksPassed >= 10) {
            response = Difficulty.Hard;
        }
        if (chunksPassed >= 22) {
            response = Difficulty.Impossible;
        }
        return response;
    }

    public void AddChunk() {
        chunksPassed++;
        if (chunksPassed == 4) {
            currentDifficulty = Difficulty.Medium;
        }
        if (chunksPassed == 10) {
            currentDifficulty = Difficulty.Hard;
        }
        if (chunksPassed == 22) {
            currentDifficulty = Difficulty.Impossible;
        }
        if (chunksPassed == 40) {
            FinishGame();
        }
    }

    public void AddObstacle() {

    }

    void FinishGame() {
        Debug.Log("FINISHED m8");
    }

}
