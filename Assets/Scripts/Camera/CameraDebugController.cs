using UnityEngine;
using System.Collections;

public class CameraDebugController : MonoBehaviour {

    //Camera status
    public enum PhantomMovement {NONE = -1, LEFT, RIGHT, UP, DOWN, NEAR, FAR};

    //Drop References
    private GameObject _lastCharacter;

    //Camera movement attributes
    public float debugVelocity = 0.5f;

    //Offset Reference
    public Vector3 _offset = new Vector3(0.0f, 0.0f, -10.0f);

    //DEbu objective
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
        debugObjective.transform.localScale = new Vector3(1.0f, 1.0f, 0.1f);

        //Set drop to its position
        transform.position = _offset;

        //Save references
        _lastPositionMovement = transform.position;
        _lastCharacter = debugObjective;
    }

    /// <summary>
    /// Update the camera atributes
    /// </summary>
    void LateUpdate()
    {
        //Camera Movement
        MoveCamera();

        //LookAt player
        transform.LookAt(debugObjective.transform);
    }

    /// <summary>
    /// Move the camera depending of the status
    /// </summary>
    private void MoveCamera()
    {
        //Set objective movement base & calculate diference from our position to obsective
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = debugObjective.transform.position + _offset - _lastPositionMovement;

        //Actualize X position
        if (diffMovement.x > 0)
        {
            objectiveMovement.x += debugVelocity;
        }
        else if (diffMovement.x < 0)
        {
            objectiveMovement.x -= debugVelocity;
        }

        //Actualize Y position
        if (diffMovement.y > 0)
        {
            objectiveMovement.y += debugVelocity;
        }
        else if (diffMovement.y < 0)
        {
            objectiveMovement.y -= debugVelocity;
        }

        //Actualize Z position
        if (diffMovement.z > 0)
        {
            objectiveMovement.z += debugVelocity;
        }
        else if (diffMovement.z < 0)
        {
            objectiveMovement.z -= debugVelocity;
        }

        //Actualize the position of the camera && the boundary
        debugObjective.transform.position = objectiveMovement - _offset;
        transform.position = objectiveMovement;

        //Save the last position
        _lastPositionMovement = objectiveMovement;
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
    public void SetPhantomMovement(PhantomMovement newMovement)
    {
        Vector3 nextPosition = debugObjective.transform.position;

        if (newMovement == CameraDebugController.PhantomMovement.NEAR)
        {
            nextPosition.z += debugVelocity;
        }
        if (newMovement == CameraDebugController.PhantomMovement.FAR)
        {
            nextPosition.z -= debugVelocity;
        }
        if (newMovement == CameraDebugController.PhantomMovement.RIGHT)
        {
            nextPosition.x += debugVelocity;
        }
        if (newMovement == CameraDebugController.PhantomMovement.LEFT)
        {
            nextPosition.x -= debugVelocity;
        }
        if (newMovement == CameraDebugController.PhantomMovement.UP)
        {
            nextPosition.y += debugVelocity;
        }
        if (newMovement == CameraDebugController.PhantomMovement.DOWN)
        {
            nextPosition.y -= debugVelocity;
        }

        debugObjective.transform.position = nextPosition;
    }
}
