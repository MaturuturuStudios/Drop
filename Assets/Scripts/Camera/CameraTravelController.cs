using UnityEngine;
using System.Collections.Generic;

public class CameraTravelController : MonoBehaviour {

    //Drop References
    public GameObject currentCharacter;
    private GameObject _lastCharacter;

    //Camera status
    enum CameraTravelState { START, PLAY, PAUSE, STOP };
    private CameraTravelState _travelState = CameraTravelState.START;


    /// <summary>
    /// Objective of the camera
    /// </summary>
    [System.Serializable]
    public class ListOfObjective
    {
        public Vector3 startPosition;
        public List<Objective> Objectives;
    }
    public ListOfObjective objectivesList;

    /// <summary>
    /// Objective of the camera
    /// </summary>
    [System.Serializable]
    public class Objective
    {
        public Movement movement = new Movement(3.5f, 4.5f, 0.0f);
        public Vector3 position = new Vector3(3.5f, 4.5f, 0.0f);
        public Vector3 _offset = new Vector3(0.0f, 4.5f, -15.0f);
        public int size = 1;
    }

    /// <summary>
    /// Distance from player to camera
    /// </summary>
    [System.Serializable]
    public class Offset
    {
        public float far = -15.0f;
        public float up = 2.0f;
    }
    //Offset Attributes
    public Offset offset;
    //Offset Reference
    private Vector3 _offset;

    /// <summary>
    /// Camera reference position with player
    /// </summary>
    [System.Serializable]
    public class Boundary
    {
        //set the boundary visible
        public bool visible = true;
        //boundary attributes
        public float width = 3.0f;
    }
    //Boudary Attributes
    public Boundary boundary;
    private GameObject _cameraBoundary;

    /// <summary>
    /// Camera options
    /// </summary>
    [System.Serializable]
    public class Movement
    {
        public float smooth = 2.0f;
        public float zSmooth = 1.0f;
        public float lookAtSmooth = 3.0f;

        public Movement(float smooth, float zSmooth, float lookAtSmooth) {
            this.smooth = smooth;
            this.zSmooth = zSmooth;
            this.lookAtSmooth = lookAtSmooth;
        }
    }
    public Movement movement;
    //Movement position control
    private Vector3 _lastPositionMovement;
    //Look at position control
    private Vector3 _lastObjective;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Calculate offset
        _offset = new Vector3(0.0f, offset.up, offset.far);
        //Set drop to its position
        transform.position = currentCharacter.transform.position + new Vector3(-4.0f, 25.0f, -55.0f);

        //Set references
        _lastObjective = currentCharacter.transform.position;
        _lastPositionMovement = transform.position;
        _lastCharacter = currentCharacter;

        //Activate boundary
        if (boundary.visible)
        {
            //Create new boundary
            _cameraBoundary = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _cameraBoundary.name = "CameraBoundary";

            //Delete colliders
            Destroy(_cameraBoundary.GetComponent<Collider>());

            //paint it
            Color color = Color.blue;
            color.a = 0.1f;
            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            material.color = color;
            _cameraBoundary.GetComponent<Renderer>().material = material;

            //Put it in its position
            _cameraBoundary.transform.position = currentCharacter.transform.position;
            _cameraBoundary.transform.localScale = new Vector3(boundary.width, boundary.width, 0.1f);
        }
    }

    /// <summary>
    /// Update the state of the camera
    /// </summary>
    void Update()
    {
        // Actualize status
        ActualizeState();
    }

    /// <summary>
    /// Update the camera atributes
    /// </summary>
    void LateUpdate()
    {
        //Camera Movement
        MoveCamera();

        //LookAt player
        LookAt();
    }

    /// <summary>
    /// Actualize the camera status
    /// </summary>
    private void ActualizeState()
    {
        //Get drop size
        float size = currentCharacter.GetComponent<CharacterSize>().GetSize();

        //Actualize ofset and boundary depends of the size
        _offset = new Vector3(0.0f, offset.up + size, offset.far - (size * 5));

        if (boundary.visible)
        {
            _cameraBoundary.SetActive(true);
            _cameraBoundary.transform.localScale = new Vector3(boundary.width * size, boundary.width * size, 0.1f);
        }
        else
            _cameraBoundary.SetActive(false);

        _lastCharacter = currentCharacter;
    }

    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera()
    {
        Vector3 destination = currentCharacter.transform.position + _offset;

        //Need to use something better than size
        Vector3 objMov = Vector2.Lerp(transform.position, destination, Time.deltaTime * movement.smooth);
        objMov.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * movement.zSmooth);

        if (boundary.visible)
            _cameraBoundary.transform.position = objMov - _offset;

        transform.position = objMov;

        //Save the last position
        _lastPositionMovement = objMov;
    }

    /// <summary>
    /// Makes the camera look to the player's position gradually
    /// </summary>
    private void LookAt()
    {
        Vector3 destination = currentCharacter.transform.position;

        destination = Vector3.Lerp(_lastObjective, destination, Time.deltaTime * movement.lookAtSmooth);

        //Set position lookAt to camera
        transform.LookAt(destination);

        _lastObjective = destination;
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
