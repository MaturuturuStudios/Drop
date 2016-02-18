﻿using System;
using UnityEngine;
using System.Collections;

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
		/// Will always but when sliding accept input.
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
		/// Will be able to jump while grounded.
		/// </summary>
		CanJumpOnGround,

		/// <summary>
		/// Will not be able to jump.
		/// </summary>
		CantJump
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
	public MovementBehaviour movementBehaviour = MovementBehaviour.CantMoveSliding;

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
	public float accelerationOnAir = 5;

	/// <summary>
	/// Defines in which situations the character will be able to jump.
	/// </summary>
	public JumpBehaviour jumpBehaviour = JumpBehaviour.CanJumpOnGround;

	/// <summary>
	/// Amount of time beetween jumps. The character will not be able to jump after this time period
	/// has passed.
	/// </summary>
	public float jumpFrecuency = 0.25f;

	/// <summary>
	/// Jumping factor. Defines the height that should be achieved at the peek of the jump.
	/// </summary>
	public float jumpMagnitude = 1;

	/// <summary>
	/// The base mass of the character, when it's size is 1. This will affect how much force will the
	/// character apply to other entitys when colliding whith them.
	/// </summary>
	public float baseMass = 1;

	/// <summary>
	/// The gravity affecting the character. Will pull the character on this direction each frame.
	/// </summary>
	public Vector3 gravity = new Vector3(0, -25, 0);

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
		clone.jumpBehaviour = jumpBehaviour;
		clone.jumpFrecuency = jumpFrecuency;
		clone.jumpMagnitude = jumpMagnitude;
		clone.baseMass = baseMass;
		clone.gravity = gravity;
		clone.maxVelocity = maxVelocity;
		clone.zClamp = zClamp;

		return clone;
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
		movementControl = MovementControl.None,
		movementBehaviour = MovementBehaviour.CantMove,
		jumpBehaviour = JumpBehaviour.CantJump,
		accelerationOnAir = 0
	};

	/// <summary>
	/// Parameters used while floating on a wind stream. It can move in any direction but can't jump.
	/// </summary>
	public static readonly CharacterControllerParameters FloatingParameters = new CharacterControllerParameters() {
		movementControl = MovementControl.Both,
		movementBehaviour = MovementBehaviour.CanMoveAnywhere,
		jumpBehaviour = JumpBehaviour.CantJump
	};

	#endregion
}
