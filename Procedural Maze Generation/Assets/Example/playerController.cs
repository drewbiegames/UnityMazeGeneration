using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Unity Tutorial Player Controller, with collison
public class playerController : MonoBehaviour {

	private Rigidbody rBody;
	private Camera gameCam;
	public float cameraDistance = 10.0f;
	// Use this for initialization
	void Start () {
		rBody = GetComponent<Rigidbody> ();
		gameCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		float x = Input.GetAxis ("Horizontal") * Time.deltaTime * 150.0f;
		float z = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;
		transform.Rotate (0, 0, x);
		transform.Translate (0, z, 0);

		Vector3 newCamPos = transform.position;
		newCamPos.z = -cameraDistance;
		gameCam.transform.position = newCamPos;
	}

	void OnCollisionEnter(Collision collision) {
		rBody.velocity = Vector3.zero;
	}
}
