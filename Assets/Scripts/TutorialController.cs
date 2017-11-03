using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    Chunk[] chunks;
    ChunkManager manager;
    public GameObject tutorialTextContainer;
    Animation tutorialTextAnimation;
    Text tutorialText;

    bool finishedTutorial;
    // Use this for initialization
    void Start () {
        manager = GetComponent<ChunkManager>();
        chunks = manager.GetChunks();
        finishedTutorial = PlayerPrefs.GetString("TutorialDone") == "true";
        if (finishedTutorial) {
            this.enabled = false;
            return;
        }
        tutorialText = tutorialTextContainer.GetComponent<RectTransform>().GetChild(0).gameObject.GetComponent<Text>();
        tutorialTextAnimation = tutorialTextContainer.GetComponent<Animation>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnterTutorialTrigger(string message, KeyCode keyToPress) {
        Debug.Log(message);
        tutorialText.text = message;
        Time.timeScale = 0.5f;
        tutorialTextAnimation.clip = tutorialTextAnimation.GetClip("TutorialFadeIn");
        tutorialTextAnimation.Play();
        StartCoroutine("WaitForKeyPress", keyToPress);
    }

    IEnumerator WaitForKeyPress(KeyCode key) {
        bool keyPress = Input.GetKeyDown(key);
        while (!keyPress) {
            yield return 0;
            keyPress = Input.GetKeyDown(key);
        }
        Time.timeScale = 1f;
        tutorialTextAnimation.clip = tutorialTextAnimation.GetClip("TutorialFadeOut");
        tutorialTextAnimation.Play();
    }

    void FinishTutorial() {
        this.enabled = false;
    }

    public bool Finished() {
        return finishedTutorial;
    }
}
