using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Custom controller that depends on Unity's CharacterController component.
/// </summary>
public class CharacterControllerCustom : MonoBehaviour {

	#region Properties

	/// <summary>
	/// Contains information about the state of the character controller on the last frame.
	/// </summary>
	public CharacterControllerState State { get; private set; }

	/// <summary>
	/// The parameters of the character controller. They store how the character should behave.
	/// </summary>
	public CharacterControllerParameters Parameters {
		get {
			// If no override paramaters have been specified, return the default parameters
			return _overrideParameters ?? defaultParameters;
		}
		set {
			_overrideParameters = value;
		}
	}

	/// <summary>
	/// The current velocity of the Character. The character will move acording to this
	/// velocity each frame.
	/// </summary>
	public Vector3 Velocity { get { return _velocity; } }

	#endregion

	#region Backing Fields

	/// <summary>
	/// Backing field for the Velocity property.
	/// </summary>
	private Vector3 _velocity;

	/// <summary>
	/// Backing field for the Parameters property.
	/// </summary>
	private CharacterControllerParameters _overrideParameters;

	#endregion

	#region Public Attributes

	/// <summary>
	/// Default parameters. If no parameters have been specified, the default ones will 
	/// be used.
	/// </summary>
	public CharacterControllerParameters defaultParameters;

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to the CharacterController component of the entity.
	/// </summary>
	private CharacterController _controller;

	/// <summary>
	/// A reference to the entity's Transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Variables

	/// <summary>
	/// Time since the last time the character jumped.
	/// </summary>
	private float _jumpingTime;

	/// <summary>
	/// Position on the global coordinates of the entity while standing on a platform.
	/// </summary>
	private Vector3 _activeGlobalPlatformPoint;

	/// <summary>
	/// Position of the entity relative to the platform it's standing on.
	/// </summary>
	private Vector3 _activeLocalPlatformPoint;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called as soon as this entity is created.
	/// It will be called even if the entity is disabled.
	/// </summary>
	public void Awake() {
		// Creates the original state
		State = new CharacterControllerState();

		// Recovers the desired components
		_controller = GetComponent<CharacterController>();
		_transform = transform;
	}

	#region Force Methods

	/// <summary>
	/// Adds a force to the velocity of the character.
	/// </summary>
	/// <param name="force">Amount of force to add</param>
	/// <param name="mode">The type of force it is applying</param>
	public void AddForce(Vector3 force, ForceMode mode = ForceMode.VelocityChange) {
		switch (mode) {
			case ForceMode.VelocityChange:
				// Adds the velocity ignoring it's mass
				_velocity += force;
				break;
			case ForceMode.Impulse:
				// Adds the velocity using it's mass
				_velocity += force / Parameters.mass;
				break;
			case ForceMode.Acceleration:
				// Adds the force over time ignoring it's mass
				_velocity += force * Time.deltaTime;
				break;
			case ForceMode.Force:
				// Adds the force over time using it's mass
				_velocity += (force / Parameters.mass) * Time.deltaTime;
				break;
			default:
				return;
		}
	}

	/// <summary>
	/// Sets the velocity of the character.
	/// </summary>
	/// <param name="force">The new velocity of the character</param>
	public void SetForce(Vector3 force) {
		_velocity = force;
	}

	/// <summary>
	/// Sets the horizontal velocity of the character.
	/// The vertical component will not be modified.
	/// </summary>
	/// <param name="x">The new horizontal velocity of the character</param>
	public void SetHorizontalForce(float x) {
		_velocity.x = x;
	}

	/// <summary>
	/// Sets the vertical velocity of the character.
	/// The horizontal component will not be modified.
	/// </summary>
	/// <param name="y">The new vertical velocity of the character</param>
	public void SetVerticalForce(float y) {
		_velocity.y = y;
	}

	/// <summary>
	/// Adds a force to the velocity of the character.
	/// This force is applied relative to the gravity instead to the global axis.
	/// </summary>
	/// <param name="force">Amount of force to add</param>
	public void AddForceRelative(Vector3 force) {
		// Rotates the force acording to the gravity and adds it to the velocity
		float gravityAngle = Vector3.Angle(Parameters.gravity, Vector3.down);
		if (Vector3.Cross(Parameters.gravity, Vector3.down).z < 0)
			gravityAngle = -gravityAngle;
		_velocity += Quaternion.Euler(0, 0, -gravityAngle) * force;
	}

	/// <summary>
	/// Sets the velocity of the character.
	/// This force is applied relative to the gravity instead to the global axis.
	/// </summary>
	/// <param name="force">The new velocity of the character</param>
	public void SetForceRelative(Vector3 force) {
		// Rotates the force acording to the gravity and sets the velocity to it
		float gravityAngle = Vector3.Angle(Parameters.gravity, Vector3.down);
		if (Vector3.Cross(Parameters.gravity, Vector3.down).z < 0)
			gravityAngle = -gravityAngle;
		_velocity = Quaternion.Euler(0, 0, -gravityAngle) * force;
	}

	/// <summary>
	/// Sets the horizontal velocity of the character.
	/// The vertical component will not be modified.
	/// This force is applied relative to the gravity instead to the global axis.
	/// </summary>
	/// <param name="x">The new horizontal velocity of the character</param>
	public void SetHorizontalForceRelative(float x) {
		Vector3 verticalVelocity = GetVerticalVelocityRelative();
		Vector3 direction = Vector3.Cross(Vector3.forward, Parameters.gravity).normalized;
		_velocity = verticalVelocity + direction * x;
	}

	/// <summary>
	/// Sets the vertical velocity of the character.
	/// The horizontal component will not be modified.
	/// This force is applied relative to the gravity instead to the global axis.
	/// </summary>
	/// <param name="y">The new vertical velocity of the character</param>
	public void SetVerticalForceRelative(float y) {
		Vector3 horizontalVelocity = GetHorizontalVelocityRelative();
		Vector3 direction = -Parameters.gravity.normalized;
		_velocity = horizontalVelocity + direction * y;
	}

	/// <summary>
	/// Returns the velocity of the character on the desired direction.
	/// </summary>
	/// <param name="direction">The direction of the desired velocity</param>
	/// <returns>The velocity of the entity projected on the direction</returns>
	public Vector3 GetVelocityOnDirection(Vector3 direction) {
		return Vector3.Project(_velocity, direction);
	}

	/// <summary>
	/// Returns the velocity of the character on the horizontal axis, relative to the
	/// gravity.
	/// </summary>
	/// <returns>Horizontal component of the velocity</returns>
	public Vector3 GetHorizontalVelocityRelative() {
		Vector3 perpendicular = Vector3.Cross(Vector3.forward, Parameters.gravity);
		return GetVelocityOnDirection(perpendicular);
	}
	
	/// <summary>
	/// Returns the velocity of the character on the vertical axis, relative to the
	/// gravity.
	/// </summary>
	/// <returns>Vertical component of the velocity</returns>
	public Vector3 GetVerticalVelocityRelative() {
		return GetVelocityOnDirection(-Parameters.gravity);
	}

	/// <summary>
	/// Zeroes the velocity of the controller.
	/// </summary>
	public void Stop() {
		_velocity = Vector3.zero;
	}

	#endregion

	#region Input Methods

	/// <summary>
	/// Sets the input of the character on this frame. The velocity will be modified
	/// acording to this input while using the acceleration defined on the parameters.
	/// </summary>
	/// <param name="horizontalInput">Signed-normalized value for the horizontal input</param>
	/// <param name="verticalInput">Signed-normalized value for the vertical input</param>
	public void SetInputForce(float horizontalInput, float verticalInput) {
		// Checks if it can move. Nullifies the input otherwise
		if (!CanMove()) {
			horizontalInput = 0;
			verticalInput = 0;
		}

		// Checks the movement type
		bool horizontal = false;
		bool vertical = false;
		switch (Parameters.movementControl) {
			case CharacterControllerParameters.MovementControl.Horizontal:
				horizontal = true;
				break;
			case CharacterControllerParameters.MovementControl.Vertical:
				vertical = true;
				break;
			case CharacterControllerParameters.MovementControl.Both:
				horizontal = true;
				vertical = true;
				break;
			case CharacterControllerParameters.MovementControl.None:
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

	/// <summary>
	/// Checks if the character can move on his current state. If not, the input
	/// will not be accepted and the character will keep it's velocity.
	/// </summary>
	/// <returns>If the character can move</returns>
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

	/// <summary>
	/// Makes the character jump, modifying the vertical force relatively to the gravity.
	/// The character vertical speed will be replaced.
	/// </summary>
	public void Jump() {
		// Checks if it can jump
		if (!CanJump())
			return;

		// Calculates the jump speed to reach the desired height
		float jumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(Parameters.gravity.magnitude * Parameters.jumpMagnitude));
		SetVerticalForceRelative(jumpSpeed);

		_jumpingTime = Parameters.jumpFrecuency;
	}

	/// <summary>
	/// Checks if the character can jump on his current state.
	/// </summary>
	/// <returns>If the character can jump</returns>
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

	#endregion

	/// <summary>
	/// Unity's method called at the end of the frame.
	/// This method will be called after each Update method is called.
	/// Moves the character acording to it's velocity
	/// </summary>
	public void LateUpdate() {
		// Decreaseses the counter of time beetween jumps
		_jumpingTime -= Time.deltaTime;

		// Adds the gravity to the velocity
		_velocity += Parameters.gravity * Time.deltaTime;

		// Checks if the entity is grounded on a moving platform
		HandleMovingPlatforms();

		// Trys the movement of the entity acording to it's velocity
		Move(Velocity * Time.deltaTime);
	}

	#region Movement Methods

	/// <summary>
	/// Handles the movement of the character while it's standing on a moving platform.
	/// The moving platform can be moved in any way, and the character will follow it
	/// even if it teleports.
	/// </summary>
	private void HandleMovingPlatforms() {
		if (State.GroundedObject != null) {
			// Gets the new global position of the entity relatively to the platform
			Vector3 newGlobalPlatformPoint = State.GroundedObject.transform.TransformPoint(_activeLocalPlatformPoint);
			Vector3 moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

			// Moves the entity to match the platform translation
			if (moveDistance != Vector3.zero)
				_transform.Translate(moveDistance, Space.World);

			// Saves the velocity of the platform
			State.PlatformVelocity = moveDistance / Time.deltaTime;
		}
		else {
			// Resets the velocity of the platform
			State.PlatformVelocity = Vector3.zero;
		}
	}

	/// <summary>
	/// Moves the character the desired distance.
	/// </summary>
	/// <param name="movement">Movement distance</param>
	private void Move(Vector3 movement) {
		// Resets the state, but keeps the platform velocity
		Vector3 temp = State.PlatformVelocity;
		State.Reset();
		State.PlatformVelocity = temp;

		// Clamps the movement
		movement.x = Mathf.Clamp(movement.x, -Parameters.maxVelocity.x, Parameters.maxVelocity.x);
		movement.y = Mathf.Clamp(movement.y, -Parameters.maxVelocity.y, Parameters.maxVelocity.y);
		movement.z = Mathf.Clamp(movement.z, -Parameters.maxVelocity.z, Parameters.maxVelocity.z);

		// Do the actual movement
		_controller.Move(movement);
		Debug.DrawRay(_transform.position, movement, Color.red);

		// If the Z coordinate is clamped, resets it to zero
		if (Parameters.zClamp) {
			Vector3 clamped = _transform.position;
			clamped.z = 0;
			_transform.position = clamped;
		}

		// Stores the global and local position relative to the ground
		if (State.GroundedObject != null) {
			_activeGlobalPlatformPoint = _transform.position;
			_activeLocalPlatformPoint = State.GroundedObject.transform.InverseTransformPoint(_transform.position);
		}
	}

	/// <summary>
	/// Unity's method for handling the collisions derived from the CharacterController
	/// component.
	/// Creates the right state of the character and modifies it's velocity based on
	/// the collision information.
	/// </summary>
	/// <param name="hit">Information of the collision</param>
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
			SetVerticalForceRelative(0);
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

		// Applys force to the other object if it allows it
		Rigidbody otherRigidbody = hit.collider.attachedRigidbody;
		if (otherRigidbody != null && !otherRigidbody.isKinematic) {
			otherRigidbody.AddForce(_velocity * Parameters.mass, ForceMode.Impulse);
		}
	}

	#endregion

	#endregion
}
