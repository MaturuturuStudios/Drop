using System;
using UnityEngine;
using System.Text;

/// <summary>
/// The parameters of the character controller. They store how the character should behave.
/// </summary>
[Serializable]
public class CharacterControllerParameters {

	#region Custom Enumerations

	/// <summary>
	/// Defines which axis the input is going to affect. Note that these axis will be affected by the
	/// input even if the controller is not receiving input or is not able to move.
	/// </summary>
	public enum MovementControl {

		/// <summary>
		/// Will receive input on the horizontal axis. If it's not receiving input, it will stop on 
		/// this axis.
		/// </summary>
		Horizontal,

		/// <summary>
		/// Will receive input on the vertical axis. If it's not receiving input, it will stop on 
		/// this axis.
		/// </summary>
		Vertical,

		/// <summary>
		/// Will receive input on both axis. If it's not receiving input, it will stop.
		/// </summary>
		Both,

		/// <summary>
		/// Will not receive input. It will stop even if it's receiving input.
		/// </summary>
		None
	}

	/// <summary>
	/// Defines in which situations the input will be accepted. If it can't, no input modifications 
	/// will be performed so the entity will keep it's velocity.
	/// </summary>
	public enum MovementBehaviour {

		/// <summary>
		/// Will always accept input.
		/// </summary>
		CanMoveAnywhere,

		/// <summary>
		/// Will always but when on a slope accept input.
		/// </summary>
		CantMoveOnSlope,

		/// <summary>
		/// Will always but when sliding down a slope accept input.
		/// </summary>
		CantMoveSliding,

		/// <summary>
		/// Will accept input while grounded.
		/// </summary>
		CanMoveOnGround,

		/// <summary>
		/// Will not accept input.
		/// </summary>
		CantMove
	}

	/// <summary>
	/// Defines in which situations the character will be able to jump.
	/// </summary>
	public enum JumpBehaviour {

		/// <summary>
		/// Will always be able to jump.
		/// </summary>
		CanJumpAnywhere,

		/// <summary>
		/// Will be able to jump while on a slope.
		/// </summary>
		CanJumpOnSlope,

		/// <summary>
		/// Will be able to jump while sliding.
		/// </summary>
		CanJumpSliding,

		/// <summary>
		/// Will be able to jump while grounded.
		/// </summary>
		CanJumpOnGround,

		/// <summary>
		/// Will not be able to jump.
		/// </summary>
		CantJump
	}

	#endregion

	#region Properties

	/// <summary>
	/// Returns the gravity of the character. It will return the custom one if it has been set
	/// to do it. Else, it will return the unity's default gravity.
	/// </summary>
	public Vector3 Gravity {
		get {
			return useCustomGravity ? customGravity : Physics.gravity;
		}
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines which axis the input is going to affect. Note that these axis will be affected by the
	/// input even if the controller is not receiving input but won't if it's not able to move.
	/// </summary>
	public MovementControl movementControl = MovementControl.Horizontal;

	/// <summary>
	/// Defines in which situations the input will be accepted. If it can't, no input modifications 
	/// will be performed so the entity will keep it's velocity.
	/// </summary>
	public MovementBehaviour movementBehaviour = MovementBehaviour.CantMoveOnSlope;

	/// <summary>
	/// If enabled, the input will be relative to the gravity instead of the vertical axis.
	/// </summary>
	public bool relativeToGravity = false;

	/// <summary>
	/// The maximum speed that can be achieved by player input. This speed will be achieved while
	/// receiving complete input after a time period of acceleration.
	/// </summary>
	public float maxSpeed = 8;

	/// <summary>
	/// The acceleration of the character while grounded. This affects the time the character takes
	/// to achieve maximum speed.
	/// </summary>
	public float accelerationOnGround = 10;

	/// <summary>
	/// The acceleration of the character while floating. This affects the time the character takes
	/// to achieve maximum speed.
	/// </summary>
	public float accelerationOnAir = 3;

	/// <summary>
	/// The amount of stickiness to apply to the character while moving down a slope.
	/// </summary>
	[Range(0, 2)]
	public float slopeStickiness = 1.1f;

	/// <summary>
	/// Defines the maximum angle vertical walls will have. Beetween this angle, and the slope limit,
	/// the character will slide along the object.
	/// </summary>
	[Range(0, 90)]
	public float maxWallSlideAngle = 90;

	/// <summary>
	/// Defines the drag factor that will affect the character's velocity while sliding down a
	/// slope.
	/// </summary>
	[Range(0, 1)]
	public float slidingDragFactor = 0.5f;

	/// <summary>
	/// Defines the precission when comparing angles. Higher values result in more reliable movement
	/// but can produce errors.
	/// </summary>
	public float angleThereshold = 0.1f;

	/// <summary>
	/// Defines in which situations the character will be able to jump.
	/// </summary>
	public JumpBehaviour jumpBehaviour = JumpBehaviour.CanJumpOnSlope;

	/// <summary>
	/// Amount of time beetween jumps. The character will not be able to jump after this time period
	/// has passed.
	/// </summary>
	public float jumpFrequency = 0.25f;

	/// <summary>
	/// Jumping factor. Defines the height that should be achieved at the peak of the jump.
	/// </summary>
	public float jumpMagnitude = 1;

	/// <summary>
	/// Jumping height reached when wall jumping.
	/// </summary>
	public float wallJumpHeight = 1;

	/// <summary>
	/// Amount of distance traveled when reaching the peak of a wall jump.
	/// </summary>
	public float wallJumpDistance = 2;

	/// <summary>
	/// The base mass of the character, when it's size is 1. This will affect how much force will the
	/// character apply to other entitys when colliding whith them.
	/// </summary>
	public float baseMass = 1;

	/// <summary>
	/// If enabled, the character will use it's own gravity instead of unity's.
	/// </summary>
	public bool useCustomGravity = true;

	/// <summary>
	/// The gravity affecting the character. Will pull the character on this direction each frame.
	/// </summary>
	public Vector3 customGravity = new Vector3(0, -25, 0);

	/// <summary>
	/// The maximimum velocity the character can move. It's independent of the input and will often
	/// be achieved by the influence of physical forces.
	/// </summary>
	public Vector3 maxVelocity = new Vector3(float.MaxValue, float.MaxValue, 0);

	/// <summary>
	/// If enabled, the position's Z coordinate of the entity will be zeroed each frame.
	/// </summary>
	public bool zClamp = true;

	#endregion

	#region Methods

	/// <summary>
	/// Creates a copy of this parameters.
	/// </summary>
	/// <returns>A copy of this parameters</returns>
	public CharacterControllerParameters Clone() {
		CharacterControllerParameters clone = new CharacterControllerParameters();
		clone.movementControl = movementControl;
		clone.movementBehaviour = movementBehaviour;
		clone.relativeToGravity = relativeToGravity;
		clone.maxSpeed = maxSpeed;
		clone.accelerationOnGround = accelerationOnGround;
		clone.accelerationOnAir = accelerationOnAir;
		clone.slopeStickiness = slopeStickiness;
		clone.maxWallSlideAngle = maxWallSlideAngle;
        clone.slidingDragFactor = slidingDragFactor;
		clone.angleThereshold = angleThereshold;
        clone.jumpBehaviour = jumpBehaviour;
		clone.jumpFrequency = jumpFrequency;
		clone.jumpMagnitude = jumpMagnitude;
		clone.wallJumpHeight = wallJumpHeight;
		clone.wallJumpDistance = wallJumpDistance;
        clone.baseMass = baseMass;
		clone.useCustomGravity = useCustomGravity;
        clone.customGravity = customGravity;
		clone.maxVelocity = maxVelocity;
		clone.zClamp = zClamp;

		return clone;
	}

	/// <summary>
	/// Overrides the ToString method with a more detailed information string.
	/// </summary>
	/// <returns>Detailed information about the parameters</returns>
	override public String ToString() {
		StringBuilder sb = new StringBuilder();
		sb.Append("Mov. Control: ").Append(movementControl.ToString()).Append("\n");
		sb.Append("Mov. Behaviour: ").Append(movementBehaviour.ToString()).Append("\n");
		sb.Append("Rel. to Gravity?: ").Append(relativeToGravity).Append("\n");
		sb.Append("Max Speed: ").Append(maxSpeed).Append("\n");
		sb.Append("Ground Accel.: ").Append(accelerationOnGround).Append("\n");
		sb.Append("Air Accel.: ").Append(accelerationOnAir).Append("\n");
		sb.Append("Slope Stickiness: ").Append(slopeStickiness).Append("\n");
		sb.Append("Wall Slide Angle: ").Append(maxWallSlideAngle).Append("\n");
		sb.Append("Slide Drag Fact.: ").Append(slidingDragFactor).Append("\n");
		sb.Append("Angle Thereshold: ").Append(angleThereshold).Append("\n");
		sb.Append("Jump Bheaviour: ").Append(jumpBehaviour.ToString()).Append("\n");
		sb.Append("Jump Frequency: ").Append(jumpFrequency).Append("\n");
		sb.Append("Jump Magnitude: ").Append(jumpMagnitude).Append("\n");
		sb.Append("WallJ. Height: ").Append(wallJumpHeight).Append("\n");
		sb.Append("WallJ. Distance: ").Append(wallJumpDistance).Append("\n");
		sb.Append("Base Mass: ").Append(baseMass).Append("\n");
		sb.Append("Custom Gravity?: ").Append(useCustomGravity).Append("\n");
		sb.Append("Custom Gravity: ").Append(customGravity).Append("\n");
		sb.Append("Max Velocity: (")
			.Append(maxVelocity.x.ToString("E2")).Append(", ")
			.Append(maxVelocity.y.ToString("E2")).Append(", ")
			.Append(maxVelocity.z.ToString("E2")).Append(")\n");
		sb.Append("Z Clamp?: ").Append(zClamp);
		return sb.ToString();
	}

	#endregion

	#region Commonly Used Parameters

	/// <summary>
	/// Parameters used while growing. It can't move nor jump and will stop.
	/// </summary>
	public static readonly CharacterControllerParameters GrowingParameters = new CharacterControllerParameters() {
		movementBehaviour = MovementBehaviour.CantMove,
		jumpBehaviour = JumpBehaviour.CantJump
	};

	/// <summary>
	/// Parameters used while shooting. It can't move nor jump and will suddenly stop.
	/// </summary>
	public static readonly CharacterControllerParameters ShootingParameters = new CharacterControllerParameters() {
		movementBehaviour = MovementBehaviour.CantMove,
		jumpBehaviour = JumpBehaviour.CantJump,
		accelerationOnGround = float.MaxValue
	};

	/// <summary>
	/// Parameters used while flying through the air. It can't move nor jump and will not stop.
	/// </summary>
	public static readonly CharacterControllerParameters FlyingParameters = new CharacterControllerParameters() {
		movementBehaviour = MovementBehaviour.CanMoveOnGround,
		jumpBehaviour = JumpBehaviour.CantJump,
		accelerationOnAir = 0
	};

    #endregion
}