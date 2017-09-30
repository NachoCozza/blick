using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour {
    public GameObject player;

    public CameraController camera;

    Chunk[] chunks;
    ChunkManager chunkManager;

    [HideInInspector]
    public View currentView = View.Persp;

    void Start() {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
    }

    private void CheckKeys() {
		if (!PointsAndLevelManager.gameOver) {
			if (Input.GetKeyDown(KeyCode.C) && currentView != View.Persp) {
				ChangePerspective(View.Persp);
				return;
			}
			if (Input.GetKeyDown(KeyCode.X) && currentView != View.Right) {
				ChangePerspective(View.Right);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Z) && currentView != View.Top) {
				ChangePerspective(View.Top);
				return;
			}
		}
    }

    void Update() {
        CheckKeys();
    }

    void ChangePerspective(View newView) {
        camera.SetCurrentView(currentView, newView);
        currentView = newView;
    }


	public void NotifyCameraStart(View oldView, View newView)
	{
        IterateChildrenAndArrange(oldView, newView, true);
	}

	public void NotifyCameraFinish(View oldView, View newView)
	{
        IterateChildrenAndArrange(oldView, newView, false);
	}


    void IterateChildrenAndArrange(View oldView, View newView, bool isStart) {
		for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++)
		{
			Chunk child = chunks[chunkIdx];
            child.AdjustCurrentPosition(oldView, newView, isStart);
			if (chunkIdx == FloorMovement.lastChunkIndex)
			{
				child.MovePlayer(player);
			}
		}
    }


}
