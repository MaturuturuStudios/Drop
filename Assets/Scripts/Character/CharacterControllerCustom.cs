using UnityEngine;
using System.Collections;
using System;

public class CharacterControllerCustom : MonoBehaviour {

	// Properties
	public CharacterControllerState State { get; private set; }
	public Vector3 Velocity { get { return _velocity; } }
	public bool HandleCollisions { get; set; }
	public CharacterControllerParameters Parameters {
		get {
			// If no override paramaters have been specified, return the default parameters
			return _overrideParameters ?? defaultParameters;
		}
	}

	// Backing fields
	private Vector3 _velocity;

	// Public attributes
	public LayerMask platformMask;
	public CharacterControllerParameters defaultParameters;
	public float skinWidth = 0.02f;
	public int totalRays = 16;

	// Private attributes
	private SphereCollider _collider;
	private CharacterControllerParameters _overrideParameters;

	// Variables
	private Vector3[] _rayOrigins;
	private float _angleBeetweenRays;
	private float _jumpingTime;

	public void Awake() {
		// Creates the original state
		State = new CharacterControllerState();
		HandleCollisions = true;

		// Creates the arrays for the ray's origins
		_rayOrigins = new Vector3[totalRays];

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
		// Checks if it can jump
		if (!CanJump())
			return;

		// Calculates the jump speed to reach the desired height
		float jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Parameters.gravity.y * Parameters.jumpMagnitude));
		SetVerticalForce(jumpSpeed);
		_jumpingTime = Parameters.jumpFrecuency;
	}

	public bool CanJump() {
		// If it has recently jumped, it cannot jump again
		if (_jumpingTime > 0)
			return false;

		// Checks the jumping behaviour
		switch (Parameters.jumpBehaviour) {
			case CharacterControllerParameters.JumpBehaviour.CanJumpAnywhere:
				return true;
			case CharacterControllerParameters.JumpBehaviour.CanJumpOnGround:
				return State.IsGrounded;
			case CharacterControllerParameters.JumpBehaviour.CantJump:
				return false;
			default:
				return false;
		}
	}


	public void LateUpdate() {
		// Decreaseses the counter of time beetween jumps
		_jumpingTime -= Time.deltaTime;

		// Adds the gravity to the velocity
		_velocity += Parameters.gravity * Time.deltaTime;

		// Trys the movement of the entity acording to it's speed
		Move(Velocity * Time.deltaTime);
	}

	private void Move(Vector3 movement) {
		// Resets the state
		State.Reset();

		if (HandleCollisions) {
			// Precomputes the ray's origins
			CalculateRayOrigins(movement);

			// Checks for possible collisions
			CheckCollisions(ref movement);
		}

		// Clamps the movement
		movement.x = Mathf.Clamp(movement.x, -Parameters.maxVelocity.x, Parameters.maxVelocity.x);
		movement.y = Mathf.Clamp(movement.y, -Parameters.maxVelocity.y, Parameters.maxVelocity.y);
		movement.z = Mathf.Clamp(movement.z, -Parameters.maxVelocity.z, Parameters.maxVelocity.z);

		// Do the actual movement
		transform.Translate(movement, Space.World);

		// Updates the velocity with the actual value this frame
		if (Time.deltaTime > 0)
			_velocity = movement / Time.deltaTime;
	}

	private void CalculateRayOrigins(Vector3 movement) {
		// Stores the angle beetween rays
		_angleBeetweenRays = 360.0f / totalRays;

		// Calculates the ray's origins
		Vector3 scale = transform.localScale;
		for (int i = 0; i < _rayOrigins.Length; i++) {
			// Calculates the right coordinates and creates the point
			float x = transform.position.x + (_collider.radius * scale.x - skinWidth) * Mathf.Cos(i * _angleBeetweenRays * Mathf.Deg2Rad);
			float y = transform.position.y + (_collider.radius * scale.y - skinWidth) * Mathf.Sin(i * _angleBeetweenRays * Mathf.Deg2Rad);
			_rayOrigins[i] = new Vector3(x, y, 0);
		}
	}

	private void CheckCollisions(ref Vector3 movement) {
		// Casts rays according to the movement direction
		for (int i = 0; i < _rayOrigins.Length; i++) {
			// Caulculates the origin and direction of the ray
			Vector3 rayOrigin = _rayOrigins[i];
			Vector3 rayDirection = movement.normalized;
			float rayDistance = movement.magnitude + skinWidth;

			// Casts the ray
			Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
			RaycastHit hit;
			if (!Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, platformMask))
				continue;

			// From now on, a collision has occured
			State.HasCollisions = true;

			// Checks the angle of the normal
			float normalAngle = Vector3.Angle(hit.normal, -Parameters.gravity);
			if (Mathf.Abs(normalAngle) < Parameters.slopeLimit) {
				// The platform is considered ground
				State.IsGrounded = true;
				State.GroundedObject = hit.collider.gameObject;
			}
			
			// Checks the angle of the movement
			float movementAngle = Vector3.Angle(movement, Parameters.gravity);
			if (Mathf.Abs(normalAngle) < Parameters.slopeLimit && Mathf.Abs(movementAngle) < Parameters.slopeLimit) {
				// Clamps the movement to the collision point
				movement = hit.point - rayOrigin;
				movement -= movement.normalized * skinWidth;
			}
			else {
				// Modifies the movement to slide with the collider
				Vector3 repositionVector = Vector3.Project(-movement, hit.normal);
				movement += repositionVector;
			}

			// Precaution check
			if (rayDistance < skinWidth + 0.0001f)
				break;
		}
	}

	public void OnTriggerEnter(Collider other) {
		// TODO
	}

	public void OnTriggerExit(Collider other) {
		// TODO
	}
}
