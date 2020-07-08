using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	public GameObject target;
	private GameObject camera;
	public float mouseSpeed;
	public Vector3 cameraDir;
	//Vector3 _offsetPos;
	Vector3 _offsetAng;
	Vector3 _3rdPerspOffset;
	Vector2 mouseAxis;
	bool _3rdPerson = false;
	
	// Use this for initialization
	void Start () {
		camera = gameObject.transform.GetChild (0).gameObject;
		_3rdPerspOffset = camera.transform.localPosition;
		_offsetAng = gameObject.transform.localRotation.eulerAngles;
		//_offsetPos = gameObject.transform.position;
		mouseAxis = new Vector2(gameObject.transform.localRotation.eulerAngles.x, gameObject.transform.localRotation.eulerAngles.y);
		cameraDir = GetCamDir ();
	}

	Vector3 GetCamDir(){
		Vector3 a = new Vector3 (target.transform.position.x, 0, target.transform.position.z);
		Vector3 b = new Vector3 (camera.transform.position.x, 0, camera.transform.position.z);
		return (a - b).normalized;
	}
	// Update is called once per frame
	void Update () {
		//Vector2 mouseNevv = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		//Vector2 mouseDelta = mouseOld - mouseNevv;
		mouseAxis+=new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		mouseAxis.x += Input.GetAxis ("Mouse X") * mouseSpeed;
		mouseAxis.y += Input.GetAxis ("Mouse Y") * mouseSpeed;
		Quaternion lrot = gameObject.transform.localRotation;
		/*float x = mouseDelta.x * mouseSpeed;
		mouseDelta.x = mouseDelta.y * mouseSpeed;
		mouseDelta.y = -x;
		float mdLen = mouseDelta.magnitude;

		*/
		/*float mdLen = mouseDelta.magnitude;
		if (mdLen <= 0.1)
			return;*/
		gameObject.transform.position = target.transform.position;
		cameraDir = GetCamDir ();
		lrot.eulerAngles = new Vector3 (-mouseAxis.y, mouseAxis.x, 0);//mouseDelta * 0.1f;
		gameObject.transform.localRotation = lrot;
		//mouseOld = mouseNevv;
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Change2FirstPerson();
		}
	}
	public void Change2FirstPerson() {
		if (_3rdPerson)
		{
			camera.transform.localPosition = new Vector3(0, 1, 0);
			_3rdPerson = false;
		}
		else {
			camera.transform.localPosition = _3rdPerspOffset;
			_3rdPerson = true;
		}
		

	}
}
