using UnityEngine;
using System.Collections;

public class Character_Jump : MonoBehaviour
{
    // Public variables
    public float max_jump = 10;

    // References
    private Rigidbody _rigidbody_component;
    private Character_Size _size_component;
    private Collider _collider;


    void Start()
    {
        _collider = GetComponent<Collider>();
        _size_component = GetComponent<Character_Size>();
        _rigidbody_component = GetComponent<Rigidbody>();
    }

    public bool Jump()
    {
        bool _grounded = Grounded();

        if (_grounded)
        {
            ExecuteJump();
        }

        return _grounded;
    }


    public bool Grounded()
    {

        Ray ray = new Ray();
        ray.origin = _collider.bounds.center;
        ray.direction = Vector3.down;

        //Look for collision bellow
        return Physics.Raycast(ray, _collider.bounds.extents.y + 0.1f, 1 << 8);
    }


    private void ExecuteJump()
    {
        // Calculates the speed
        float speed = max_jump * _size_component.GetSize();

        // Push drop up
        _rigidbody_component.velocity = new Vector3(_rigidbody_component.velocity.x, speed, _rigidbody_component.velocity.z);
    }

}