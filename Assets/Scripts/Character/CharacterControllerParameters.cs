using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class CharacterControllerParameters {

	// Custom enumerations
	public enum MovementFreedom {
		Horizontal,
		Vertical,
		Both
	}

	public enum MovementBehaviour {
		CanMoveAnywhere,
		CantMoveSliding,
		CanMoveOnGround,
		CantMove
	}

	public enum JumpBehaviour {
		CanJumpAnywhere,
        CanJumpOnGround,
		CantJump
	}

	// Public attributes
	public float mass = 1;
	public MovementFreedom movementFreedom = MovementFreedom.Horizontal;
	public bool relativeToGravity = false;
	public MovementBehaviour movementBehaviour = MovementBehaviour.CantMoveSliding;
	public float maxSpeed = 8;
	public float accelerationOnGround = 10;
	public float accelerationOnAir = 5;
	public JumpBehaviour jumpBehaviour = JumpBehaviour.CanJumpOnGround;
	public float jumpFrecuency = 0.25f;
	public float jumpMagnitude = 1;
	public Vector3 gravity = new Vector3(0, -25, 0);
	public Vector3 maxVelocity = new Vector3(float.MaxValue, float.MaxValue, 0);
}
