using UnityEngine;
using System.Collections;

public class JumpMushroom : MonoBehaviour
{
   
    public Vector3 upforce;
    // Use this for initialization
    void Start()
    {
        upforce.x = 0;
        upforce.y = 50;
        upforce.z = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();

        if ((cccp !=null) && (cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsGrounded==false) || (cccp.gameObject.GetComponent<CharacterControllerCustom>().State.IsFalling == true)){
            //iff(other.gameObject.GetComponent<CharacterControllerCustomPlayer>().isgrounded==false)
            // CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();     
            float vel = cccp.GetComponent<CharacterControllerCustom>().Velocity.y;
            // other.GetComponent<CharacterControllerCustom>().SetVerticalSpeed(0);
            cccp.GetComponent<CharacterControllerCustom>().AddForce(upforce, ForceMode.VelocityChange);
            
        }
    }

}
