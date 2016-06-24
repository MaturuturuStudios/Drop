using UnityEngine;
using System.Collections;

public class MainCameraAnimationController : MonoBehaviour {

    public GameObject startLeave;
    public GameObject endLeave;
    public Vector3 startPosition;

    private GameControllerInput _gci;
    private MainCameraController _mcc;
    private Animator _cameraAnimator;

    // Use this for initialization
    void Start () {
        _gci = GetComponentInParent<GameControllerInput>();
        _mcc = GetComponent<MainCameraController>();
        _cameraAnimator = GetComponent<Animator>();
        _gci.ResumeInput();
    }

    public void StopInput() {
        _gci.StopInput(true);
    }

    public void ResumeInput() {
        _gci.ResumeInput();
    }

    public void ResumeCameraController() {
        Camera.main.transform.localRotation = Quaternion.identity;
        _cameraAnimator.enabled = false;
        _mcc.enabled = true;
    }

    public void SkipIntro() {
        startLeave.GetComponent<FollowPath>().delay = 0;
        startLeave.GetComponent<FollowPath>().Next();
        GetComponentInParent<GameControllerIndependentControl>().currentCharacter.transform.localPosition = startPosition;

        ResumeInput();
        ResumeCameraController();
    }

    public void MoveStartLeave() {
        startLeave.GetComponent<FollowPath>().Next();
    }

    public void MoveEndLeave() {
        endLeave.GetComponent<FollowPath>().Next();
    }
}
