using UnityEngine;
using System.Collections;

public class MainCameraAnimationController : MonoBehaviour {

    #region Public attributes
    
    /// <summary>
    /// Leave to move in the start animation
    /// </summary>
    public GameObject startLeave;


    /// <summary>
    /// Desired start position of dop when intro is skiped
    /// </summary>
    public Vector3 startPosition;

    #endregion

    #region Private attributes
    /// <summary>
    /// Reference to input controller
    /// </summary>
    private GameControllerInput _gci;

    /// <summary>
    /// REference to camera controller
    /// </summary>
    private MainCameraController _mcc;

    /// <summary>
    /// Refrence to camera animator
    /// </summary>
    private Animator _cameraAnimator;

    #endregion

    #region Public methods

    void Start () {

        // Get the references to the required objects
        _gci = GetComponentInParent<GameControllerInput>();
        _mcc = GetComponentInParent<MainCameraController>();
        _cameraAnimator = GetComponent<Animator>();

        // Stops the input
        _gci.ResumeInput();
    } 


    /// <summary>
    /// Stops input called from animator
    /// </summary>
    public void StopInput() {
        _gci.StopInput(true);
    }


    /// <summary>
    /// Resumes input called from animator
    /// </summary>
    public void ResumeInput() {
        _gci.ResumeInput();
    }


    /// <summary>
    /// Sets up the camera controller
    /// </summary>
    public void ResumeCameraController() {

        // Ensure zero rotation of the camera
        Camera.main.transform.localRotation = Quaternion.identity;

        // Disable the animator
        _cameraAnimator.enabled = false;

        // Enable the main camera controller script
        _mcc.enabled = true;
    }


    /// <summary>
    /// Skips the intro and sets all the object to its position
    /// </summary>
    public void SkipIntro() {

        // Move leave to the end 
        startLeave.GetComponent<FollowPath>().delay = 0;
        startLeave.GetComponent<FollowPath>().Set(1);

        // Move drop to the leave
        // GetComponentInParent<GameControllerIndependentControl>().currentCharacter.transform.position = startPosition;

        ResumeInput();
        ResumeCameraController();
    }


    /// <summary>
    /// Used from animator to start leave movement at 5 second to end
    /// </summary>
    public void MoveStartLeave() {
        startLeave.GetComponent<FollowPath>().Next();
    }

    #endregion  
}
