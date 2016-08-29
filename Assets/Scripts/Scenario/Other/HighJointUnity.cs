using UnityEngine;
using System.Collections;

public class HighJointUnity : MonoBehaviour {

    public float rigbodyMassIncrease=10;

    public float rigbodyMass=100;

    private float _rigbody;

    private bool _onEnter = false;
    private bool _onExit = false;

    // Use this for initialization
    void Start () {
        rigbodyMass = transform.parent.GetComponent<Rigidbody>().mass;
        _rigbody = rigbodyMassIncrease;
	}


    public void OnTriggerEnter(Collider other)
    {
        CharacterControllerCustom cccp = other.gameObject.GetComponent<CharacterControllerCustom>();

        if (cccp != null)
        {
            if (cccp.GetTotalMass() > 2) { 
                 transform.parent.GetComponent<Rigidbody>().mass = rigbodyMass - (cccp.GetTotalMass() * rigbodyMassIncrease);
             }
            Debug.Log(" MASAAAAAAAA " + transform.parent.GetComponent<Rigidbody>().mass);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        //transform.eulerAngles = _initialPosition;
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();

        if (cccp != null)
        {
            transform.parent.GetComponent<Rigidbody>().mass = rigbodyMass;
        }
    }
}
