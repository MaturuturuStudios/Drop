using UnityEngine;
using System.Collections.Generic;

public class CameraTravelController : MonoBehaviour
{
    // Internal references
    private CameraSwitcher _cameraSwitcher;
    private Camera _mainCamera;

    //Drop References
    private Objective currentObjective;

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
        public Movement movement = new Movement(2.0f, 1.0f, 2.5f);
        public Vector3 position = new Vector3(3.5f, 4.5f, 0F);
        public Vector3 lookAtPosition = new Vector3(0F, 0F, 0F);
        public float waitTime = 2;
    }

    /// <summary>
    /// Camera reference position with player
    /// </summary>
    [System.Serializable]
    public class Boundary
    {
        //set the boundary visible
        public bool visible = true;
        //boundary attributes
        public float width = 5.0f;
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

        public Movement(float smooth, float zSmooth, float lookAtSmooth)
        {
            this.smooth = smooth;
            this.zSmooth = zSmooth;
            this.lookAtSmooth = lookAtSmooth;
        }
    }

    //Movement position control
    private Vector3 _lastPositionMovement;
    //Look at position control
    private Vector3 _lastObjective;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        _cameraSwitcher = GameObject.FindGameObjectWithTag("CameraSet")
                                .GetComponent<CameraSwitcher>();

        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera")
                                .GetComponent<Camera>();

        currentObjective = objectivesList.Objectives[0];
        //Set drop to its position
        transform.position = objectivesList.startPosition;

        //Set references
        _lastObjective = currentObjective.lookAtPosition;
        _lastPositionMovement = objectivesList.startPosition;
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

    float waitTime = 0F;
    /// <summary>
    /// Actualize the camera status
    /// </summary>
    private void ActualizeState()
    {
        //look for change to the next drop
        Vector3 diff = currentObjective.position - transform.position;

        if (diff.magnitude < 1F)
            waitTime += Time.deltaTime;
        else
            waitTime = 0F;

        if (waitTime > currentObjective.waitTime)
        {
            int index = objectivesList.Objectives.IndexOf(currentObjective);
            if (++index == objectivesList.Objectives.Count) { 
                index = 0;
                _mainCamera.transform.position = transform.position;
                _mainCamera.transform.rotation = transform.rotation;
                _cameraSwitcher.SetActiveCamera(1);
            }
            currentObjective = objectivesList.Objectives[index];
        }
    }

    Vector3 objMov;
    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera()
    {
        Vector3 destination = currentObjective.position;

        //Need to use something better than size
        objMov = Vector2.Lerp(transform.position, destination, Time.deltaTime * currentObjective.movement.smooth);
        objMov.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * currentObjective.movement.zSmooth);

        transform.position = objMov;

        //Save the last position
        _lastPositionMovement = objMov;
    }

    /// <summary>
    /// Makes the camera look to the player's position gradually
    /// </summary>
    private void LookAt()
    {
        Quaternion lookingAt = transform.rotation;

        Vector3 destination = currentObjective.lookAtPosition;

        destination = Vector3.Lerp(_lastObjective, destination, Time.deltaTime * currentObjective.movement.lookAtSmooth);

        transform.LookAt(destination);

        _lastObjective = destination;
    }
}
