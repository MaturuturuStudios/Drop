using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WaterRepulsion : MonoBehaviour {
    #region Public attributes
    public Transform pointExpulsion;
    public Transform pointTarget;
	/// <summary>
	/// Angle of the trajectory
	/// </summary>
	public float angle=45;
    #endregion

    #region Private attributes
    private List<GameObject> _enteredDrop;
    private Bounds _ownCollider;
    #endregion

    #region Methods
    // Use this for initialization
    void Start () {
        //get the collider
        _ownCollider = GetComponent<Collider>().bounds;
        //create list
        _enteredDrop = new List<GameObject>();
    }

    public void Update() {
        //no drop? get out
        if (_enteredDrop.Count == 0) return;
		Vector3 directionShoot = pointTarget.position - pointExpulsion.position;
		if (directionShoot.x < 0 && angle < 90)	angle += 90;

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
				drop.transform.position = pointExpulsion.position;

                //send it flying
				controller.SendFlying(GetNeededVelocityVector());
            }
        }
    }

	public Vector3 GetNeededVelocityVector(){
		Vector3 velocityVector=Vector3.zero;
		float angleRadian = angle * Mathf.Deg2Rad;
		float velocity = GetNeededVelocity(angleRadian);

		velocityVector.x = Mathf.Cos (angleRadian) * velocity;
		velocityVector.y = Mathf.Sin (angleRadian) * velocity;

		return velocityVector;
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

	/// <summary>
	/// Gets the needed velocity.
	/// </summary>
	/// <returns>The needed velocity.</returns>
	/// <param name="angleRadian">Angle in radian.</param>
	private float GetNeededVelocity(float angleRadian){
		float cosAngle = Mathf.Cos(angleRadian);
		float cosAnglePow = cosAngle * cosAngle;
		Vector3 direction = pointTarget.position - pointExpulsion.position;
		float tangent = (direction.y) - Mathf.Tan (angleRadian) * (direction.x);

		float squaredVelocity = (-25* direction.x * direction.x) 
			/ (2 * cosAnglePow	* tangent);
		float velocity= Mathf.Sqrt(squaredVelocity);

		return velocity;
	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// </summary>
	public void OnDrawGizmos() {
		if (!Application.isPlaying) {
			RaycastHit hitpoint;
			Vector3[] points=new Vector3[100];

			float localAngle = angle;
			Vector3 directionShoot = pointTarget.position - pointExpulsion.position;
			if (directionShoot.x < 0 && angle < 90)	localAngle += (90-angle) * 2;
			else if(directionShoot.x > 0 && angle > 90) localAngle += (90-angle) * 2;

			float angleRadian = localAngle * Mathf.Deg2Rad;
			float velocity = GetNeededVelocity(angleRadian);

			float fTime = 0.1f;
			for (int i = 0; i < points.Length; i++)	{
				float dx = velocity * fTime * Mathf.Cos(angleRadian);
				float dy = velocity * fTime * Mathf.Sin(angleRadian) - ((25) * fTime * fTime / 2.0f);

				Vector3 position = new Vector3(pointExpulsion.position.x + dx, pointExpulsion.position.y + dy, 0);
				points[i] = position;
				fTime += 0.1f;

				Gizmos.color = Color.green;
				if (i > 0){
					Vector3 f = points[i-1] - points[i];
					if ((Physics.Raycast(points[i], f, out hitpoint, f.magnitude))) break;
					else Gizmos.DrawRay(points[i], f);
					
				}else if(i==0){
					Vector3 f = pointExpulsion.position-points[i];
					Gizmos.DrawRay(points[i], f);
				}

			}
		}

	}
    #endregion
}
