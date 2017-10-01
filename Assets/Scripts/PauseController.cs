using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {


    public GameObject pauseMenu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            PauseGame();
        }
	}

    public void PauseGame() {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void UnPauseGame() {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void QuitGame() {
        Debug.Log("quit");
        SceneManager.LoadScene("MainMenu");
    }



}
