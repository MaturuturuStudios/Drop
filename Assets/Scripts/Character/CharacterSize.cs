using UnityEngine;
using System.Collections;

public class CharacterSize : MonoBehaviour {

    public float shrink_speed = 0.4f;
    public float enlarge_speed = 0.4f;

    private float _targetting_size;
    private int _shrink_or_enlarge;
    private int _size;

    private Transform _drop_transform;
    private SphereCollider _drop_collider;

    // Use this for initialization
    void Start() {
        _drop_transform = gameObject.transform;
        _drop_collider = gameObject.GetComponent<SphereCollider>();
        _drop_transform.localScale = Vector3.one;

        _size = 1;
        _targetting_size = 0;
        SetSize(1);
    }

    // Update is called once per frame
    void Update() {
        

        GradualModifySize();
    }

    public void IncrementSize() {
        SetSize(_size + 1);
    }

    public void DecrementSize() {
        SetSize(_size - 1);
    }

    public void SetSize(int size) {
        if(size > 0 && this._size != size) {
            //TODO: watch this value, using only x scale, presuppose  x,y,z has the same scale
            float difference = size - _drop_transform.localScale.x;
            _targetting_size = Mathf.Abs(difference);
            //positive if I grow up
            _shrink_or_enlarge = (difference > 0) ? (int)Mathf.Ceil(difference) :
                                                    (int)Mathf.Floor(difference);
            this._size = size;
        }
    }

    public float GetSize() {
        return _size;
    }

    private void SetCenter(float previousRadius, float newRadius) {
        float offset = newRadius - previousRadius;

        Vector3 localPosition = _drop_transform.localPosition;
        localPosition.y += offset;
        _drop_transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
    }

    private void GradualModifySize() {
        if(_targetting_size > 0) {
            float radius = _drop_collider.radius;
            float previous_radius = radius * _drop_transform.localScale.x;

            //positive if I grow up
            float speed = (_shrink_or_enlarge < 0) ? shrink_speed : enlarge_speed;

            _targetting_size -= Time.deltaTime * speed * Mathf.Abs(_shrink_or_enlarge);
            //if finally reached the target size, set the size so we don't have floating remains
            if(_targetting_size <= 0) {
                _drop_transform.localScale = new Vector3(_size, _size, _size);
                _targetting_size = 0;
            } else 
                _drop_transform.localScale += Vector3.one * Time.deltaTime * speed * _shrink_or_enlarge;

            radius = _drop_collider.radius;
            float new_radius = radius * _drop_transform.localScale.x;
            SetCenter(previous_radius, new_radius);
        }
    }

    public bool CanSetSize(int size) {
        return true;
	}

	public void OnCustomCollision(RaycastHit hit) {
		// TODO: Test method, remove at will
	}

	public void OnCustomCollisionEnter(RaycastHit hit) {
		// TODO: Test method, remove at will
	}

	public void OnCustomCollisionStay(RaycastHit hit) {
		// TODO: Test method, remove at will
	}

	public void OnCustomCollisionExit(RaycastHit hit) {
		// TODO: Test method, remove at will
	}
}