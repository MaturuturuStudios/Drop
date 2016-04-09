using UnityEngine;

public class JumpMushroom : MonoBehaviour{

	public BoxCollider jumpCollider;

	public float minHeight = 1.0f;
	public float maxHeight = 10.0f;

	public float jumpFactor = 1.015f;

	public bool keepPerpendicularSpeed = false;
	public bool lostControl = true;
	public bool temporaly = true;
	public float time = 0.1f;

	private Transform _transform;

	void Start() {
		_transform = transform;
	}
	
	void OnTriggerEnter(Collider other) {
		Rigidbody rb = other.attachedRigidbody;
		if (rb != null) {
			float speed = Vector3.Project(rb.velocity, transform.up).magnitude;

			float minheightvelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * minHeight);
			float maxheightvelocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxHeight);

			if (speed < minheightvelocity)
				speed = minheightvelocity;
			if (speed > maxheightvelocity)
				speed = maxheightvelocity;

			Vector3 velocity = transform.up * speed;
			if (keepPerpendicularSpeed)
				velocity += Vector3.Project(rb.velocity, transform.right);
			rb.velocity = velocity;
		}
	}

	void OnCustomControllerCollision(ControllerColliderHit hit) {
		if (hit.collider != jumpCollider)
			return;

		CharacterControllerCustom ccc = hit.controller.GetComponent<CharacterControllerCustom>();
		if (ccc != null) {
			float speed = Vector3.Project(ccc.BeforeCollisionVelocity, transform.up).magnitude;

			float minheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * minHeight);
			float maxheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * maxHeight);

			if (speed < minheightvelocity)
				speed = minheightvelocity;
			if (speed > maxheightvelocity)
				speed = maxheightvelocity;

			Vector3 velocity = transform.up * speed * jumpFactor;
			if (keepPerpendicularSpeed)
				velocity += Vector3.Project(ccc.BeforeCollisionVelocity, transform.right);

			if (lostControl) {
				if (!temporaly)
					ccc.SendFlying(velocity);
				else
					ccc.SendFlying(velocity, false, true, time);
			}
			else {
				ccc.SetForce(velocity);
			}
		}
	}

	void OnDrawGizmos() {
		float height = maxHeight - minHeight;
		Color color = Color.yellow;
		color.a = 0.25f;

		// Draws the box
		Gizmos.color = color;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0, minHeight + height / 2, 0f), new Vector3(jumpCollider.size.x, height, 0.5f));
	}
}