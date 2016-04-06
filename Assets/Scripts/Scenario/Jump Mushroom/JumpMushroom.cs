using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour{
    
    public float minheight = 1.0f;
    public float maxheight = 10.0f;
    private float minheightvelocity;
    private float maxheightvelocity;

    public bool lostcontrol = true;
    public bool temporaly = true;
    public bool KeepVerticalSpeed = false;
    public float time = 0.1f;
    
    private float speed;
	
    public void OnTriggerEnter(Collider other) {

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null) {
            float speed = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxheight);
			rb.AddForce(transform.up * speed, ForceMode.VelocityChange);            
        }
		
        CharacterControllerCustom ccc = other.gameObject.GetComponent<CharacterControllerCustom>();
        if (ccc != null) {
            speed = Vector3.Project(ccc.State.BeforeCollisionsVelocity, transform.up).magnitude;
            

            //float height = (speed * speed) / (2 * ccc.Parameters.Gravity.magnitude);
            //speed = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * height);

            Debug.Log(" speed "+speed);

            minheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * minheight);
            maxheightvelocity = Mathf.Sqrt(2 * ccc.Parameters.Gravity.magnitude * maxheight);

            Debug.Log( " min "+minheightvelocity);
            Debug.Log(" max "+maxheightvelocity);

            if (speed < minheightvelocity)
				speed = minheightvelocity;
			if (speed > maxheightvelocity)
				speed = maxheightvelocity;

            if (KeepVerticalSpeed == true)
            {
                speed += Vector3.Magnitude(Vector3.Project(ccc.Velocity, transform.right));

            }
  
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

    public void OnTriggerExit()
    {

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