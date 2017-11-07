using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PointsAndLevelManager : MonoBehaviour {

    [HideInInspector]
    public int points;
    public Difficulty currentDifficulty;
    public float pointRate = 0.3f;
    public int pointsPerObstacle;

    InputField playerNameInput;
    public GameObject playerNamePanel;
    public Text scoreText;
    public Text multiplierText;

    public int chunksToMedium = 50;
    public int chunksToHard = 100;
    public float hardDifficultyFloorSpeed = 10;
    public int chunksToImpossible = 200;
    public float impossibleDifficultyFloorSpeed = 14;
    public int chunksToWin = 300;

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
        //Time.timeScale = 1f;
        gameOver = false;
        currentMultiplier = 1;
        pointsPerTick = 1;
        chunksPassed = 0;
        obstaclesPassed = 0;
        playerNameInput = playerNamePanel.transform.GetChild(1).GetComponent<InputField>();
        playerNameInput.characterLimit = 5;
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
        if (forChunks >= chunksToMedium) {
            response = Difficulty.Medium;
        }
        if (forChunks >= chunksToHard) {
            response = Difficulty.Hard;
        }
        if (forChunks >= chunksToImpossible) {
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
        pointRate -= 0.1f;
        multiplierText.text = "x" + currentMultiplier;
        if (currentDifficulty == Difficulty.Hard) {
           // floor.SetNewSpeed(hardDifficultyFloorSpeed);
        }
        if (currentDifficulty == Difficulty.Impossible) {
           // floor.SetNewSpeed(impossibleDifficultyFloorSpeed);
        }
        Debug.Log("NEW DIFFICULTY " + currentDifficulty);
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
        StartCoroutine("FinishGameAndAskForName");

    }

    IEnumerator FinishGameAndAskForName() {
        yield return new WaitForSeconds(1.2f);
        AskForName();
    }

    void AskForName() {
        playerNamePanel.SetActive(true);
		playerNameInput.Select();
		playerNameInput.ActivateInputField();
    }

    public void SetScoreAndQuit() {
        if (!string.IsNullOrEmpty(playerNameInput.text)) {
			SetScore();
			SceneManager.LoadScene("MainMenu");
        }
    }

    public void SetScoreAndRestart() {
        if (!string.IsNullOrEmpty(playerNameInput.text)) {
			SetScore();
			SceneManager.LoadScene("Main");         
        }
        
    }

    void SetScore() {
        string playerName = playerNameInput.text;
        bool added = false;
        string score = PlayerPrefs.GetString("scores");
        List<Score> scores = new List<Score>();
        if (score != null && score != "") {
            scores = Score.GetScoreList(score);
        }
        if (scores.Count < maxScoreCount) {
            bool hasToAdd = true;
            int i = 0;
            while(hasToAdd && i < scores.Count) {
                hasToAdd = scores[i].score != points;
                i++;
            }
            if (hasToAdd) {
                scores.Add(new Score(playerName, points));
				added = true;
            }
        }
        else {
            int insertIndex = -1;
            for (int i = 0; i < scores.Count; i ++) {
                if (scores[i].score < points) {
                    insertIndex = i;
                    break;
                }
                if (scores[i].score == points) {
                    insertIndex = -1;
                    break;
                }
            }
            if (insertIndex != -1) {
                scores.Insert(insertIndex, new Score(playerName, points));
                scores = scores.GetRange(0, 5);
                added = true;
            }
        }

        if (added) {
            string newScoreString = Score.GetScoreString(scores);
            PlayerPrefs.SetString("scores", newScoreString);
        }
    }


    public void ResetObstacles() {
        obstaclesPassed = 0;
        currentMultiplier = 1;
        multiplierText.text = "x" + currentMultiplier;
    }

    private class Score {
        public string name;
        public int score;

        override public string ToString() {
            return this.name + ";" + score;
        }

        public Score(string name, int score) {
            this.name = name;
            this.score = score;
        }

        public static List<Score> GetScoreList(string score) {
            string[] scores = score.Split('|');
            List<Score> response = new List<Score>();
            Score aux;
            string[] auxScore;
            foreach (string singleScore in scores) {
                auxScore = singleScore.Split(';');
                aux = new Score(auxScore[0], int.Parse(auxScore[1]));
                response.Add(aux);
            }
            return response;
        }

        public static string GetScoreString(List<Score> scores) {
            scores.Sort((a, b) => -1 * a.score.CompareTo(b.score)); //Descending
			string response = "";
			foreach (Score score in scores)
			{
				response += score.ToString() + "|";
			}
			response = response.Substring(0, response.Length - 1);
			return response;
        }

    }

}
