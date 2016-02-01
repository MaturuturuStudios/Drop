using UnityEngine;
using System.Collections;

public class Character_Size : MonoBehaviour {
    public float shrink_speed = 0.4f;
    public float enlarge_speed = 0.4f;

    public int set_debug_can_size = 4;

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
        if(set_debug_can_size > 0 && Input.GetKeyDown(KeyCode.T))
            CanSetSize(set_debug_can_size);

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

    private struct InfoAxis {
        //0 for horizontal
        //1 for vertical
        public int axis;
        public Vector2 offset;
        public bool block;
    };

    public bool CanSetSize(int size) {
        bool can_grow_up = true;
        _drop_collider.enabled = false;


        //get the data of the drop
        Vector3 position = _drop_transform.position;
        float radius = _drop_collider.radius * size;

        //resize
        float previous_radius = _drop_collider.radius * _drop_transform.localScale.x;
        float offset = radius - previous_radius;
        position.y += offset;

        RaycastHit[] hits = Physics.SphereCastAll(position, radius, Vector3.forward, 0);
        Debug.Log(hits.Length);
        if(hits.Length == 0)
            return can_grow_up;


        InfoAxis horizontal_axis = checkAxis(0, position, radius, hits);
        InfoAxis vertical_axis = checkAxis(1, position, radius, hits);

        //both blocked? no way
        if(horizontal_axis.block && vertical_axis.block)
            can_grow_up = false;
        //both free? let's go!
        else if(!horizontal_axis.block && !vertical_axis.block)
            can_grow_up = true;

        //ammm... let's go for a second round moving a little the ball
        else {
            InfoAxis[] info = { horizontal_axis, vertical_axis };
            can_grow_up = checkSizeMove(position, radius, info);
        }

        _drop_collider.enabled = true;
        Debug.Log(can_grow_up);
        return can_grow_up;
    }

    //get the boxes for both sides of the axis
    private Vector3[] GetBoxesAxis(int axis, Vector3 original_position, float radius) {
        //make the box to check
        float half_side = radius / 2;
        Vector3 half_side_box = new Vector3(half_side, half_side, half_side);

        Vector3 one_side_center_box = new Vector3(original_position.x, original_position.y, original_position.z);
        Vector3 other_side_center_box = new Vector3(original_position.x, original_position.y, original_position.z);
        Vector2 pivot = new Vector2(original_position.x, original_position.y);

        //checking the axis, we center the position to see both sides of axis,also, rotate it to make vertical/horizontal
        switch(axis) {
            //horizontal axis
            case 0:
                one_side_center_box.x -= half_side;
                one_side_center_box.y += half_side;

                other_side_center_box.x += half_side;
                other_side_center_box.y -= half_side;

                drawBox(one_side_center_box, pivot, Mathf.PI / 4, radius);
                drawBox(other_side_center_box, pivot, Mathf.PI / 4, radius);
                //fix the position because of the pivot effect (overlapbox hasn't a pivot option)
                one_side_center_box.y -= half_side;
                other_side_center_box.y += half_side;
                break;

            //vertical axis
            case 1:
                one_side_center_box.x -= half_side;
                one_side_center_box.y -= half_side;

                other_side_center_box.x += half_side;
                other_side_center_box.y += half_side;

                drawBox(one_side_center_box, pivot, Mathf.PI / 4, radius);
                drawBox(other_side_center_box, pivot, Mathf.PI / 4, radius);
                //fix the position because of the pivot effect (overlapbox hasn't a pivot option)
                one_side_center_box.x += half_side;
                other_side_center_box.x -= half_side;
                break;
        }

        Vector3[] result = { one_side_center_box, other_side_center_box };
        return result;
    }

    private InfoAxis checkAxis(int axis, Vector3 position, float radius, RaycastHit[] hits) {
        InfoAxis info_result;
        info_result.axis = axis;
        info_result.block = false;
        info_result.offset = Vector3.zero;

        Vector3 offset = Vector3.zero;

        Quaternion rotation = Quaternion.Euler(0, 0, 45);

        //make the box to check
        float half_side = radius / 2;
        Vector3 half_side_box = new Vector3(half_side, half_side, half_side);

        Vector3[] centers_box = GetBoxesAxis(axis, position, radius);
        Vector3 one_side_center_box = centers_box[0];
        Vector3 other_side_center_box = centers_box[1];

        //see one side
        Collider[] colliders = Physics.OverlapBox(one_side_center_box, half_side_box, rotation);
        //if collision, get the offset

        bool found = false;
        //make sure is not a false positive and get the hit
        for(int i = 0; i < colliders.Length && !found; i++) {
            for(int j = 0; j < hits.Length && !found; j++) {
                if(colliders[i] == hits[j].collider) {
                    found = true;
                    //get the offset to move
                    Debug.Log(position);
                    Debug.Log(hits[j].point);
                    //TODO: ERROR the Raycasthit comes null, no point, no position, no contact...
                    offset = position - hits[j].point;
                    Debug.Log("First side collides");
                    Debug.Log(offset);
                }
            }
        }

        //see other side with offset
        other_side_center_box += offset;
        /*switch(axis) {
            case 0:
                other_side_center_box.x += offset.x;
                break;
            case 1:
                other_side_center_box.y += offset.y;
                break;
        }*/

        colliders = Physics.OverlapBox(other_side_center_box, half_side_box, rotation);

        found = false;
        //make sure is not a false positive and get the hit
        for(int i = 0; i < colliders.Length && !found; i++) {
            for(int j = 0; j < hits.Length && !found; j++) {
                if(colliders[i] == hits[j].collider) {
                    found = true;
                    Debug.Log("Second side collides");
                    //if offset was setted, the first side collides too, so is blocked
                    if(offset != Vector3.zero) {
                        info_result.block = true;
                    } else {
                        Debug.Log("first didn't");
                        offset = (-1) * (position - hits[j].point);
                    }
                }
            }
        }


        bool check_again = offset != Vector3.zero && !info_result.block;
        if(!check_again) {
            info_result.offset = offset;
            return info_result;
        }
        //check again the first side with the offset
        //see other side with offset
        switch(axis) {
            case 0:
                other_side_center_box.x += offset.x;
                break;
            case 1:
                other_side_center_box.y += offset.y;
                break;
        }

        colliders = Physics.OverlapBox(other_side_center_box, half_side_box, rotation);
        found = false;
        //make sure is not a false positive and get the hit
        for(int i = 0; i < colliders.Length && !found; i++) {
            for(int j = 0; j < hits.Length && !found; j++) {
                if(colliders[i] == hits[j].collider) {
                    Debug.Log("Checking again but detected block");
                    found = true;
                    info_result.block = true;
                }
            }
        }


        info_result.offset = offset;
        return info_result;
    }

    //draw the box given the position and pivot
    private void drawBox(Vector3 position_center, Vector2 pivot, float angle_radians, float side, float duration = 5) {
        float half_side = side / 2;

        Vector3 up_left = position_center;
        Vector3 up_right = position_center;
        Vector3 down_left = position_center;
        Vector3 down_right = position_center;

        up_left.x -= half_side;
        up_left.y += half_side;

        up_right.x += half_side;
        up_right.y += half_side;

        down_left.x -= half_side;
        down_left.y -= half_side;

        down_right.x += half_side;
        down_right.y -= half_side;

        Vector2 point = new Vector2(up_left.x, up_left.y);
        point = rotate_point(pivot, angle_radians, point);
        up_left.x = point.x;
        up_left.y = point.y;

        point = new Vector2(up_right.x, up_right.y);
        point = rotate_point(pivot, angle_radians, point);
        up_right.x = point.x;
        up_right.y = point.y;

        point = new Vector2(down_left.x, down_left.y);
        point = rotate_point(pivot, angle_radians, point);
        down_left.x = point.x;
        down_left.y = point.y;

        point = new Vector2(down_right.x, down_right.y);
        point = rotate_point(pivot, angle_radians, point);
        down_right.x = point.x;
        down_right.y = point.y;

        Debug.DrawLine(up_left, up_right, Color.white, duration, false);
        Debug.DrawLine(up_left, down_left, Color.white, duration, false);
        Debug.DrawLine(down_left, down_right, Color.white, duration, false);
        Debug.DrawLine(up_right, down_right, Color.white, duration, false);
    }

    //rotate a point given a pivot
    private Vector2 rotate_point(Vector2 pivot, float angle_radians, Vector2 point) {
        float s = Mathf.Sin(angle_radians);
        float c = Mathf.Cos(angle_radians);

        // translate point back to origin:
        point.x -= pivot.x;
        point.y -= pivot.y;

        // rotate point
        float xnew = point.x * c - point.y * s;
        float ynew = point.x * s + point.y * c;

        // translate point back:
        point.x = xnew + pivot.x;
        point.y = ynew + pivot.y;

        return point;
    }

    private bool IsNotBlocked(InfoAxis one, InfoAxis two) {
        return !(one.block || two.block);
    }

    private bool checkSizeMove(Vector3 original_position, float radius, InfoAxis[] info_axis) {
        bool can_grow_up = true;
        float tolerance_growing_crushed = radius / 4;

        int axis = 0;
        Vector3 offset = Vector3.zero;
        Vector3 tolerance = Vector3.zero;
        //check which axis is not blocked
        if(info_axis[0].block) {
            axis = 1;
            offset.y = info_axis[axis].offset.y;
            tolerance.y = tolerance_growing_crushed;
        } else {
            offset.x = info_axis[axis].offset.x;
            tolerance.x = tolerance_growing_crushed;
        }

        Vector3 position = original_position;
        if(offset != Vector3.zero) {
            position += offset;

            RaycastHit[] hits = Physics.SphereCastAll(position, radius, Vector3.one, 0);
            if(hits.Length == 0)
                return can_grow_up;

            InfoAxis horizontal_axis = checkAxis(0, position, radius, hits);
            InfoAxis vertical_axis = checkAxis(1, position, radius, hits);

            can_grow_up = IsNotBlocked(horizontal_axis, vertical_axis);

        } else {
            //two checks, one for directions of the axis
            //check one direction
            position = original_position + tolerance;
            RaycastHit[] hits = Physics.SphereCastAll(position, radius, Vector3.one, 0);
            if(hits.Length == 0) return can_grow_up;

            InfoAxis horizontal_axis = checkAxis(0, position, radius, hits);
            InfoAxis vertical_axis = checkAxis(1, position, radius, hits);

            can_grow_up = IsNotBlocked(horizontal_axis, vertical_axis);

            if(can_grow_up) return can_grow_up;

            //if that way is blocked, try with the other side
            position = original_position - tolerance;
            hits = Physics.SphereCastAll(position, radius, Vector3.one, 0);
            if(hits.Length == 0)
                return can_grow_up;

            horizontal_axis = checkAxis(0, position, radius, hits);
            vertical_axis = checkAxis(1, position, radius, hits);

            can_grow_up = IsNotBlocked(horizontal_axis, vertical_axis);

        }

        return can_grow_up;
    }
}