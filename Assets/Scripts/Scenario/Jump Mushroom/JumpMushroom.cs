using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour{
   
    private Vector3 FacingDirection;

    private Vector3 velo;
    
    public float minheight=1;
    public float maxheight=20;
    public float height=10;
    public float Jumpforce=2;

    public bool KeepVerticalSpeed = true;
    public bool lostcontrol = true;
    public bool temporaly=true;
    public float time=0.1f;
    private float velocidad;

    

    public CharacterControllerParameters parameters;

    // Use this for initialization
    public void Start() {
        
        FacingDirection = Vector3.up;
        velo.x = 0;
        velo.z = 0;
 
    }

    // Update is called once per frame
    void Update() {
        //FacingDirection = new Vector3(transform.position.x, transform.position.y, 0).normalized;
        FacingDirection = (transform.rotation * FacingDirection);

    }
    public void OnTriggerEnter(Collider other) {

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null) {
            velocidad = Mathf.Sqrt(2 * rb.velocity.magnitude * height);
            rb.AddForce((FacingDirection * velocidad * Jumpforce), ForceMode.VelocityChange);
            
        }

        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
       // CharacterControllerCustom state = other.gameObject.GetComponent<CharacterControllerCustom>();
       // CharacterControllerCustom  parameters = other.gameObject.GetComponent<CharacterControllerCustom>();

        if ((cccp != null)) {
            if ((cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsGrounded == false) || (cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsFalling == true)){
                //iff(other.gameObject.GetComponent<CharacterControllerCustomPlayer>().isgrounded==false)

                cccp.gameObject.GetComponent<CharacterControllerCustom>().StopFlying();

               velo.y = cccp.GetComponent<CharacterControllerCustom>().Velocity.y;//getvertical
               velo.y = velo.y * -1;

                if (height < minheight) height = minheight;
                if (height > maxheight) height = maxheight;

                if (KeepVerticalSpeed == false) {
                    
                     velocidad = Mathf.Sqrt(2 * parameters.Gravity.magnitude* height);
                }
                if (KeepVerticalSpeed == true)
                {

                    velocidad = Mathf.Sqrt(2 * velo.y * height);
                }
                

                if (lostcontrol == true) {
                    if(time==0.0f)
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velocidad * Jumpforce));
                    else
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velocidad * Jumpforce), false, true, time);
                }
                if (lostcontrol == false) {
                    cccp.GetComponent<CharacterControllerCustom>().AddForce(FacingDirection * velocidad * Jumpforce, ForceMode.VelocityChange);
                    //parameters.Parameters = CharacterControllerParameters.FlyingParameters;
                }
            }
        }
    }
    public void OnTriggerExit(Collider other) {
    }

    }
