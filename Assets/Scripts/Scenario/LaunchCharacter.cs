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
    /// Get the angle as unity use it [0,180]
    /// </summary>
    /// <returns></returns>
    public float GetAngle() {
        float finalAngle = angle;

        if (finalAngle == 0) {
            finalAngle = 90;
        } else if (finalAngle > 90) {
            finalAngle -= 90;
        } else {
            finalAngle -= 90;
            finalAngle *= -1;
        }

        return finalAngle;
    }

    /// <summary>
    /// Get the  given angle in a range [90,-90]
    /// 90 at right, -90 at left, 0 at up
    /// </summary>
    /// <returns></returns>
    public float GetAngleClamped(float value) {
        float finalAngle = value;

        while (finalAngle > 180 || finalAngle <180) finalAngle %= 180;
        
        if (finalAngle == 90) finalAngle = 0;
        else if (finalAngle < 90) {
            finalAngle = 90 - finalAngle;
        } else {
            finalAngle -= 90;
            finalAngle *= -1;
        }
        
        return finalAngle;
    }

    /// <summary>
    /// will convert between [90,-90]
    /// </summary>
    /// <param name="angle"></param>
    public void SetAngle(float angle) {
        angle = GetAngleClamped(angle);
    }

    /// <summary>
    /// Value between -90 and 90
    /// if not, will be ignored
    /// </summary>
    /// <param name="angle"></param>
    public void SetAngleClamped(float angle) {
        if (angle > 90 && angle < -90) return;
        this.angle = angle;
    }
	
	// Update is called once per frame
	void Update () {
        if (pointOrigin == null) pointOrigin = transform;
    }

    /// <summary>
    /// Get the necessary velocity to send the drop to the destiny point
    /// </summary>
    /// <returns></returns>
	public Vector3 GetNeededVelocityVector() {
        Vector3 velocityVector = Vector3.zero;

        //get angle [0,180]
        float finalAngle = GetAngle();
        float angleRadian = finalAngle * Mathf.Deg2Rad;
        float velocity = GetNeededVelocity(angleRadian);

        velocityVector.x = Mathf.Cos(angleRadian) * velocity;
        velocityVector.y = Mathf.Sin(angleRadian) * velocity;

        return velocityVector;
    }

    /// <summary>
	/// Gets the needed velocity depending on angle [0,180]
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
            
            //get angle [0,180]
            float localAngle = GetAngle();

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
