using UnityEngine;
using System.Collections.Generic;

public class WaterRepulsion : MonoBehaviour {
    #region Public attributes
    public Transform pointExpulsion;
    public Transform direction;
    public float impulse=15;
    #endregion

    #region Private attributes
    private Vector3 _velocity;
    private List<GameObject> _enteredDrop;
    private Bounds _ownCollider;
    private Vector3 positionExpulsion;
    private Vector3 positionDirection;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        //get the collider
        _ownCollider = GetComponent<Collider>().bounds;
        //create list
        _enteredDrop = new List<GameObject>();

        //get the setted positions
        positionDirection = direction.position;
        positionDirection.z = 0;
        positionExpulsion = pointExpulsion.position;
        positionExpulsion.z = 0;

        //set the velocity the drop will be send flying
        _velocity = (positionDirection - positionExpulsion).normalized;
        _velocity *= impulse;
    }

    public void Update() {
        //no drop? get out
        if (_enteredDrop.Count == 0) return;

        //for every drop in water...
        foreach(GameObject drop in _enteredDrop) {
            //get position and bounds
            Vector3 position = drop.transform.position;
            float halfSize = drop.GetComponent<CharacterSize>().GetSize() * 0.5f;
            //get four direction of drop
            Vector3[] vertices = new Vector3[4];
            vertices[0] = position;
            vertices[0].x -= halfSize;
            vertices[1] = position;
            vertices[1].x += halfSize;
            vertices[2] = position;
            vertices[2].y -= halfSize;
            vertices[3] = position;
            vertices[3].y += halfSize;


            //check if al points are inside the water
            bool result = true;
            for(int i=0; i<vertices.Length && result; i++) {
                result = _ownCollider.Contains(vertices[i]);
            }

            //is inside? get the drop out!
            if (result) {
                CharacterControllerCustom controller = drop.GetComponent<CharacterControllerCustom>();
                //put drop on point expulsion
                controller.Stop();
                drop.transform.position = positionExpulsion;
                //send it flying
                controller.SendFlying(_velocity);
            }
        }
    }

    public void OnTriggerEnter(Collider other) {
        //get the component if is a drop
        GameObject drop = other.gameObject;
        if (drop.tag != Tags.Player) return;
        _enteredDrop.Add(drop);
    }

    public void OnTriggerExit(Collider other) {
        //get the component if is a drop
        GameObject drop = other.gameObject;
        if (drop.tag != Tags.Player) return;
        _enteredDrop.Remove(drop);
    }
    #endregion
}
