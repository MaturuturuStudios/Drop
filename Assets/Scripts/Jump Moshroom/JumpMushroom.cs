using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour{
   
    private Vector3 FacingDirection;

    private Vector3 velo;

    public float minJump = 1;
    public float maxJump = 2;
    public float Jumpforce = 10;

    public bool KeepVerticalSpeed = true;
    public bool lostcontrol = true;
    public bool temporaly;
    public float time;
    

    // Use this for initialization
    void Start() {
        
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
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
        CharacterControllerCustom parameters = other.gameObject.GetComponent<CharacterControllerCustom>();

        if ((cccp != null)) {
            if ((cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsGrounded == false) || (cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsFalling == true)){
                //iff(other.gameObject.GetComponent<CharacterControllerCustomPlayer>().isgrounded==false)

                cccp.gameObject.GetComponent<CharacterControllerCustom>().StopFlying();

               velo.y = cccp.GetComponent<CharacterControllerCustom>().Velocity.y;//getvertical
               velo.y = velo.y * -1;

                //Debug.Log("LOOOOOOOOOOOOOOOOOOOOL " + velo.y);

                if (velo.y < minJump)
                    velo.y = minJump;

                if (velo.y > maxJump)
                    velo.y = maxJump;

                if (KeepVerticalSpeed== false)
                     cccp.GetComponent<CharacterControllerCustom>().SetVerticalForce(0);

                //cccp.GetComponent<CharacterControllerCustom>().AddForceRelative(FacingDirection*Jumpforce, ForceMode.VelocityChange);
                //Vector3 finalVelocity = Quaternion.Euler(0, FacingDirection.y, 0) * new Vector3(0, velocity.y, 0);

                if (lostcontrol == true) {
                    if(time==0.0f)
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velo.y * Jumpforce));
                    else
                        cccp.GetComponent<CharacterControllerCustom>().SendFlying((FacingDirection * velo.y * Jumpforce), false, true, time);
                }
                if (lostcontrol == false) {
                    cccp.GetComponent<CharacterControllerCustom>().AddForce(FacingDirection * velo.y * Jumpforce, ForceMode.VelocityChange);
                    //parameters.Parameters = CharacterControllerParameters.FlyingParameters;
                }
            }
        }
    }
    public void OnTriggerExit(Collider other) {
    }

    }
