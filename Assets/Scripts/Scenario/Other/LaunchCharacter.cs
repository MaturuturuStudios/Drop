using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class LaunchCharacter {

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

        while (finalAngle > 180 || finalAngle < 0) finalAngle %= 180;

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

    /// <summary>
    /// Get the necessary velocity to send the drop to the destiny point
    /// </summary>
    /// <returns></returns>
	public Vector3 GetNeededVelocityVector() {
        Vector3 velocityVector = Vector3.zero;
        if (pointOrigin == null) return velocityVector;

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
	private float GetNeededVelocity(float angleRadian) {
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
			points[0] = pointOrigin.position;

			Gizmos.color = Color.green;
			for (int i = 1; i < points.Length; i++) {
                float dx = velocity * fTime * Mathf.Cos(angleRadian);
                float dy = velocity * fTime * Mathf.Sin(angleRadian) - ((25) * fTime * fTime / 2.0f);

                Vector3 position = pointOrigin.position + new Vector3(dx, dy, 0);
                points[i] = position;
                fTime += 0.1f;
                Vector3 f = points[i] - points[i - 1];
                if (Physics.Raycast(points[i - 1], f, out hitpoint, f.magnitude, layerMask)) {
					Gizmos.DrawLine(points[i - 1], hitpoint.point);
					break;
				}
                else
					Gizmos.DrawLine(points[i - 1], points[i]);

            }
        }
    }
}
