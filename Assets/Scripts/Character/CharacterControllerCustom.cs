using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class CharacterControllerCustom : MonoBehaviour {

	// Properties
	public CharacterControllerState State { get; private set; }
	public Vector3 Velocity { get { return _velocity; } }
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

	// Private attributes
	private CharacterController _controller;
	private CharacterControllerParameters _overrideParameters;

	// Variables
	private float _jumpingTime;
	private Vector3 _activeGlobalPlatformPoint;
	private Vector3 _activeLocalPlatformPoint;

	public void Awake() {
		// Creates the original state
		State = new CharacterControllerState();

		// Recovers the desired components
		_controller = GetComponent<CharacterController>();
	}

	public void SetInputForce(float horizontalInput, float verticalInput) {
		// Checks if it can move
		if (!CanMove())
			return;

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

		// If the input is relative to the gravity, rotates the velocity to match it
		float gravityAngle = 0;
		if (Parameters.relativeToGravity) {
			gravityAngle = Vector3.Angle(Parameters.gravity, Vector3.down);
			if (Vector3.Cross(Parameters.gravity, Vector3.down).z < 0)
				gravityAngle = -gravityAngle;
			_velocity = Quaternion.Euler(0, 0, gravityAngle) * _velocity;
		}

		// Gets the right acceleration
		float acceleration = State.IsGrounded ? Parameters.accelerationOnGround : Parameters.accelerationOnAir;
		// Adds the right forces
		if (horizontal) {
			_velocity.x = Mathf.Lerp(Velocity.x, horizontalInput * Parameters.maxSpeed, acceleration * Time.deltaTime);
		}
		if (vertical) {
			_velocity.y = Mathf.Lerp(Velocity.y, verticalInput * Parameters.maxSpeed, acceleration * Time.deltaTime);
		}

		// If it's grounded on a slope, substracts the necessary vertical speed to stick to the ground
		if (State.IsGrounded && Mathf.Abs(State.SlopeAngle) > 0.001f) {
			_velocity.y -= _velocity.x * Mathf.Sin(State.SlopeAngle * Mathf.Deg2Rad);
		}

		// If the input was relative to the gravity, restores it's orientation
		if (Parameters.relativeToGravity) {
			_velocity = Quaternion.Euler(0, 0, -gravityAngle) * _velocity;
		}
	}

	public void AddForce(Vector3 force) {
		_velocity += force;
	}

	public void SetForce(Vector3 force) {
		_velocity = force;
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
		return Vector3.Project(_velocity, normalized);
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

		// Adds the velocity of the platform
		AddForce(State.PlatformVelocity);

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

		// Checks if the entity is grounded on a moving platform
		HandleMovingPlatforms();

		// Trys the movement of the entity acording to it's speed
		Move(Velocity * Time.deltaTime);
	}

	private void HandleMovingPlatforms() {
		if (State.GroundedObject != null) {
			// Gets the new global position of the entity relatively to the platform
			Vector3 newGlobalPlatformPoint = State.GroundedObject.transform.TransformPoint(_activeLocalPlatformPoint);
			Vector3 moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

			// Moves the entity to match the platform translation
			if (moveDistance != Vector3.zero)
				transform.Translate(moveDistance, Space.World);

			// Saves the velocity of the platform
			State.PlatformVelocity = moveDistance / Time.deltaTime;
		}
		else {
			// Resets the velocity of the platform
			State.PlatformVelocity = Vector3.zero;
		}
	}

	private void Move(Vector3 movement) {
		// Resets the state
		State.Reset();

		// Clamps the movement
		movement.x = Mathf.Clamp(movement.x, -Parameters.maxVelocity.x, Parameters.maxVelocity.x);
		movement.y = Mathf.Clamp(movement.y, -Parameters.maxVelocity.y, Parameters.maxVelocity.y);
		movement.z = Mathf.Clamp(movement.z, -Parameters.maxVelocity.z, Parameters.maxVelocity.z);

		// Do the actual movement
		_controller.Move(movement);
		Debug.DrawRay(transform.position, movement, Color.red);

		// Stores the global and local position relative to the ground
		if (State.GroundedObject != null) {
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatformPoint = State.GroundedObject.transform.InverseTransformPoint(transform.position);
		}
	}

	public void OnControllerColliderHit(ControllerColliderHit hit) {
		// There has been collisions this frame
		State.HasCollisions = true;

		// Spheres have their normal inverted for whatever reason
		Vector3 normal = hit.normal;
		if (hit.collider is SphereCollider)
			normal = -hit.normal;

		// Looks for the angle beetween the collision nomal an the gravity
		State.SlopeAngle = Vector3.Angle(normal, -Parameters.gravity);
		if (Vector3.Cross(normal, -Parameters.gravity).z < 0)
			State.SlopeAngle = -State.SlopeAngle;
		if (Mathf.Abs(State.SlopeAngle) < _controller.slopeLimit) {
			// The collider is considered ground
			State.IsGrounded = true;
			State.IsSliding = false;
			State.GroundedObject = hit.collider.gameObject;

			// Removes the velocity's vertical component
			SetVerticalForce(0);
		}
		else {
			// The collider is considered a slope
			State.IsGrounded = false;
			State.IsSliding = true;
			State.GroundedObject = null;

			// Projects the speed to the normal's perpendicular
			Vector3 normalPerpendicular = Vector3.Cross(normal, Vector3.forward);
			_velocity = Vector3.Project(_velocity, normalPerpendicular);
		}

		// Apply force to the other object if it allows it
		Rigidbody otherRigidbody = hit.collider.attachedRigidbody;
		if (otherRigidbody != null && !otherRigidbody.isKinematic) {
			otherRigidbody.AddForceAtPosition(_velocity * Parameters.mass, hit.point, ForceMode.Impulse);
		}
	}
}
