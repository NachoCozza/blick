using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject highscores;

	// Use this for initialization
	void Start () {
        string scoresText = PlayerPrefs.GetString("scores");
        if (scoresText != null && scoresText != "") {
            scoresText = scoresText.Replace("|", "\n");
        }
        else {
            scoresText = "No highscores. GO PLAY!";
        }
        Text txt = highscores.transform.GetChild(1).gameObject.GetComponent<Text>();
        txt.text = scoresText;
	}
	
    public void StartGame() {
        SceneManager.LoadScene("Main");
    }

    public void HighScores() {
		highscores.SetActive(true);
        mainMenu.SetActive(false);
        //TODo set scores
    }

    public void Quit() {
		Application.Quit();
    }

    public void MainMenu() {
        highscores.SetActive(false);
        mainMenu.SetActive(true);
    }

}
