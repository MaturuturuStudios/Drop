using System;
using UnityEngine;
using System.Collections;

public class CharacterControllerState {

	// Properties
	public bool HasCollisions { get; set; }
	public bool IsGrounded { get; set; }
	public bool IsSliding { get; set; }
	public GameObject GroundedObject { get; set; }
	public float SlopeAngle { get; set; }

	public void Reset() {
		HasCollisions = false;
		IsGrounded = false;
		IsSliding = false;
		SlopeAngle = 0;
		GroundedObject = null;
	}
}
