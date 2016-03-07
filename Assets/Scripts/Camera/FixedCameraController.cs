using UnityEngine;
using System.Collections;

public class FixedCameraController : MonoBehaviour {
    
    //Drop References
    private GameObject currentCharacter;
    
    //Look at position control
    public Vector3 startPosition;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Set camera< to its position
        transform.position = startPosition;
    }

    /// <summary>
    /// Update the camera atributes
    /// </summary>
    void LateUpdate()
    {
        //LookAt player
        transform.LookAt(currentCharacter.transform.position);
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
