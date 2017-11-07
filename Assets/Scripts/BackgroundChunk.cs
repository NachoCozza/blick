using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChunk : MonoBehaviour {

    public bool isFrontLane;
    static PerspectiveController PERSPECTIVE;
    float currentMovementSpeed;

    void Start() {
        if (PERSPECTIVE == null) {
            PERSPECTIVE = GameObject.FindGameObjectWithTag("GameController").GetComponent<PerspectiveController>();
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        if (!PointsAndLevelManager.gameOver) {
            currentMovementSpeed = isFrontLane ? FloorMovement.FIRST_LANE_SPEED : FloorMovement.FIRST_LANE_SPEED / 2;
            if (PERSPECTIVE.currentView != View.Persp) {
                currentMovementSpeed /= 10;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - currentMovementSpeed * Time.deltaTime);
        }
    }
}
