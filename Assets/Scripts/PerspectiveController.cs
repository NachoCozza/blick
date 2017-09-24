using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour {
    public GameObject player;

    public CameraController camera;
    public GameObject cameraPerspective;
    public GameObject cameraRight;
    public GameObject cameraTop;

    Chunk[] chunks;
    ChunkManager chunkManager;

    [HideInInspector]
    public View currentView = View.Persp;
    private bool mustMoveChunks = false;

    void Start() {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
    }

    private void CheckKeys() {
        if (Input.GetKeyDown(KeyCode.Z) && currentView != View.Persp) {
            ChangePerspective(View.Persp);
            return;
        }
        if (Input.GetKeyDown(KeyCode.X) && currentView != View.Right) {
            ChangePerspective(View.Right);
            return;
        }
		if (Input.GetKeyDown(KeyCode.C) && currentView != View.Top) {
            ChangePerspective(View.Top);
            return;
        }
    }


    void Update() {
        CheckKeys();
    }

    void ChangePerspective(View newView) {
        camera.SetCurrentView(currentView, newView);
        currentView = newView;
        IterateChunksAndArrange(false);
    }

    private void IterateChunksAndArrange(bool instantExecution) {
        if (instantExecution) {
            UnlockChunkArrangement();
        }
        IEnumerator doChunkArrangement = DoChunkArrengement();
        StartCoroutine(doChunkArrangement);
    }

    //This method will wait for ExecuteChunkArrangement() to be called before actually arranging the chunks
    IEnumerator DoChunkArrengement() {
        while (!mustMoveChunks) {
            yield return 0;
        }
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
            Chunk child = chunks[chunkIdx];
            child.AdjustCurrentPosition();
            /*
            * Si el chunk actual es el mismo que el ultimo que recorri Y ademas una de dos: 
            * 1) tiene un solo hijo por ende se debe parar en ese
            * 2) La Z del hijo actual es anterior a la del jugador y la Z del siguiente es mayor a la del jugador (el jugador esta entre medio de estas dos)
            * 
            * Si se cumple la 1er condicion, y una de esas 2, pongo al jugador en la posicion X e Y del bloque dentro del chunk
            */
            if (chunkIdx == FloorMovement.lastChunkIndex) {
                Vector3 aux = child.getTransform().position;
                aux.z = player.transform.position.z;
                player.transform.position = aux;
            }
        }
        yield return 0;
        mustMoveChunks = false;
    }

    //This method sets this flag to true, so that IterateChunksAndArrange stops waiting and actually does arrange the chunks
    public void UnlockChunkArrangement() {
        mustMoveChunks = true;
    }

}
