using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {
    public string messageToDisplay;
    public KeyCode keyToPress;
    static TutorialController controller;

    private void Start() {
        if (controller == null) {
            controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<TutorialController>();
        }
    }
    public void OnTriggerEnter(Collider other) {
        if (other.tag  == "Player") {
            controller.EnterTutorialTrigger(messageToDisplay, keyToPress);
        }
    }
}
