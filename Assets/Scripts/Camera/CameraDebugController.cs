using UnityEngine;
using System.Collections;

public class CameraDebugController : MonoBehaviour {

    //Camera status
    public enum PhantomMovement {NONE = -1, LEFT, RIGHT, UP, DOWN, NEAR, FAR};

    //Camera movement attributes
    public float debugVelocity = 0.5f;

    //Offset Reference
    public Vector3 _offset = new Vector3(0.0f, 0.0f, -10.0f);

    //Debug objective
    public GameObject debugObjective;

    //Movement position control
    private Vector3 _lastPositionMovement;
   

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Create phantom objective
        debugObjective = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugObjective.name = "PhantomDrop";

        //Delete phantom objective colliders
        Destroy(debugObjective.GetComponent<Collider>());

        //paint phantom objective
        Color color = Color.white;
        color.a = 0.1f;
        Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = color;
        debugObjective.GetComponent<Renderer>().material = material;

        //Put phantom objective in its position
        debugObjective.transform.position = debugObjective.transform.position;
        debugObjective.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        //Set drop to its position
        transform.position = _offset;

        //Save references
        _lastPositionMovement = transform.position;
    }

    /// <summary>
    /// Update the camera atributes
    /// </summary>
    void LateUpdate()
    {
        //Camera Movement
        transform.position = debugObjective.transform.position + _offset;

        //LookAt player
        transform.LookAt(debugObjective.transform);
    }

    /// <summary>
    /// Set the new position of the phantom
    /// </summary>
    public void SetPhantomPosition(Vector3 newPosition)
    {
        debugObjective.transform.position = newPosition;
    }

    /// <summary>
    /// Set the new position of the phantom
    /// </summary>
    public void SetPhantomMovement(Vector3 movement)
    {
        Vector3 nextPosition = movement;

        debugObjective.transform.position += nextPosition;
    }
}
