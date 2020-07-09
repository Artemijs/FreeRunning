using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
	
	Slider _staminaBar;
	bool[] _input;
	StateData _state;
	Transform _ground;
	public Transform[] debug;
	public AnimationCurve _wallRunCurve;
	Pair<Transform, Vector3> _wallCollision;
	WallRunData _wallRunData;
	public float _gravity;
	public Vector3 _jumpVelocity;

	float _stamina;
	float _currentStamina;
	//Vector3 _wallRunDir;
    // Start is called before the first frame update
    void Start()
    {
		_jumpVelocity = new Vector3();
		_wallRunData = new WallRunData();
		_staminaBar = GameObject.Find("StaminaBar").GetComponent<Slider>();
		//_wallRunPositions = new Transform[5];
		_currentStamina = 1;
		_stamina = _currentStamina;
		_state._sprinting = false;
		_input = new bool[6];
		_wallCollision = null;
		_state._touchingWall = false;
		_state._wallRunning = false;
	}

	// Update is called once per frame
	void Update()
    {
		Debug.Log(_jumpVelocity);
		//GetComponent<Rigidbody>().velocity = Vector3.zero;
		RecoverStamina();
		HandleInput();
		HandleActionCalls();
		if (_state._wallRunning) {
			WallRun();
		}
		UpdateStamina();
		if (_state._inAir) {
			_jumpVelocity -= new Vector3(0, _gravity, 0);
			if (_jumpVelocity.y < -5) {
				_jumpVelocity.y = -5;
			}
			transform.position += _jumpVelocity;
		}
		
	}
	void RecoverStamina() {
		if (!_state._wallRunning && _currentStamina < _stamina) {
			_currentStamina += Time.deltaTime;
		}
	}
	void HandleActionCalls() {
		if (IfRunning()) {
			Running();
		}
		else {
			_state._running = false;
		}
		if (IfSprinting()) {
			Sprinting();
			_currentStamina -= Time.deltaTime;
		}
		if (IfWallRun()) {
			StartWallRun();
		}
		if (_state._wallRunning && !_input[5]) {
			StopWallRun();
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
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && (!(_input[4]) && !_input[5]) && !_state._wallRunning);
	}
	bool IfSprinting() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && (!_input[4]) && _input[5] && !_state._wallRunning && !_state._inAir);
	}
	bool IfWallRun() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && !(_input[4]) && _input[5] && _state._touchingWall && !_state._wallRunning);
	}
	bool IfDirectionalJump() {
		return ((_input[0] || _input[1] || _input[2] || _input[3]) && _input[4] && !_input[5] && !_state._inAir);
	}
	bool IfJump() {
		return ((!_input[0] && !_input[1] && !_input[2] && !_input[3]) && _input[4] && !_input[5] && !_state._inAir);
	}
	void WallRun() {
		Debug.Log("Wall Run");
		float speed = 2f;
		Vector3 next = _wallRunData.GetNext();
		Vector3 movDir = ( next - transform.position).normalized;
		if (Vector3.Distance(next, transform.position) < 10)
			_wallRunData.Next();
		if (_wallRunData.GetNext().x == -69696969) {
			StopWallRun();
		}

		transform.position += (movDir) * speed;
		transform.localRotation = Quaternion.LookRotation(movDir);
	}

	void OutOfStamina() {
		_currentStamina = 0;
		_state._wallRunning = false;
	}
	void StopWallRun() {
		_state._inAir = true;
		_state._wallRunning = false;
		_wallRunData.Reset();
		
	}
	void StartWallRun() {
		Debug.Log("start wall run");
		Grounded();
		_state._wallRunning = true;
		float vThresh = 0.85f;
		bool vertical = false;
		Vector3 movDir = GetInputDirection();
		float dot = Vector3.Dot(movDir, _wallCollision._two);
		Vector3 rayDir = -_wallCollision._two;
		Vector3 side = Vector3.Cross(_wallCollision._two, Vector3.up);
		Vector3 newUp = -Vector3.Cross(_wallCollision._two, side);

		if (Mathf.Abs(dot) > vThresh) {
			movDir = newUp;
			vertical = true;
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


		}
		if (!vertical) movDir.y = 0;
		RaycastHit hit;
		Vector3 pos = transform.position;
		int layer = LayerMask.GetMask("Wall");
		for (int i = 0; i < _wallRunData._wallRunPositions.Length-1; i++) {
			pos += movDir * 25;
			if (Physics.Raycast(pos, rayDir, out hit, 150, layer)) {
				Debug.Log(hit.point);
				pos = hit.point + hit.normal * 14;
				_wallRunData.Set(i, pos);
				debug[i].position = pos;
			}
			else {

			}

		}
		

	}
	void Running() {
		_state._running = true;
		float speed = 2f;
		if (_state._inAir) speed = 1f;
		Vector3 runDir = GetRunningDir();
		transform.position += runDir * speed;
		transform.localRotation = Quaternion.LookRotation(runDir);

	}
	void Sprinting() {
		Debug.Log("Sprinting");
		_currentStamina -= Time.deltaTime;
		_state._sprinting = true;
		float speed = 3f;
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
		if (_state._touchingWall) {
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
		_state._inAir = true;

		_jumpVelocity = GetRunningDir() * 3 + new Vector3(0, 10, 0);
	}
	void Jump() {
		_state._inAir = true;
		transform.position += new Vector3(0, 0.1f, 0);
		_jumpVelocity = new Vector3(0, 10, 0);
	}
	void Grounded() {
		_state._inAir = false;
		_jumpVelocity = new Vector3();
	}

	private void OnCollisionStay(Collision collision) {
		ContactPoint[] cols = collision.contacts;
		for (int i = 0; i < cols.Length; i++) {
			if (cols[i].normal.y > 0.75f) {//if ground
				Debug.Log(cols[i]);
				Grounded();
			}
			else {
				if (!_state._wallRunning) {
					_state._inAir = true;
				}
			}	
		}
	}

	private void OnCollisionEnter(Collision collision) {

		if (collision.GetContact(0).normal.y <0.75f) {//if not ground

			_wallCollision = new Pair<Transform, Vector3>(collision.transform, collision.GetContact(0).normal);
			_state._touchingWall = true;
		}
		
			
	}
	private void OnCollisionExit(Collision collision) {

		if(_wallCollision!= null) {
			if (_wallCollision._one == collision.transform) {
				_wallCollision = null;
				_state._touchingWall = false;
			}
		}
		if (!_state._wallRunning) {
			_state._inAir = true;
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

};
public struct StateData {
	public bool _running;
	public bool _inAir;
	public bool _wallRunning;
	public bool _touchingWall;
	public bool _sprinting;
};
public class WallRunData {
	public Vector3[] _wallRunPositions;
	int _wallRunIndex;
	public WallRunData() {
		_wallRunIndex = 0;
		_wallRunPositions = new Vector3[6];
		Reset();
	}
	public Vector3 GetNext() {
		return _wallRunPositions[_wallRunIndex];
	}
	public void Set(int index, Vector3 pos) {
		_wallRunPositions[index] = pos;
	}
	public void Next() {
		_wallRunIndex++;
	}
	public void Reset() {
		_wallRunIndex = 0;
		for (int i = 0; i < _wallRunPositions.Length; i++) {
			_wallRunPositions[i] = new Vector3(-69696969, -69696969, -69696969);
		}
	}
};