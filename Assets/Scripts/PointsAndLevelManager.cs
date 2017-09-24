using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsAndLevelManager : MonoBehaviour {

    [HideInInspector]
    public int points;
    public Difficulty currentDifficulty;
	public float pointRate = 0.3f;
	public Text scoreText;

	public static bool gameOver = false;

	int pointsPerTick = 1;
    int chunksPassed = 0;
    FloorMovement floor;

    // Use this for initialization
    void Start() {
        currentDifficulty = Difficulty.Easy;
        floor = GetComponent<FloorMovement>();
		StartCoroutine ("Points");
    }

	IEnumerator Points() {
		WaitForSeconds wait = new WaitForSeconds (pointRate);
		yield return new WaitForSeconds (GetComponent<IntroController> ().waitTime);
		while (true) {
			points += pointsPerTick;
			scoreText.text = points.ToString();
			yield return wait;
		}
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
        if (chunksPassed == 10) {
            currentDifficulty = Difficulty.Medium;
			UpDifficulty ();
            
        }
        if (chunksPassed == 25) {
            currentDifficulty = Difficulty.Hard;
			UpDifficulty ();
        }
        if (chunksPassed == 40) {
            currentDifficulty = Difficulty.Impossible;
			UpDifficulty ();
        }
        if (chunksPassed == 65) {
			UpDifficulty ();
        }
    }


	void UpDifficulty() {
		floor.Faster();
		//ToDo tell the player x2 score
		pointRate *= 2;
	}

    public void AddObstacle() {

    }

	public void GameOver(DeathCause cause) {
		gameOver = true;
		GetComponent<ChunkManager> ().StopCoroutine ("SpawnNextAndDeleteLast");
		GetComponent<ChunkManager> ().StopCoroutine ("UpdateLastChunkIndex");
		Time.timeScale = 0.1f;
		//ToDo save high score and redirect to score scene
        Debug.Log("FINISHED m8, died of " + cause);
    }

}
