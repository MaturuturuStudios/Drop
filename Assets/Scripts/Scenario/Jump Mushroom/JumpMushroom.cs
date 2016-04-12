using UnityEngine;

/// <summary>
/// Objects will bounce on this object, keeping their speed in
/// the oposite direction this object is facing.
/// </summary>
	public class JumpMushroom : MonoBehaviour{

	#region Public Attributes

	/// <summary>
	/// The min height an object will reach after bouncing.
	/// </summary>
	public float minHeight = 1.0f;

	/// <summary>
	/// The max height an object will reach after bouncing.
	/// </summary>
	public float maxHeight = 10.0f;

	/// <summary>
	/// Compensation factor to avoid loose of velocity.
	/// </summary>
	public float jumpFactor = 1.015f;

	/// <summary>
	/// If disabled, objects will be send flying in the same
	/// direction this object is facin, ignoreing their perpendicular
	/// velocity
	/// </summary>
	public bool keepPerpendicularSpeed = false;

	/// <summary>
	/// If enabled, the player won't be able to control a bouncing
	/// character until it touches another collider.
	/// </summary>
	public bool lostControl = true;

	/// <summary>
	/// If enabled with Lost Control, the player will retake control
	/// after a short amount of time.
	/// </summary>
	public bool temporary = true;

	/// <summary>
	/// Time until the player retakes control of a bouncing character.
	/// </summary>
	public float time = 0.1f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the object's Transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of the first frame this
	/// object is active.
	/// </summary>
	void Start() {
		_transform = transform;
	}
	
	/// <summary>
	/// Handles collisions with rigidbodys.
	/// </summary>
	/// <param name="other">Information about the collision</param>
	void OnCollisionEnter(Collision other) {
		if (!enabled)
			return;

		Rigidbody rb = other.rigidbody;
		if (rb != null) {
			// Determines the speed of the object on the facing direction
			float speed = Vector3.Project(other.relativeVelocity, _transform.up).magnitude;

			// Calculates the speed needed in order to reach the max and min heights
			float minheightvelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * minHeight);
			float maxheightvelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxHeight);

			// Clamps the speed
			if (speed < minheightvelocity)
				speed = minheightvelocity;
			if (speed > maxheightvelocity)
				speed = maxheightvelocity;

			// Reassigns the speed in the right direction
			Vector3 velocity = _transform.up * speed * jumpFactor;
			if (keepPerpendicularSpeed)
				velocity += Vector3.Project(other.relativeVelocity, _transform.right);
			rb.velocity = velocity;
		}
	}

	/// <summary>
	/// HAndles collisions with characters.
	/// </summary>
	/// <param name="hit">Information about the collision</param>
	void OnCustomControllerCollision(ControllerColliderHit hit) {
		if (!enabled)
			return;

		CharacterControllerCustom ccc = hit.controller.GetComponent<CharacterControllerCustom>();
		if (ccc != null) {
			// Determines the speed of the object on the facing direction
			float speed = Vector3.Project(ccc.BeforeCollisionVelocity, _transform.up).magnitude;

			// Calculates the speed needed in order to reach the max and min heights
			float minheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * minHeight);
			float maxheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * maxHeight);

			// Clamps the speed
			if (speed < minheightvelocity)
				speed = minheightvelocity;
			if (speed > maxheightvelocity)
				speed = maxheightvelocity;

			// Reassigns the speed in the right direction
			Vector3 velocity = _transform.up * speed * jumpFactor;
			if (keepPerpendicularSpeed)
				velocity += Vector3.Project(ccc.BeforeCollisionVelocity, _transform.right);

			// Checks if the player will lose control of the character
			if (lostControl) {
				if (!temporary)
					ccc.SendFlying(velocity);
				else
					ccc.SendFlying(velocity, false, true, time);
			}
			else {
				ccc.SetForce(velocity);
			}
		}
	}

	/// <summary>
	/// Draws the area where the objects will remain bouncing
	/// on the editor.
	/// </summary>
	void OnDrawGizmos() {
		float height = maxHeight - minHeight;
		Color color = Color.yellow;
		color.a = 0.25f;

		// Draws the box
		Gizmos.color = color;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0, minHeight + height / 2, 0f), new Vector3(GetComponent<BoxCollider>().size.x, height, 0.5f));
	}

	#endregion
}