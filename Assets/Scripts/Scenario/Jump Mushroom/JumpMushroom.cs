using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour{
   
    private Vector3 FacingDirection;

    private Vector3 velo;
    
    public float minheight=1;
    public float maxheight=10;
    
    public float Jumpforce=1;

    public bool KeepVerticalSpeed = true;
    public bool lostcontrol = true;
    public bool temporaly=true;
    public float time=0.1f;
    private float velocity,vel;
    private float height;
    private BoxCollider _collider;
    private bool firstiem = false;
    //public float width = 1f;
    public CharacterControllerParameters parameters;

    // Use this for initialization
    public void Awake() {
        
        FacingDirection = Vector3.up;
        
        _collider = gameObject.GetComponent<BoxCollider>();
        velo.x = 0;
        velo.z = 0;
 
    }

    // Update is called once per frame
    void Update() {
        //FacingDirection = new Vector3(transform.position.x, transform.position.y, 0).normalized;
        // FacingDirection = (transform.rotation * FacingDirection);
        FacingDirection = (transform.rotation * FacingDirection);
        // _collider.size = new Vector3(width / 2, maxheight / 2, 0.5f);

        // Sets the center of the collider
        // _collider.center = new Vector3(0, length / 4, 0);

    }
    public void OnTriggerEnter(Collider other) {

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null) {
            velocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * maxheight);
            rb.AddForce((FacingDirection * velocity * Jumpforce), ForceMode.VelocityChange);
            
        }

        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
        CharacterControllerCustom ccc = other.gameObject.GetComponent<CharacterControllerCustom>();
       // CharacterControllerCustom  parameters = other.gameObject.GetComponent<CharacterControllerCustom>();

        if ((cccp != null)) {
            
                //iff(other.gameObject.GetComponent<CharacterControllerCustomPlayer>().isgrounded==false)

 
                velo = cccp.GetComponent<CharacterControllerCustom>().Velocity;
                vel = Vector3.Magnitude(Vector3.Project(velo, transform.up));
                 
               // cccp.gameObject.GetComponent<CharacterControllerCustom>().Stop();

                
                height = (vel * vel) / (2 * parameters.Gravity.magnitude);

                Debug.Log("VELOCITY "+vel);
                


                if (height < minheight) height = minheight;
                if (height > maxheight) height = maxheight;

                
                //velocidad = Mathf.Sqrt(2 * velo.y  * maxheight);  to ricochet

                velocity = Mathf.Sqrt(2 * parameters.Gravity.magnitude  * height);

                if (KeepVerticalSpeed == true)
                {
                    velocity += Vector3.Magnitude(Vector3.Project(ccc.Velocity, transform.right));
                    
                }

                if (lostcontrol == true) {
                    if(time==0.0f)
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velocity * Jumpforce));
                    else
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velocity * Jumpforce), false, true, time);
                }
                if (lostcontrol == false) {
                    cccp.GetComponent<CharacterControllerCustom>().AddForce(FacingDirection * velocity * Jumpforce, ForceMode.VelocityChange);
                    //parameters.Parameters = CharacterControllerParameters.FlyingParameters;
                }
            }
            
        
    }
    public void OnTriggerExit(Collider other) {
        
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