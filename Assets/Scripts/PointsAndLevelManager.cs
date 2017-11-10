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
    public GameObject playerRestartPanel;
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
    int obstaclesPassed = 0;

    int lastObstacleInstanceId;

    List<Score> highscores;

    public Text aux;

    ColorManager colorManager;

    // Use this for initialization
    void Start() {
        //PlayerPrefs.DeleteAll();
        Time.timeScale = 1f;
        gameOver = false;
        currentMultiplier = 1;
        pointsPerTick = 1;
        chunksPassed = 0;
        obstaclesPassed = 0;
        playerNameInput = playerNamePanel.transform.GetChild(2).GetComponent<InputField>();
        playerNameInput.characterLimit = 5;
        currentDifficulty = Difficulty.Easy;
        GetHighscores();
        StartCoroutine("Points");
        if (highscores.Count > 0) {
            aux.text = Score.GetScoreString(highscores).Replace("|", "\n");
        }
        colorManager = GetComponent<ColorManager>();
    }

    private void GetHighscores() {
        string score = PlayerPrefs.GetString("scores");
        if (!string.IsNullOrEmpty(score)) {
            highscores = Score.GetScoreList(score);
        }
        else {
            highscores = new List<Score>();
        }
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
        if (currentMultiplier < 4) {
            currentMultiplier++;
            multiplierText.text = "x" + currentMultiplier;
        }
        colorManager.OneColorUp();
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
        if (GetNewScoreIndex(points) != -1) {
            AskForName();
        }
        else {
            playerRestartPanel.SetActive(true);
        }
    }


    int GetNewScoreIndex(int score) {
        bool listIsFull = highscores.Count >= maxScoreCount;
        int res = -1;
        if (!listIsFull) {
            bool hasToAdd = true;
            int i = 0;
            while (hasToAdd && i < highscores.Count) {
                hasToAdd = highscores[i].score != points;
                i++;
            }
            if (hasToAdd) {
                res = i;
            }
        }
        else {
            for (int i = 0; i < highscores.Count; i++) {
                if (highscores[i].score < points) {
                    res = i;
                    break;
                }
                if (highscores[i].score == points) {
                    res = -1;
                    break;
                }
            }
        }
        return res;
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
            Restart();
        }
    }

    public void Restart() {
        SceneManager.LoadScene("Main");
    }

    void SetScore() {
        string playerName = playerNameInput.text;
        int newIdx = GetNewScoreIndex(points);
        if (newIdx != -1) {
            if (highscores.Count + 1 <= maxScoreCount) {
                highscores.Add(new Score(playerName, points));
            }
            else {
                highscores.Insert(newIdx, new Score(playerName, points));
                highscores = highscores.GetRange(0, maxScoreCount);
            }
            string newScoreString = Score.GetScoreString(highscores);
            PlayerPrefs.SetString("scores", newScoreString);
        }
    }


    public void ResetObstacles() {
        obstaclesPassed = 0;
        currentMultiplier = 1;
        multiplierText.text = "x" + currentMultiplier;
        colorManager.ResetColor();
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
            foreach (Score score in scores) {
                response += score.ToString() + "|";
            }
            response = response.Substring(0, response.Length - 1);
            return response;
        }

    }

}
