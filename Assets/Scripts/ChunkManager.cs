using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour {

	public GameObject [] floors; //ToDo borrar y que sea todo dinamico
	public Queue<GameObject> chunks;
	public GameObject [] chunkPrefabs; //ToDo

	public float interval = 2f;

	// Use this for initialization
	void Start () {
		StartCoroutine ("SpawnNextAndDeleteLast");
		chunks = new Queue<GameObject> (floors);

	}

	public GameObject [] GetChunks() {
		return chunks.ToArray (); //todo optimize
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SpawnNextAndDeleteLast() {
		while (true) {
			yield return new WaitForSeconds (interval);
			int prefabIdx = Random.Range (0, chunkPrefabs.Length); //ToDo quen levante todos
			GameObject lastChunk = chunks.Dequeue ();
			float lastZ = lastChunk.transform.position.z;
			Destroy (lastChunk);
			GameObject newChunk = Instantiate (chunkPrefabs[prefabIdx]);
			newChunk.transform.position = new Vector3 (newChunk.transform.position.x, newChunk.transform.position.y, lastZ+30f);
			chunks.Enqueue (newChunk);
		}
	}

}
