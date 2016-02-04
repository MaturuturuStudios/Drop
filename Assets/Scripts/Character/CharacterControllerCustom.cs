using UnityEngine;
using System.Collections;
using System;

public class CharacterControllerCustom : MonoBehaviour {

	// Constants
	private static readonly float slopeLimitTangant = Mathf.Tan(75 * Mathf.Deg2Rad);
	private const float skinWidth = 0.02f;
	private const int totalHorizontalRays = 7;
	private const int totalVerticalRays = 7;

	// Properties
	public CharacterControllerState State { get; private set; }
	public Vector3 Velocity { get { return _velocity; } }
	public bool CanJump { get { return false; } }
	public bool HandleCollisions { get; set; }
	public CharacterControllerParameters Parameters {
		get {
			// If not override paramaters have been specified, return the default parameters
			return _overrideParameters ?? defaultParameters;
		}
	}

	// Backing fields
	private Vector3 _velocity;

	// Public attributes
	public LayerMask platformMask;
	public CharacterControllerParameters defaultParameters;

	// Private attributes
	private SphereCollider _collider;
	private CharacterControllerParameters _overrideParameters;

	// Variables
	private Vector3[] _horizontalRayOrigins;
	private Vector3[] _verticalRayOrigins;

	public void Awake() {
		// Creates the original state
		State = new CharacterControllerState();
		HandleCollisions = true;

		// Creates the arrays for the ray's origins
		_horizontalRayOrigins = new Vector3[totalHorizontalRays];
		_verticalRayOrigins = new Vector3[totalVerticalRays];

		// Recovers the desired components
		_collider = GetComponent<SphereCollider>();
	}

	public void AddForce(Vector3 force) {
		_velocity += force;
	}

	public void SetForce(Vector3 force) {
		_velocity = force;
	}

	public void SetHorizontalForce(float x) {
		_velocity.x = x;
	}

	public void SetVerticalForce(float y) {
		_velocity.y = y;
	}

	public void Jump() {
		// TODO
	}

	public void LateUpdate() {
		// Trys the movement of the entity acording to it's speed
		Move(Velocity * Time.deltaTime);
	}

	private void Move(Vector3 movement) {
		// Stores the grounded status of the entity and resets the state
		bool grounded = State.IsGrounded;
		State.Reset();

		if (HandleCollisions) {
			HandlePlatforms();
			CalculateRayOrigins(movement);

			// Checks for a possible slope if the enitty is falling and grounded
			if (movement.y < 0 && grounded)
				HandleVerticalSlope(ref movement);

			// Checks for a possible collision horizontally or vertically
			if (Mathf.Abs(movement.x) > 0)
				MoveHorizontally(ref movement);
			MoveVertically(ref movement);
		}

		// Do the actual movement
		transform.Translate(movement, Space.World);

		// TODO: Additional moving platform code

		// Updates the velocity with the actual value this frame
		if (Time.deltaTime > 0)
			_velocity = movement / Time.deltaTime;

		// Clamps the velocity and sets the vertical speed to 0 if it was originated by a slope
		_velocity.x = Mathf.Min(_velocity.x, Parameters.maxVelocity.x);
		_velocity.y = Mathf.Min(_velocity.y, Parameters.maxVelocity.y);
		if (State.IsMovingUpSlope)
			_velocity.y = 0;


	}

	private void HandlePlatforms() {
		// TODO
	}

	private void CalculateRayOrigins(Vector3 movement) {
		Vector3 scale = transform.localScale;

		// Calculates the horizontal rays
		float distanceBeetweenHorizontalRays = 2 * (_collider.radius * scale.y - skinWidth) / (totalHorizontalRays - 1);
		float angleBeetweenHorizontalRays = 180.0f / (totalHorizontalRays - 1);
        float referenceYPosition = transform.position.y - _collider.radius * scale.y;
		float horizontalDirectionFactor = movement.x > 0 ? 1 : -1;
		for (int i = 0; i < _horizontalRayOrigins.Length; i++) {
			// Calculates the right coordinates and creates the point
			float x = transform.position.x + horizontalDirectionFactor * _collider.radius * scale.x * Mathf.Cos((i * angleBeetweenHorizontalRays - 90) * Mathf.Deg2Rad);
			float y = referenceYPosition + i * distanceBeetweenHorizontalRays;
			_horizontalRayOrigins[i] = new Vector3(x, y, 0);
		}

		// Calculates the vertical rays
		float distanceBeetweenVerticalRays = 2 * (_collider.radius * scale.x - skinWidth) / (totalVerticalRays - 1);
		float angleBeetweenVerticalRays = 180.0f / (totalVerticalRays - 1);
		float referenceXPosition = transform.position.x - _collider.radius * scale.x;
		float verticalDirectionFactor = movement.y > 0 ? 1 : -1;
		for (int i = 0; i < _verticalRayOrigins.Length; i++) {
			// Calculates the right coordinates and creates the point
			float x = referenceXPosition + i * distanceBeetweenVerticalRays;
			float y = transform.position.y + verticalDirectionFactor * _collider.radius * scale.y * Mathf.Sin((i * angleBeetweenVerticalRays + 180) * Mathf.Deg2Rad);
			_verticalRayOrigins[i] = new Vector3(x, y, 0);
		}
	}

	private void MoveHorizontally(ref Vector3 currentMovement) {
		// Looks for the movement direction
		bool isMovingRight = currentMovement.x > 0;
		float rayDistance = Mathf.Abs(currentMovement.x) + skinWidth;
		Vector3 rayDirection = isMovingRight ? Vector3.right : Vector3.left;

		for (int i = 0; i < _horizontalRayOrigins.Length; i++) {
			Debug.DrawRay(_horizontalRayOrigins[i], rayDirection * rayDistance, Color.red);
			RaycastHit hit;
			if (!Physics.Raycast(_horizontalRayOrigins[i], rayDirection, out hit, rayDistance, platformMask))
				continue;

			if (i == 0 && HandleHorizontalSlope(ref currentMovement, Vector3.Angle(hit.normal, Vector3.up), isMovingRight))
				break;

			currentMovement.x = hit.point.x - _horizontalRayOrigins[i].x;
			rayDistance = Mathf.Abs(currentMovement.x);

			if (isMovingRight) {
				currentMovement.x -= skinWidth;
				State.IsCollidingRight = true;
			}
			else {
				currentMovement.x += skinWidth;
				State.IsCollidingLeft = true;
			}

			if (rayDistance < skinWidth + 0.001f)
				break;
		}
	}

	private void MoveVertically(ref Vector3 currentMovement) {
		// TODO
	}

	private void HandleVerticalSlope(ref Vector3 currentMovement) {
		// TODO
	}

	private bool HandleHorizontalSlope(ref Vector3 currentMovement, float angle, bool isGoingRight) {
		// TODO
		return false;
	}

	public void OnTriggerEnter(Collider other) {
		// TODO
	}

	public void OnTriggerExit(Collider other) {
		// TODO
	}
}
