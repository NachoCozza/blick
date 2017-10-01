using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip[] clips;
    AudioSource audioSource;
    int currentClipIdx = 0;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
        if (!audioSource.isPlaying) {
            audioSource.clip = clips[currentClipIdx];
            audioSource.Play();
            currentClipIdx++;
            if (currentClipIdx >= clips.Length) {
                currentClipIdx = 0;
            }
        }
	}
}
