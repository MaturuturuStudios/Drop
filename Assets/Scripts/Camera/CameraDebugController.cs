using UnityEngine;
using System.Collections;

public class CameraDebugController : MonoBehaviour {

    //Camera status
    public enum PhantomMovement {NONE = -1, LEFT, RIGHT, UP, DOWN, NEAR, FAR};

    //Camera movement attributes
    public float debugVelocity = 0.5f;

    //Movement position control
    private Vector3 _lastPositionMovement;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Set camera to start position //maybe at player + offset position
        transform.position = new Vector3(0.0f, 0.0f, -10.0f);
    }

    /// <summary>
    /// Set the new position of the phantom
    /// </summary>
    public void SetMovement(Vector3 movement)
    {

        Vector3 nextPosition = transform.rotation * movement;

        transform.position += nextPosition;
    }

    /// <summary>
    /// Set the new position of the phantom
    /// </summary>
    public void SetLookAt (float mouseAxisX, float mouseAxisY)
    {
        yaw += debugVelocity * Input.GetAxis("Mouse X");
        pitch -= debugVelocity * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
