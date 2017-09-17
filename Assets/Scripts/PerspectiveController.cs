using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveController : MonoBehaviour {
    public GameObject player;

    public GameObject cameraPerspective;
    public GameObject cameraRight;
    public GameObject cameraTop;

    Transform[] chunks;
    Vector3[][] originalPositions;
    ChunkManager chunkManager;

    [HideInInspector]
    public View currentView = View.Persp;


    void Start() {
        chunkManager = GetComponent<ChunkManager>();
        chunks = chunkManager.GetChunks();
        originalPositions = new Vector3[chunks.Length][];
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
        cameraTop.SetActive(false);
        cameraRight.SetActive(false);
        cameraPerspective.SetActive(true);

        IterateChunksAndArrange(0, -Vector3.one);
        currentView = View.Persp;
    }

    void ChangeTo2D(bool right) {
        Vector3 multiplier;
        cameraPerspective.SetActive(false);
        if (right) {
            cameraRight.SetActive(true);
            cameraTop.SetActive(false);
            multiplier = new Vector3(0, 1, 1);
        }
        else {
            cameraTop.SetActive(true);
            cameraRight.SetActive(false);
            multiplier = new Vector3(1, 0, 1);
        }
        //itero todos los chunks, guardo la posicion y alineo todos los hijos
        IterateChunksAndArrange(0, multiplier);
        currentView = right ? View.Right : View.Top;
    }

    public void StoreAllPositions(bool replaceFirstHalfWithSecondHalf) {
        for (int chunkIdx = 0; chunkIdx < chunks.Length; chunkIdx++) {
            int childIdx = 0;
            if (replaceFirstHalfWithSecondHalf && chunkIdx < originalPositions.Length / 2) {
                originalPositions[chunkIdx] = originalPositions[originalPositions.Length / 2 + chunkIdx];
            }
            else {
                originalPositions[chunkIdx] = new Vector3[chunks[chunkIdx].transform.childCount];
                foreach (Transform child in chunks[chunkIdx].transform) {
                    originalPositions[chunkIdx][childIdx] = child.position;
                    childIdx++;
                }

            }
        }
    }

    private void IterateChunksAndArrange(int startIndex, Vector3 multiplier) {
        for (int chunkIdx = startIndex; chunkIdx < chunks.Length; chunkIdx++) {
            int childIdx = 0;
            Vector3 newPos;
            foreach (Transform child in chunks[chunkIdx].transform) {
                if (multiplier == -Vector3.one) //en este caso es perspectiva, asi que tambien alineo al jugador al gameobject mas cercano en Z
                {
                    newPos = originalPositions[chunkIdx][childIdx];
                }
                else {
                    newPos = Vector3.Scale(originalPositions[chunkIdx][childIdx], multiplier);
                }
                /*
                * Si el chunk actual es el mismo que el ultimo que recorri Y ademas una de dos: 
                * 1) tiene un solo hijo por ende se debe parar en ese
                * 2) La Z del hijo actual es anterior a la del jugador y la Z del siguiente es mayor a la del jugador (el jugador esta entre medio de estas dos)
                * 
                * Si se cumple la 1er condicion, y una de esas 2, pongo al jugador en la posicion X e Y del bloque dentro del chunk
                */
                if (chunkIdx == FloorMovement.lastChunkIndex) {
                    bool mustTranslate = chunks[chunkIdx].childCount == 1;
                    if (!mustTranslate) {
                        mustTranslate = newPos.z <= player.transform.position.z && chunks[chunkIdx].GetChild(childIdx + 1).transform.position.x > player.transform.position.z;
                    }
                    if (mustTranslate) {
                        Vector3 aux = newPos;
                        aux.z = player.transform.position.z;
                        player.transform.position = aux;
                    }
                }

                newPos.z = child.position.z;
                child.position = newPos;
                childIdx++;
            }
        }
    }

    public void AdjustNewChunks(int startIndex) {
        Vector3 multiplier = currentView == View.Right ? new Vector3(0, 1, 1) : new Vector3(1, 0, 1);
        if (currentView == View.Persp) {
            multiplier = -Vector3.one;
        }

        //esto se puede unificar en 1 sola funcion pero paja. Si hay performance baja refactorizar
        StoreAllPositions(true);
        IterateChunksAndArrange(startIndex, multiplier);
    }

}
