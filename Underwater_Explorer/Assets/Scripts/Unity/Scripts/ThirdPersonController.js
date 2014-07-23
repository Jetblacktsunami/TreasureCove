/****************************************************
*
* 	NOTE1: This is a modified version of the standard asset
* 		third person controller that unity provides
*
*	NOTE2: I decided to use these particular assets rather
* 		than write my own to show that i can work with other
*		code that wasnt written by myself. Since UE4 is heavy
*		on combining behaviors and inheriting, even though
*		i didn't show any type of inheritance in my code
****************************************************/

// Require a character controller to be attached to the same game object
@script RequireComponent(CharacterController)

enum CharacterState 
{
	Idle = 0,
	Walking = 1,
	Running = 3,
	Jumping = 4,
}

private var _characterState : CharacterState;

var walkSpeed = 2.0; // The speed when walking
var runSpeed = 6.0; // when pressing "Fire3" button (cmd) we start running
var speedSmoothing = 10.0; // The gravity in controlled descent mode
var rotateSpeed = 500.0;
var trotAfterSeconds = 3.0;

private var lockCameraTimer = 0.0; // The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.

private var moveDirection = Vector3.zero; // The current move direction in x-z
private var verticalSpeed = 0.0; // The current vertical speed
private var moveSpeed = 0.0; // The current x-z move speed

private var collisionFlags : CollisionFlags; // The last collision flags returned from controller.Move

private var movingBack = false; // Are we moving backwards (This locks the camera to not do a 180 degree spin)
private var isMoving = false; // Is the user pressing any keys?

private var walkTimeStart = 0.0; // When did the user start walking (Used for going into trot after a while)
private var isControllable = true;
private var cameraTransform;

function Awake ()
{
	moveDirection = transform.TransformDirection(Vector3.forward);	
	cameraTransform = Camera.main.transform;		
}

function UpdateSmoothedMovementDirection ()
{	
	// Forward vector relative to the camera along the x-z plane	
	var forward = cameraTransform.TransformDirection(Vector3.forward);
	forward.y = 0.0;
	var dive = Vector3( 0.0 , 0.0 , 0.0);
	dive.y = Input.GetAxis("Mouse Y") || Input.GetAxis("Dive");
	
	forward = forward.normalized;

	var v = Input.GetAxisRaw("Vertical");
	var h = Input.GetAxisRaw("Horizontal");

	// Are we moving backwards or looking backwards
	if (v < -0.2)
	{
		movingBack = true;
	}
	else
		movingBack = false;
	
		// Right vector relative to the camera
	// Always orthogonal to the forward vector
	var right = Vector3(forward.z, 0.0 , -forward.x);
	
	var wasMoving = isMoving;
	isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
	
	// Target direction relative to the camera
	var targetDirection = h * right + v * forward +  dive;
	
	// Grounded controls
	// Lock camera for short period when transitioning moving & standing still
	lockCameraTimer += Time.deltaTime;
	if (isMoving != wasMoving)
		lockCameraTimer = 0.0;

	// We store speed and direction seperately,
	// so that when the character stands still we still have a valid forward direction
	// moveDirection is always normalized, and we only update it if there is user input.
	if (targetDirection != Vector3.zero)
	{
		// If we are really slow, just snap to the target direction
		if (moveSpeed < walkSpeed * 0.9 )
		{
			moveDirection = targetDirection.normalized;
		}
		// Otherwise smoothly turn towards it
		else
		{
			moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			
			moveDirection = moveDirection.normalized;
		}
	}
	
	// Smooth the speed based on the current target direction
	var curSmooth = speedSmoothing * Time.deltaTime;
	
	// Choose target speed
	//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
	var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0);

	_characterState = CharacterState.Idle;
	
	// Pick speed modifier
	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
	{
		targetSpeed *= runSpeed;
		_characterState = CharacterState.Running;
	}
	else
	{
		targetSpeed *= walkSpeed;
		_characterState = CharacterState.Walking;
	}
	
	moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
	
	// Reset walk time start when we slow down
	if (moveSpeed < walkSpeed * 0.3)
		walkTimeStart = Time.time;
	
}

function Update() {
	
	if (!isControllable)
	{
		Input.ResetInputAxes(); // kill all inputs if not controllable.
	}

	UpdateSmoothedMovementDirection();
	
	// Calculate actual motion
	var movement = moveDirection * moveSpeed + Vector3 (0, verticalSpeed, 0);
	movement *= Time.deltaTime;
	
	// Move the controller
	var controller : CharacterController = GetComponent(CharacterController);
	collisionFlags = controller.Move(movement);
	
	var xzMove = movement;
	xzMove.y = 0;
	if (xzMove.sqrMagnitude > 0.001)
	{
		transform.rotation = Quaternion.LookRotation(xzMove);
	}		
}

function OnControllerColliderHit (hit : ControllerColliderHit )
{
//	Debug.DrawRay(hit.point, hit.normal);
	if (hit.moveDirection.y > 0.01) 
		return;
}

function IsMovingBackwards () 
{
	return movingBack;
}

function GetLockCameraTimer()
{
	return lockCameraTimer;
}
