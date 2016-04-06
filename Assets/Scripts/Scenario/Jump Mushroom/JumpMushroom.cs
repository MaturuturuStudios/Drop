using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour{
    
    public float minheight = 1.0f;
    public float maxheight = 10.0f;
	
    public bool lostcontrol = true;
    public bool temporaly = true;
    public float time = 0.1f;
	
    public void OnTriggerEnter(Collider other) {

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null) {
            float speed = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxheight);
			rb.AddForce(transform.up * speed, ForceMode.VelocityChange);            
        }
		
        CharacterControllerCustom ccc = other.gameObject.GetComponent<CharacterControllerCustom>();
        if (ccc != null) {
			float speed = Vector3.Project(ccc.State.BeforeCollisionsVelocity, transform.up).magnitude;
			Debug.Log(speed);

			float height = (speed * speed) / (2 * ccc.Parameters.Gravity.magnitude);

			if (height < minheight)
				height = minheight;
			if (height > maxheight)
				height = maxheight;

			speed = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * height);
			Vector3 velocity = transform.up * speed;

			if (lostcontrol == true) {
                if(time == 0.0f)
                    ccc.SendFlying(velocity);
                else
                    ccc.SendFlying(velocity, false, true, time);
            }
            else {
                ccc.AddForce(velocity, ForceMode.VelocityChange);
            }
        }
    }

    public void OnDrawGizmos() {
		BoxCollider collider = GetComponent<BoxCollider>();
        float height = maxheight - minheight;
		Color color = Color.yellow;
		color.a = 0.25f;

		// Draws the box
		Gizmos.color = color;
		Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(0, minheight + height / 2, 0f), new Vector3(collider.size.x, height, 0.5f));
    }
}