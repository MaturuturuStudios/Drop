using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class CharacterControllerParameters {

	// Custom enumerations
	public enum JumpBehaviour {
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump
	}

	// Public attributes
	[Range(0, 90)]
	public float slopeLimit = 45;
	public Vector3 maxVelocity = new Vector3(float.MaxValue, float.MaxValue, 0);
	public Vector3 gravity = new Vector3(0, -25, 0);
	public JumpBehaviour jumpBehaviour = JumpBehaviour.CanJumpOnGround;
	public float jumpFrecuency = 0.25f;
}
