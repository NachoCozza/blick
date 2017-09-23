using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public View[] allowedViews;
    PerspectiveController perspective;
    static PlayerController player;
    static PointsAndLevelManager points;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bool canGoThrough = false;
            int i = 0;
            while (i < allowedViews.Length && !canGoThrough)
            {
                canGoThrough = allowedViews[i] == perspective.currentView;
                i++;
            }
            if (!canGoThrough)
            {
                if (player == null) {
                    player = other.GetComponent<PlayerController>();
                }
                player.Damage(DeathCause.Collision);
            }
            else {
                if (points == null) {
                    points = GameObject.FindGameObjectWithTag("GameController").GetComponent<PointsAndLevelManager>();
                }
                points.AddObstacle();
            }
        }
        
    }

    // Use this for initialization
    void Awake () {
        perspective = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();
        if (allowedViews.Length > 3)
        {
            Debug.LogError("More than 3 allowed perspectives in gameObject " + this.name + " ID: " + this.GetInstanceID());
        }
    }
}
