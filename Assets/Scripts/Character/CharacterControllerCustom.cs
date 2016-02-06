using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

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
	private List<RaycastHit> _collisions;

	// Variables
	private Vector3[] _rayOrigins;
	private float _angleBeetweenRays;
	private float _jumpingTime;

	public void Awake() {
		// Creates the original state
		State = new CharacterControllerState();
		_collisions = new List<RaycastHit>();
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

	public void SetInputForce(float horizontalInput, float verticalInput) {
		// Checks if it can move
		if (!CanMove()) {
			horizontalInput = 0;
			verticalInput = 0;
		}

		// Checks the movement type
		bool horizontal = false;
		bool vertical = false;
		switch (Parameters.movementFreedom) {
			case CharacterControllerParameters.MovementFreedom.Horizontal:
				horizontal = true;
				break;
			case CharacterControllerParameters.MovementFreedom.Vertical:
				vertical = true;
				break;
			case CharacterControllerParameters.MovementFreedom.Both:
				horizontal = true;
				vertical = true;
				break;
			default:
				return;
		}

		// Gets the right acceleration
		float acceleration = State.IsGrounded ? Parameters.accelerationOnGround : Parameters.accelerationOnAir;

		// Adds the right force
		if (horizontal) {
			// Horizontal force
			if (Parameters.relativeToGravity) {
				// Relative velocity
				Vector3 horizontalVelocity = GetHorizontalVelocity();
				Vector3 perpendicular = Vector3.Cross(Vector3.forward, Parameters.gravity);
				float gravityAngle = Vector3.Angle(horizontalVelocity, perpendicular);
				float magnitude = horizontalVelocity.magnitude;
				float speed = gravityAngle < 90 ? magnitude : -magnitude;
                SetHorizontalForce(Mathf.Lerp(speed, horizontalInput * Parameters.maxSpeed, acceleration * Time.deltaTime));
			}
			else
			{
				// Global velocity
				_velocity.x = Mathf.Lerp(Velocity.x, horizontalInput * Parameters.maxSpeed, acceleration * Time.deltaTime);
			}
		}
		if (vertical) {
			// Vertical force
			if (Parameters.relativeToGravity) {
				// Relative velocity
				Vector3 verticalVelocity = GetVerticalVelocity();
				float gravityAngle = Vector3.Angle(verticalVelocity, -Parameters.gravity);
				float magnitude = verticalVelocity.magnitude;
				float speed = gravityAngle < 90 ? magnitude : -magnitude;
				SetVerticalForce(Mathf.Lerp(speed, verticalInput * Parameters.maxSpeed, acceleration * Time.deltaTime));
			}
			else {
				// Global velocity
				_velocity.y = Mathf.Lerp(Velocity.y, verticalInput * Parameters.maxSpeed, acceleration * Time.deltaTime);
			}
		}
	}

	public void SetHorizontalForce(float x) {
		Vector3 verticalVelocity = GetVerticalVelocity();
		Vector3 direction = Vector3.Cross(Vector3.forward, Parameters.gravity).normalized;
		_velocity = verticalVelocity + direction * x;
	}

	public void SetVerticalForce(float y) {
		Vector3 horizontalVelocity = GetHorizontalVelocity();
		Vector3 direction = -Parameters.gravity.normalized;
        _velocity = horizontalVelocity + direction * y;
	}

	public Vector3 GetHorizontalVelocity() {
		Vector3 perpendicular = Vector3.Cross(Vector3.forward, Parameters.gravity);
		return GetVelocityOnDirection(perpendicular);
	}

	public Vector3 GetVerticalVelocity() {
		return GetVelocityOnDirection(-Parameters.gravity);
	}

	public Vector3 GetVelocityOnDirection(Vector3 direction) {
		Vector3 normalized = direction.normalized;
		return Vector3.Project(_velocity, direction);
	}

	public bool CanMove() {
		// Checks if the controller accepts input
		switch (Parameters.movementBehaviour) {
			case CharacterControllerParameters.MovementBehaviour.CanMoveAnywhere:
				return true;
			case CharacterControllerParameters.MovementBehaviour.CantMoveSliding:
				return !State.IsSliding;
			case CharacterControllerParameters.MovementBehaviour.CanMoveOnGround:
				return State.IsGrounded;
			case CharacterControllerParameters.MovementBehaviour.CantMove:
				return false;
			default:
				return false;
		}
	}

	public void Jump() {
		// Checks if it can jump
		if (!CanJump())
			return;

		// Calculates the jump speed to reach the desired height
		float jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Parameters.gravity.magnitude * Parameters.jumpMagnitude));
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


	public void FixedUpdate() {
		// Decreaseses the counter of time beetween jumps
		_jumpingTime -= Time.fixedDeltaTime;

		// Adds the gravity to the velocity
		_velocity += Parameters.gravity * Time.fixedDeltaTime;

		// Trys the movement of the entity acording to it's speed
		Move(Velocity * Time.fixedDeltaTime);
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
			_velocity = movement / Time.fixedDeltaTime;
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
		// Creates a list for this frame's collisions
		List<RaycastHit> thisFrameCollisions = new List<RaycastHit>();

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
			// If the collider is not already stored, stores the raycast hit
			if (thisFrameCollisions.Where(e => e.collider == hit.collider).Count() == 0)
				thisFrameCollisions.Add(hit);

			// Checks the angle of the normal
			float normalAngle = Vector3.Angle(hit.normal, -Parameters.gravity);
			if (Mathf.Abs(normalAngle) < Parameters.slopeLimit) {
				// The platform is considered ground
				State.IsGrounded = true;
				State.GroundedObject = hit.collider.gameObject;
				State.IsSliding = false;
			}
			else if (!State.IsGrounded) {
				State.IsSliding = true;
			}
			State.SlopeAngle = normalAngle;

			// Checks the angle of the movement
			float movementAngle = Vector3.Angle(movement, Parameters.gravity);
			if (State.IsGrounded && Mathf.Abs(movementAngle) < Parameters.slopeLimit) {
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

		// Handles the collisions of the frame
		HandleRaycastCollisions(thisFrameCollisions);
	}

	private void HandleRaycastCollisions(List<RaycastHit> thisFrameCollisions) {
		// Checks the new collisions
		foreach (RaycastHit hit in thisFrameCollisions) {

			// Calls the generic collision method for all collisions
			gameObject.SendMessage("OnCustomCollision", hit);

			// Collision enter
			if (_collisions.Where(e => e.collider == hit.collider).Count() == 0)
				gameObject.SendMessage("OnCustomCollisionEnter", hit);

			// Collision stay
			else
				gameObject.SendMessage("OnCustomCollisionStay", hit);
		}

		// Checks the exit collisions
		foreach (RaycastHit hit in _collisions) {

			// Collision exit
			if (thisFrameCollisions.Where(e => e.collider == hit.collider).Count() == 0)
				gameObject.SendMessage("OnCustomCollisionExit", hit);
		}

		// Stores the frame's collisions
		_collisions = thisFrameCollisions;
	}

	public void OnCustomCollision(RaycastHit hit) {
		Rigidbody rb = hit.rigidbody;
		if (rb != null) {
			// If the other object has a rigidbody and it's not kinematic, adds force to it at the contact point
			if (!rb.isKinematic)
				rb.AddForceAtPosition(Velocity * Parameters.mass, hit.point, ForceMode.Impulse);
		}
	}

	public void OnCustomCollisionEnter(RaycastHit hit) {
		// TODO: Add any functionality
	}

	public void OnCustomCollisionStay(RaycastHit hit) {
		// TODO: Add any functionality
	}

	public void OnCustomCollisionExit(RaycastHit hit) {
		// TODO: Add any functionality
	}
}
