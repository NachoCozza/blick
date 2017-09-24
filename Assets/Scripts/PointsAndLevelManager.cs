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
        if (chunksPassed == 4) {
            currentDifficulty = Difficulty.Medium;
			UpDifficulty ();
            
        }
        if (chunksPassed == 10) {
            currentDifficulty = Difficulty.Hard;
			UpDifficulty ();
        }
        if (chunksPassed == 22) {
            currentDifficulty = Difficulty.Impossible;
			UpDifficulty ();
        }
        if (chunksPassed == 40) {
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

    void FinishGame() {
        Debug.Log("FINISHED m8");
    }

}
