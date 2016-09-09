using UnityEngine;
using System.Collections;

/// <summary>
/// This class is for Plant Animation when drop is above them
/// </summary>
public class HighJoint : MonoBehaviour{

    #region Public Attributes

    /// <summary>
    /// Vector use to change the rotation angle of the plant
    /// </summary>
    public Vector3 destinationAngle;

    #endregion

    #region Private Attributes

    /// <summary>
    /// Character, Drop
    /// </summary>
    private GameObject _character;

    /// <summary>
    /// Variable to know when the the drop has entered in the plant's trigger
    /// </summary>
    private bool _colisionUpDetected = false;

    /// <summary>
    /// Variable to know when the the drop has left  the plant's trigger
    /// </summary>
    private bool _colisionDownDetected = false;

    /// <summary>
    /// Vector with the initial position of the plant
    /// </summary>
    private Vector3 _initialPosition;

    /// <summary>
    /// Variable to know when the the drop has left  the plant's trigger
    /// </summary>
    private Vector3 _incrementAngle;

    #endregion

    // Use this for initialization
    void Start(){

        _initialPosition = transform.GetComponentInChildren<Collider>().transform.eulerAngles;      

        destinationAngle = new Vector3(30, 0, 0);
        _incrementAngle = new Vector3(30, 180, 0);

    }

	public Transform platform;
	public float speed;

    // Update is called once per frame
    void Update(){

        if (_colisionDownDetected){
			platform.eulerAngles = Vector3.MoveTowards(platform.eulerAngles,
                                             _incrementAngle,
                                             speed * Time.deltaTime);
        }

		if (_colisionUpDetected){
			platform.eulerAngles = Vector3.MoveTowards(platform.eulerAngles,
				_initialPosition,
				speed * Time.deltaTime);
        }

    }

    public void OnTriggerStay(Collider other){
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
        
        if (cccp != null){
            _colisionDownDetected = true;
            _colisionUpDetected = false;
        }
    }

    public void OnTriggerExit(Collider other){
        //transform.eulerAngles = _initialPosition;
        CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();

        if (cccp != null){
            _colisionUpDetected = true;
            _colisionDownDetected = false;
        }
    }

}
