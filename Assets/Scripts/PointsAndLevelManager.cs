using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsAndLevelManager : MonoBehaviour {

    [HideInInspector]
    public int points;
    public Difficulty currentDifficulty;
    public float pointRate = 0.3f;
    public int pointsPerObstacle;

    public Text scoreText;
    public Text multiplierText;

    public int chunksToMedium = 10;
    public int chunksToHard = 25;
    public int chunksToImpossible = 40;
    public int chunksToWin = 70;

    public int maxScoreCount = 5;

    public static bool gameOver = false;

    int currentMultiplier = 1;
    int pointsPerTick = 1;
    int chunksPassed = 0;
    FloorMovement floor;
    int obstaclesPassed = 0;

    int lastObstacleInstanceId;

    // Use this for initialization
    void Start() {
        currentDifficulty = Difficulty.Easy;
        floor = GetComponent<FloorMovement>();
        StartCoroutine("Points");
    }

    IEnumerator Points() {
        WaitForSeconds wait = new WaitForSeconds(pointRate);
        yield return new WaitForSeconds(GetComponent<IntroController>().waitTime);
        while (true) {
            points += pointsPerTick * currentMultiplier;
            scoreText.text = points.ToString();
            yield return wait;
        }
    }

    public Difficulty GetDifficulty(int forChunks) {
        Difficulty response = Difficulty.Easy;
        if (chunksPassed >= chunksToMedium) {
            response = Difficulty.Medium;
        }
        if (chunksPassed >= chunksToHard) {
            response = Difficulty.Hard;
        }
        if (chunksPassed >= chunksToImpossible) {
            response = Difficulty.Impossible;
        }
        return response;
    }

    public void AddChunk() {
        chunksPassed++;
        if (chunksPassed == chunksToMedium) {
            currentDifficulty = Difficulty.Medium;
            UpDifficulty();

        }
        if (chunksPassed == chunksToHard) {
            currentDifficulty = Difficulty.Hard;
            UpDifficulty();
        }
        if (chunksPassed == chunksToImpossible) {
            currentDifficulty = Difficulty.Impossible;
            UpDifficulty();
        }
        if (chunksPassed == chunksToWin) {
            UpDifficulty();
        }
    }


    void UpDifficulty() {
        floor.Faster();
        //ToDo tell the player x2 score
        pointRate -= 0.1f;
        multiplierText.text = "x" + currentMultiplier;
    }

    public void AddObstacle(GameObject obstacle) {
        if (lastObstacleInstanceId == 0 || lastObstacleInstanceId != obstacle.GetInstanceID()) {
            obstaclesPassed++;
            if (obstaclesPassed >= 1) {
                obstaclesPassed = 0;
                MoreMultiplier();
            }
            points += pointsPerObstacle * currentMultiplier;
        }
        lastObstacleInstanceId = obstacle.GetInstanceID();
    }

    private void MoreMultiplier() {
        currentMultiplier++;
        multiplierText.text = "x" + currentMultiplier;
        //ToDo animation
    }

    public void GameOver(DeathCause cause) {
        gameOver = true;
        GetComponent<ChunkManager>().StopCoroutine("SpawnNextAndDeleteLast");
        GetComponent<ChunkManager>().StopCoroutine("UpdateLastChunkIndex");
        StopCoroutine("Points");
        Time.timeScale = 0.1f;
        SetScore();
        //ToDo save high score and redirect to score scene
        Debug.Log("FINISHED m8, died of " + cause);
    }

    void SetScore() {
        bool added = false;
        string score = PlayerPrefs.GetString("scores");
        List<int> scores = new List<int>(ToIntArray(score.Split('|')));

        if (scores.Count < maxScoreCount) {
            scores.Add(points);
            added = true;
        }
        else {
            
        }

        if (added) {
            string newScoreString = JoinScores(scores);
            PlayerPrefs.SetString("scores", newScoreString);
        }
    }

    string JoinScores(List<int> scores) {
        scores.Sort();
        string response = "";
        foreach(int score in scores) {
            response += score.ToString() + "|";
        }
        response = response.Substring(0, response.Length - 1);
        return response;
    }

    int [] ToIntArray(string [] arr) {
        int[] res = new int[arr.Length];
        for (int i = 0; i < arr.Length; i++) {
            res[i] = int.Parse(arr[i]);
        }
        return res;
    }

    public void ResetObstacles() {
        obstaclesPassed = 0;
        currentMultiplier = 1;
        multiplierText.text = "x" + currentMultiplier;
    }

}
