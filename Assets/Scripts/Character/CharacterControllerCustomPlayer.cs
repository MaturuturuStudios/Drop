using UnityEngine;

/// <summary>
/// Communication point beetween the input system and the character controller.
/// </summary>
[RequireComponent(typeof(CharacterControllerCustom))]
public class CharacterControllerCustomPlayer : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The time the jump button will be considered pressed.
	/// </summary>
	public float jumpPressTolerance = 0.1f;

	/// <summary>
	/// The time the player will be able to jump after the character stops
	/// being grounded.
	/// </summary>
	public float jumpDelayTolerance = 0.1f;

	/// <summary>
	/// The time the character will stick to a slope after releaseing
	/// the direction button.
	/// </summary>
	public float slopeStickTolerance = 0.1f;

	/// <summary>
	/// Amount of friction applied to the perpendicular velocity while the
	/// player is on a wind tube.
	/// </summary>
	[Range(0, 1)]
	public float windTubeFriction = 0.05f;

	#endregion

	#region Properties

	/// <summary>
	/// Signed-normalized horizontal input the controller will receive this frame.
	/// </summary>
	public float HorizontalInput { get; set; }

	/// <summary>
	/// Signed-normalized vertical input the controller will receive this frame.
	/// </summary>
	public float VerticalInput { get; set; }

	/// <summary>
	/// The direction the character is fancing, defined by the last input received.
	/// </summary>
	public Vector3 FacingDirection { get; private set; }

	/// <summary>
	/// The wind tube which the player is currently standing on.
	/// </summary>
	public WindTube CurrentWindTube { get; set; }

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's CharacterControllerCustom component.
	/// </summary>
	private CharacterControllerCustom _controller;

    /// <summary>
    /// Flag that indicates if the jump button has been pressed since the
    /// previous fixed tick.
    /// </summary>
    private bool _jumpButtonPressed;

	/// <summary>
	/// Time since the jump button was pressed.
	/// </summary>
	private float _jumpPressTime;

	/// <summary>
	/// Time since the character stopped being grounded.
	/// </summary>
	private float _jumpDelayTime;

	/// <summary>
	/// Flag that indicates if the player is stuck to a slope.
	/// </summary>
	private bool _isAlreadyStuck;

	/// <summary>
	/// Time since the character sticked to a slope.
	/// </summary>
	private float _slopeStickTime;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called as soon as this entity is created.
	/// It will be called even if the entity is disabled.
	/// </summary>
	public void Awake() {
		// Recovers the other components
		_controller = GetComponent<CharacterControllerCustom>();

		// By default, the player starts facing right
		FacingDirection = Vector3.right;
	}

	/// <summary>
	/// Unity's method called every frame.
	/// Sends the input to the controller.
	/// </summary>
	public void FixedUpdate() {
		// Decreses the timers
		_jumpPressTime -= Time.fixedDeltaTime;
		_jumpDelayTime -= Time.fixedDeltaTime;
		_slopeStickTime -= Time.fixedDeltaTime;

		// Checks where the player is facing
		FacingDirection = new Vector3(HorizontalInput, VerticalInput, 0).normalized;

		if (CurrentWindTube == null) {
			// If the character is on a slope, checks if the character is stuck to it
			float newHorizontalInput = HorizontalInput;
			if (_controller.State.IsOnSlope) {
				// Gets the direction of the slope
				float slopeOrientationSign = -Mathf.Sign(Mathf.Sin(_controller.State.SlopeAngle));

				// Checks if the slope orientation is the same as the horizontal speed
				// If the horizontal speed and slope orientation have the same sign, their multiplication will be positive
				if (!_isAlreadyStuck || HorizontalInput * slopeOrientationSign > 0) {
					// Restarts the stick timer
					_slopeStickTime = slopeStickTolerance;
					_isAlreadyStuck = true;
				}

				// If the timer has not expired yet, inverses the input to keep the character stuck to the slope
				if (_slopeStickTime > 0)
					// Overrides the input with the orientation of the slope
					newHorizontalInput = slopeOrientationSign;
			}
			else {
				_isAlreadyStuck = false;
			}

			// Sets the input force of the character controller
			_controller.SetInputForce(newHorizontalInput, VerticalInput);
		}
		else {
			// The character is on a wind tube
			Vector3 movementForce = Vector3.Project(_controller.Velocity, CurrentWindTube.transform.right);

			// Adds a friction to the velocity
			movementForce *= (1 - CurrentWindTube.characterFrictionFactor * windTubeFriction);

			// Adds the input force
			Vector3 inputForce = new Vector3(HorizontalInput, VerticalInput, 0);
			inputForce = Vector3.Project(inputForce, CurrentWindTube.transform.right);

			// Applys the final force
			Vector3 finalForce = Vector3.Project(_controller.Velocity, CurrentWindTube.transform.up);
			finalForce += movementForce + inputForce;
			_controller.SetForce(finalForce);
		}

		// Checks if the jump button has been recently pressed
		if (_jumpButtonPressed) {
            // While this counter is running the character will jump as soon as it cans
			_jumpPressTime = jumpPressTolerance;

            // Restores the flag
            _jumpButtonPressed = false;
		}

		// Checks if the character was grounded recently
		bool groundTrick = false;
		if (_controller.State.IsGrounded) {
			_jumpDelayTime = jumpDelayTolerance;
		}
		else if (_jumpDelayTime >= 0) {
			// Tricks the controller to think it's still grounded
			_controller.State.IsGrounded = true;
			groundTrick = true;
		}

		// Makes the character jump
		if (_jumpPressTime >= 0 && _controller.CanJump()) {
			_jumpDelayTime = -1;
			_jumpPressTime = -1;
			_controller.Jump();
		}

		// Restores the controller state, undoing the trick
		if (groundTrick)
			_controller.State.IsGrounded = false;
	}

	/// <summary>
	/// Removes the input of the controller. If the optional parameter
	/// is received, instantly stops the entity, removing it's velocity.
	/// </summary>
	/// <param name="hardStop">If the entity velocity should be removed</param>
	public void Stop(bool hardStop = false) {
		// Removes the input
		HorizontalInput = 0;
		VerticalInput = 0;
		_jumpButtonPressed = false;

		// If it has to, hard stops the controller
		if (hardStop)
			_controller.Stop();
	}

    /// <summary>
    /// Makes the character jump. If it can't at this moment, there will be
    /// a grace period where the character will jump automatically as soon as
    /// it cans.
    /// </summary>
    public void Jump() {
        // Sets the jump button flag
        _jumpButtonPressed = true;
    }

	#endregion
}
