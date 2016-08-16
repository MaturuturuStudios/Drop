using UnityEngine;
using System.Collections;

public class HighJoint : MonoBehaviour
{

    public Vector3 destinationAngle;

    private GameObject _character;

    private bool _colisionDetected = false;

    private float _angle = 0;

    private Vector3 _initialPosition;

    private Vector3 _incrementAngle;
    // Use this for initialization
    void Start()
    {

        _initialPosition = transform.GetComponentInChildren<Collider>().transform.eulerAngles;
        _angle = destinationAngle.z;

        destinationAngle = new Vector3(45, 0, 0);
        _incrementAngle = new Vector3(25, 180, 0);

        Debug.Log("  no es nulo" + transform.eulerAngles);
    }


    // Update is called once per frame
    void Update()
    {
     
        if (_colisionDetected){

            transform.GetComponentInChildren<Collider>().transform.eulerAngles = Vector3.Lerp(transform.GetComponentInChildren<Collider>().transform.eulerAngles,
                                             _initialPosition,
                                             Time.deltaTime);
        }

    }

    public void OnTriggerEnter(Collider other){
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
        
        if (cccp != null)
        {
            Debug.Log("  no es nulo" + transform.eulerAngles);
            transform.GetComponentInChildren<Collider>().transform.eulerAngles = _incrementAngle;

        }

    }
    public void OnTriggerStay(Collider other){
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
        
        if (cccp != null)
        {
            _colisionDetected = false;

        }
    }

    public void OnTriggerExit(Collider other){
        //transform.eulerAngles = _initialPosition;
        _colisionDetected = true;
    }

}
