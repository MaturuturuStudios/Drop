using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject currentCharacter;

    public float far = -15.0f;
    public float up = 3.0f;
    public float lookAtSpeed = 0.5f;

    private Vector3 _offset;

    private Vector3 _lastPosition;

    void OnEnable()
    {
        _offset = new Vector3(0.0f, up, far);
        transform.position = currentCharacter.transform.position + _offset;

        _lastPosition = currentCharacter.transform.position;
    }

    void LateUpdate()
    {
        Vector3 objective = currentCharacter.transform.position;
        Vector3 diff = currentCharacter.transform.position - _lastPosition;
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        if (diff.x > lookAtSpeed)
        {
            objective.x = _lastPosition.x + (lookAtSpeed * ratioXY);
        }
        else if (diff.x < -lookAtSpeed)
        {
            objective.x = _lastPosition.x - (lookAtSpeed * ratioXY);
        }


        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;
        if (diff.y > lookAtSpeed)
        {
            objective.y = _lastPosition.y + (lookAtSpeed * ratioXY);
        }
        else if (diff.y < -lookAtSpeed)
        {
            objective.y = _lastPosition.y - (lookAtSpeed * ratioXY);
        }

        transform.LookAt(objective);

        _lastPosition = objective;

    }

    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
