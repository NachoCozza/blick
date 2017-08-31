using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//Rigidbodies
	private Rigidbody character3D;
	private Rigidbody2D character2D;

	//Character Movement Values
	public float speed = 1.0f;
	public float jumpStrength = 1.0f;

	//Z Axis Limits
	public float topLimit = 5f;
	public float bottomLimit = -5f;

	void Start ()
	{
		character3D = GetComponent<Rigidbody>();
	}

	void FixedUpdate () 
	{		
		//Input Detection
		float moveHorizontal = Input.GetAxis ("Horizontal") * Time.deltaTime * speed;
		float moveVertical = Input.GetAxis ("Vertical") * Time.deltaTime * speed;
		float jump = Input.GetAxis ("Jump") * Time.deltaTime * jumpStrength;

		transform.rotation = new Quaternion(0, 0, 0, 0); //Rotation Lock

		transform.Translate(moveHorizontal, jump, moveVertical); //Movement

		//Limit Detection
		if (HitTopLimit()) 
		{
			transform.position = new Vector3(transform.position.x,transform.position.y,topLimit);
		}
		else if (HitBottomLimit())
		{
			transform.position = new Vector3(transform.position.x,transform.position.y,bottomLimit);
		}
	}

	bool HitTopLimit ()
	{
		return (transform.position.z > topLimit);
	}

	bool HitBottomLimit ()
	{
		return (transform.position.z < bottomLimit);
	}

	public void SetRigidbody (Rigidbody rb)
	{
		character3D = rb;
	}

	public void SetRigidbody2D (Rigidbody2D rb2d)
	{
		character2D = rb2d;
	}

	public Rigidbody GetRigidbody ()
	{
		return character3D;
	}

	public Rigidbody2D GetRigidbody2D ()
	{
		return character2D;		
	}
}