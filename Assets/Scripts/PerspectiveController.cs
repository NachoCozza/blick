using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour {
    public GameObject player;

    public CameraController camera;
    public GameObject cameraPerspective;
    public GameObject cameraRight;
    public GameObject cameraTop;

    Transform[] chunks;
    Vector3[] originalPositions;
    ChunkManager chunkManager;

    [HideInInspector]
    public View currentView = View.Persp;
    private bool mustMoveChunks = false;

    void Start() {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
        originalPositions = new Vector3[chunks.Length];
        StoreAllPositions(false);
    }

    private void CheckKeys() {
        if (Input.GetKeyDown(KeyCode.P) && currentView != View.Persp) {
            ChangeTo3D();
            return;
        }
        if (Input.GetKeyDown(KeyCode.O) && currentView != View.Right) {
            ChangeTo2D(true);
            return;
        }
        if (Input.GetKeyDown(KeyCode.I) && currentView != View.Top) {
            ChangeTo2D(false);
            return;
        }
    }


    void Update() {
        CheckKeys();
    }

    void ChangeTo3D() {
        View newView = View.Persp;
        camera.SetCurrentView(currentView, newView);
        currentView = newView;
        IterateChunksAndArrange(0, -Vector3.one, false);
    }

    void ChangeTo2D(bool right) {
        View newView = right ? View.Right : View.Top;
        camera.SetCurrentView(currentView, newView);
        currentView = newView;
        Vector3 multiplier;
        if (right) {
            multiplier = new Vector3(0, 1, 1);
        }
        else {
            multiplier = new Vector3(1, 0, 1);
        }
        //itero todos los chunks, guardo la posicion y alineo todos los hijos
        IterateChunksAndArrange(0, multiplier, false);
    }

    public void StoreAllPositions(bool replaceFirstHalfWithSecondHalf) {
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
            if (replaceFirstHalfWithSecondHalf && chunkIdx < originalPositions.Length / 2) {
                originalPositions[chunkIdx] = originalPositions[originalPositions.Length / 2 + chunkIdx];
            }
            else {
                originalPositions[chunkIdx] = chunks[chunkIdx].transform.GetChild(0).transform.position;
                //foreach (Transform child in chunks[chunkIdx].transform) {
                //    if (child.tag == "Floor") { //ToDo check if "child" isn't actually the parent (child != chunks[chunkIdx].transform[0] doesnt work)
                //        originalPositions[chunkIdx].add(child.position);
                //    }
                //}
            }
        }
    }

    //This method will wait for ExecuteChunkArrangement() to be called before actually arranging the chunks
    private void IterateChunksAndArrange(int startIndex, Vector3 multiplier, bool instantExecution) {
        if (instantExecution) {
            UnlockChunkArrangement();
        }
        IEnumerator doChunkArrangement = DoChunkArrengement(startIndex, multiplier);
        StartCoroutine(doChunkArrangement);
        
    }

    IEnumerator DoChunkArrengement(int startIndex, Vector3 multiplier) {
        while (!mustMoveChunks) {
            yield return 0;
        }
        for (int chunkIdx = startIndex; chunkIdx < chunks.Length; chunkIdx++) {
            Vector3 newPos;
            Transform child = chunks[chunkIdx].GetChild(0);
            if (multiplier == -Vector3.one) //en este caso es perspectiva, asi que tambien alineo al jugador al gameobject mas cercano en Z
            {
                newPos = originalPositions[chunkIdx];
            }
            else {
                newPos = Vector3.Scale(originalPositions[chunkIdx], multiplier);
            }
            /*
            * Si el chunk actual es el mismo que el ultimo que recorri Y ademas una de dos: 
            * 1) tiene un solo hijo por ende se debe parar en ese
            * 2) La Z del hijo actual es anterior a la del jugador y la Z del siguiente es mayor a la del jugador (el jugador esta entre medio de estas dos)
            * 
            * Si se cumple la 1er condicion, y una de esas 2, pongo al jugador en la posicion X e Y del bloque dentro del chunk
            */
            if (chunkIdx == FloorMovement.lastChunkIndex) {
                Vector3 aux = newPos;
                aux.z = player.transform.position.z;
                player.transform.position = aux;
            }
            newPos.z = child.position.z;
            child.position = newPos;
        }
        Debug.Log("finished chunk arrangement");
        yield return 0;
        mustMoveChunks = false;
    }


    public void AdjustNewChunks(int startIndex) {
        Vector3 multiplier = currentView == View.Right ? new Vector3(0, 1, 1) : new Vector3(1, 0, 1);
        if (currentView == View.Persp) {
            multiplier = -Vector3.one;
        }

        //These 2 calls should be unified in a single function to optimize iterations, but it's easier to code like this. ToDo: Fix if performance issues
        StoreAllPositions(true);
        IterateChunksAndArrange(startIndex, multiplier, true);
    }

    //This method sets this flag to true, so that IterateChunksAndArrange stops waiting and actually does arrange the chunks
    public void UnlockChunkArrangement() {
        mustMoveChunks = true;
    }

}
