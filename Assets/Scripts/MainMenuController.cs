using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject highscores;
    public GameObject roadMap;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1f;
        string scoresText = PlayerPrefs.GetString("scores");
        if (scoresText != null && scoresText != "") {
            scoresText = scoresText.Replace("|", "\n").Replace(";", " - ");
        }
        else {
            scoresText = "No highscores. GO PLAY!";
        }
        Text txt = highscores.transform.GetChild(2).gameObject.GetComponent<Text>();
        txt.text = scoresText;
	}
	
    public void StartGame() {
        SceneManager.LoadScene("Main");
    }

    public void HighScores() {
		highscores.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void MainMenu() {
        highscores.SetActive(false);
        roadMap.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void FutureFeatures() {
        roadMap.SetActive(true);
        mainMenu.SetActive(false);
    }

}
