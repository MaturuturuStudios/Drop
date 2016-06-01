using UnityEngine;
using System.Collections;
using System;

public abstract class LaunchCharacter : ActionPerformer {

    /// <summary>
    /// Point in which the character will be shooted
    /// </summary>
    public Transform pointOrigin;
    /// <summary>
    /// Point in which the drop will end
    /// </summary>
    public Transform pointTarget;
    /// <summary>
    /// Angle of the trajectory
    /// </summary>
    [Range(-90, 90)]
    public float angle = 45;
    /// <summary>
	/// Layers the trajectory will collide with.
	/// </summary>
	public LayerMask layerMask;

    /// <summary>
    /// Get the angle in a range [0,180]
    /// </summary>
    /// <returns></returns>
    public float GetAngle() {
        float finalAngle = angle;
        if (finalAngle < 0) {
            finalAngle = 180 + finalAngle;
        }
        return finalAngle;
    }

    /// <summary>
    /// Clamped to -90, 90,
    /// if not, will be ignored
    /// </summary>
    /// <param name="angle"></param>
    public void SetAngle(float angle) {
        if (angle > 90 && angle < -90) return;
        angle = angle % 180;
        this.angle = angle;
    }
	
	// Update is called once per frame
	void Update () {
        float finalAngle = angle;
        if (finalAngle < 0) {
            finalAngle = 180 + finalAngle;
        }
        Debug.Log(finalAngle);
        if (pointOrigin == null) pointOrigin = transform;
    }

    /// <summary>
    /// Get the necessary velocity to send the drop to the destiny point
    /// </summary>
    /// <returns></returns>
	public Vector3 GetNeededVelocityVector() {
        Vector3 velocityVector = Vector3.zero;

        float finalAngle = angle;
        if (finalAngle < 0) {
            finalAngle = 180 + finalAngle;
        }
        float angleRadian = finalAngle * Mathf.Deg2Rad;
        float velocity = GetNeededVelocity(angleRadian);

        velocityVector.x = Mathf.Cos(angleRadian) * velocity;
        velocityVector.y = Mathf.Sin(angleRadian) * velocity;

        return velocityVector;
    }

    /// <summary>
	/// Gets the needed velocity depending on angle
	/// </summary>
	/// <returns>The needed velocity.</returns>
	/// <param name="angleRadian">Angle in radian.</param>
	protected float GetNeededVelocity(float angleRadian) {
        float cosAngle = Mathf.Cos(angleRadian);
        float cosAnglePow = cosAngle * cosAngle;
        Vector3 direction = pointTarget.position - pointOrigin.position;
        float tangent = (direction.y) - Mathf.Tan(angleRadian) * (direction.x);

        float squaredVelocity = (-25 * direction.x * direction.x)
            / (2 * cosAnglePow * tangent);
        float velocity = Mathf.Sqrt(squaredVelocity);

        return velocity;
    }

    /// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	public void OnDrawGizmos() {
        if (!Application.isPlaying) {
            if (pointOrigin == null) return;

            RaycastHit hitpoint;
            Vector3[] points = new Vector3[100];

            float finalAngle = angle;
            if (finalAngle < 0) {
                finalAngle = 180 + finalAngle;
            }
            float localAngle = finalAngle;

            float angleRadian = localAngle * Mathf.Deg2Rad;
            float velocity = GetNeededVelocity(angleRadian);

            float fTime = 0.1f;
            for (int i = 0; i < points.Length; i++) {
                float dx = velocity * fTime * Mathf.Cos(angleRadian);
                float dy = velocity * fTime * Mathf.Sin(angleRadian) - ((25) * fTime * fTime / 2.0f);

                Vector3 position = new Vector3(pointOrigin.position.x + dx, pointOrigin.position.y + dy, 0);
                points[i] = position;
                fTime += 0.1f;

                Gizmos.color = Color.green;
                if (i > 0) {
                    Vector3 f = points[i - 1] - points[i];
                    if ((Physics.Raycast(points[i], f, out hitpoint, f.magnitude, layerMask))) break;
                    else Gizmos.DrawRay(points[i], f);

                } else if (i == 0) {
                    Vector3 f = pointOrigin.position - points[i];
                    Gizmos.DrawRay(points[i], f);
                }

            }
        }
    }
}
