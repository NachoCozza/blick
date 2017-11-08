using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    public GameObject tutorialTextContainer;
    Animation tutorialTextAnimation;
    Text tutorialText;

    bool finishedTutorial;
    // Use this for initialization
    void Awake() {
        finishedTutorial = PlayerPrefs.GetString("TutorialDone") == "true";
        if (finishedTutorial) {
            this.enabled = false;
            return;
        }
        tutorialText = tutorialTextContainer.GetComponent<RectTransform>().GetChild(0).gameObject.GetComponent<Text>();
        tutorialTextAnimation = tutorialTextContainer.GetComponent<Animation>();
    }

    public void EnterTutorialTrigger(TutorialTrigger trigger) {
        if (!finishedTutorial) {
            tutorialText.text = trigger.messageToDisplay;
            Time.timeScale = 0.15f;
            tutorialTextAnimation.clip = tutorialTextAnimation.GetClip("TutorialFadeIn");
            tutorialTextAnimation.Play();
            StartCoroutine("WaitForKeyPress", trigger);
        }
    }

    IEnumerator WaitForKeyPress(TutorialTrigger trigger) {
        bool keyPress = Input.GetKeyDown(trigger.keyToPress);
        while (!keyPress) {
            yield return 0;
            keyPress = Input.GetKeyDown(trigger.keyToPress);
        }
        Time.timeScale = 1f;
        tutorialTextAnimation.clip = tutorialTextAnimation.GetClip("TutorialFadeOut");
        tutorialTextAnimation.Play();
        if (trigger.finishesTutorial) {
            PlayerPrefs.SetString("TutorialDone", "true");
        }
    }

    void FinishTutorial() {
        this.enabled = false;
    }

    public bool Finished() {
        return finishedTutorial;
    }
}
