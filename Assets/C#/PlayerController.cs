using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
	public AnimationCurve _wallRunCurve;
	Slider _staminaBar;
	bool[] _input;
	bool _running;
	public bool _inAir;
	public bool _wallRunning;
	bool _touchingWall;
	bool _sprinting;
	bool[] _runningCheck;
	Transform _ground;
	Pair<Transform, Vector3> _wallCollision;
	Vector3[] _wallRunPositions;
	Rigidbody _body;
	float _stamina;
	float _currentStamina;
	//Vector3 _wallRunDir;
    // Start is called before the first frame update
    void Start()
    {
		_staminaBar = GameObject.Find("StaminaBar").GetComponent<Slider>();
		_wallRunPositions = new Vector3[5];
		_currentStamina = 1;
		_stamina = _currentStamina;
		_sprinting = false;
		_input = new bool[6];
		_wallCollision = null;
		_touchingWall = false;
		_wallRunning = false;
		_body = GetComponent<Rigidbody>();
		//_collidedWithCount = 0;
		//_runningCheck = new 
		//if nothing and direction
		//run
		//if running and jump
		//direction jump
		//if nothing and jump
		//direction jump
		//if in air or on ground and direction and action button
		//wall  run
		//if wall run and jump
		//wall jump
		//if touching wall and jump adn in air
		//wall jump
	}

	// Update is called once per frame
	void Update()
    {
		RecoverStamina();
		HandleInput();
		HandleActionCalls();
		Check4Air();
		if (_wallRunning) {
			WallRun();
		}
		UpdateStamina();
	}
	void RecoverStamina() {
		if (!_wallRunning && _currentStamina < _stamina) {
			_currentStamina += Time.deltaTime;
		}
	}
	void HandleActionCalls() {
		if (IfRunning()) {
			Running();
		}
		else {
			_running = false;
		}
		if (IfSprinting()) {
			Sprinting();
			_currentStamina -= Time.deltaTime;
		}
		if (IfWallRun()) {
			StartWallRun();
		}
		if (_wallRunning && !_input[5]) {
			_body.useGravity = true;
			_wallRunning = false;
		}
		if (_wallRunning && !_touchingWall) {
			_body.useGravity = true;
			_wallRunning = false;
		}
		if (IfDirectionalJump()) {
			DirectionalJump();
		}
		if (IfJump()) {
			Jump();
		}
		//if wallrunning and at corner of wall
	}
	void HandleInput() {
		_input[0] = (Input.GetKey(KeyCode.W));
		_input[1] = (Input.GetKey(KeyCode.S));
		_input[2] = (Input.GetKey(KeyCode.A));
		_input[3] = (Input.GetKey(KeyCode.D));

		_input[4] = (Input.GetKey(KeyCode.Space));
		_input[5] = (Input.GetKey(KeyCode.LeftShift));
	}
	bool IfRunning() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && (!(_input[4]) && !_input[5]) && !_wallRunning);
	}
	bool IfSprinting() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && (!_input[4]) && _input[5] && !_wallRunning && !_inAir);
	}
	bool IfWallRun() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && !(_input[4]) && _input[5] && _touchingWall && !_wallRunning);
	}
	bool IfDirectionalJump() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && _input[4] && !_input[5] && !_inAir);
	}
	bool IfJump() {
		return ((!_input[0] && !_input[1] && !_input[2] && !_input[3]) && _input[4] && !_input[5] && !_inAir);
	}
	void WallRun() {

		if (!_touchingWall) {
			_wallRunning = false;
			return;
		}
		Debug.Log("Wall Run");
		float speed = 0.15f;
		float curveValue = 0;
		_currentStamina -= Time.deltaTime*0.1f;
		if (_currentStamina <= 0) {
			OutOfStamina();
		}
		Vector3 movDir = GetInputDirection();
		float dot = Vector3.Dot(movDir, _wallCollision._two);
		

		Vector3 side = Vector3.Cross(_wallCollision._two, Vector3.up);
		Vector3 newUp = -Vector3.Cross(_wallCollision._two, side);
		Vector3 rayDir;
		if (Mathf.Abs(dot) > 0.8f) {
			movDir = newUp;
			rayDir = Vector3.Cross(side, Vector3.up);
		}
		else {
			Vector3 cross = Vector3.Cross(_wallCollision._two, Vector3.up);
			dot = Vector3.Dot(movDir, cross);
			rayDir = Vector3.Cross(side, Vector3.up);
			if (dot > 0) {
				movDir = cross;
				
			}
			else {
				movDir = -cross;
			}
			curveValue = _wallRunCurve.Evaluate((1 - (_currentStamina / _stamina)));
			
			//movDir = (_wallCollision._two + movDir).normalized;

		}
		//wallrunDir upwards
		RaycastHit hit;
		if (Physics.Raycast(transform.position, rayDir, out hit, 2)) {
			//hit.normal;
			//hit.point;
			_wallCollision._two = hit.normal;
		}
		//movDir = (Vector3.up + movDir).normalized;
		_body.velocity = Vector3.zero;
		transform.position += (movDir + (newUp * curveValue)) * speed;
		transform.localRotation = Quaternion.LookRotation(movDir);
	}

	void OutOfStamina() {
		_currentStamina = 0;
		_wallRunning = false;
	}
	void Check4Air() {
		

	}
	void StartWallRun() {
		Debug.Log("start wall run");
		_wallRunning = true;
		_body.useGravity = false;
		
	}
	void Running() {
		_running = true;
		float speed = 0.1f;
		if (_inAir) speed = 0.05f;
		Vector3 runDir = GetRunningDir();
		transform.position += runDir * speed;
		transform.localRotation = Quaternion.LookRotation(runDir);

	}
	void Sprinting() {
		Debug.Log("Sprinting");
		_currentStamina -= Time.deltaTime;
		_sprinting = true;
		float speed = 0.15f;
		Vector3 runDir = GetRunningDir();
		transform.position += runDir * speed;
		transform.localRotation = Quaternion.LookRotation(runDir);

	}
	Vector3 GetInputDirection() {
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0;
		Vector3 side = Camera.main.transform.right;
		side.y = 0;
		Vector3 runFDir = new Vector3();
		Vector3 runSDir = new Vector3();
		if (_input[0] || _input[1]) {//if forward backward
									 //runDir = forward;
			if (_input[0]) {
				runFDir = forward;
			}
			else runFDir = -forward;

		}
		if (_input[2] || _input[3]) {//if left right
			if (_input[2]) {
				runSDir = -side;
			}
			else {
				runSDir = side;
			}
		}
		return (runSDir + runFDir).normalized;
		
	}
	Vector3 GetRunningDir() {

		Vector3 runDir = GetInputDirection();
		if (_touchingWall) {
			float dot = Vector3.Dot(runDir, _wallCollision._two);
			if(dot > 0) return runDir;

			Vector3 cross = Vector3.Cross(_wallCollision._two, Vector3.up);
			 dot = Vector3.Dot(runDir, cross);
			if (dot > 0)
				runDir = cross;
			else
				runDir = -cross;
		}
		return runDir;
	}
	void DirectionalJump() {
		_inAir = true;

		_body.AddForce((GetRunningDir() + new Vector3(0, 1, 0))*250);
	}
	void Jump() {
		_inAir = true;
		transform.position += new Vector3(0, 0.1f, 0);
		_body.AddForce(Vector3.up * 200);
	}
	private void OnCollisionEnter(Collision collision) {

		if (collision.GetContact(0).normal.y > 0.8f) {//if ground
			_inAir = false;
			_ground = collision.gameObject.transform;
		}
		else { //if not ground
			   //_collidedNormal = collision.GetContact(0).normal;
			_wallCollision = new Pair<Transform, Vector3>(collision.transform, collision.GetContact(0).normal);
			_touchingWall = true;
		}
			
	}
	private void OnCollisionExit(Collision collision) {
		if (collision.transform == _ground) {
			_inAir = true;
			_ground = null;
		}
		else if (_wallCollision._one == collision.transform) {
			_wallCollision = null;
			_touchingWall = false;
		}
	}
	public void UpdateStamina() {
		_staminaBar.value = _currentStamina / _stamina;
	}
}
public class Pair<T, U> {
	public T _one;
	public U _two;
	public Pair() { }
	public Pair(T one, U two) {
		_one = one;
		_two = two;
	}

}