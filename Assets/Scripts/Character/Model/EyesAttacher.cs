using UnityEngine;
using System.Linq;

/// <summary>
/// Component that attachs the eyes of the character
/// to it's mesh. It also allows to change the eye
/// separation, scale and penetration.
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class EyesAttacher : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// A reference to the character's left eye bone.
	/// </summary>
	public Transform leftEye;

	/// <summary>
	/// A reference to the character's right eye bone.
	/// </summary>
	public Transform rightEye;

	/// <summary>
	/// The horizontal separation distance between the
	/// eyes and their vertical displacement.
	/// </summary>
	public Vector2 eyeSeparation;

	/// <summary>
	/// Scales the eyes to make them bigger or smaller.
	/// </summary>
	public float eyeScale = 1;

	/// <summary>
	/// Moves the eyes inside the model to prvent big eyes
	/// from floating in the air.
	/// </summary>
	public float eyePenetration = 0;

	/// <summary>
	/// A reference to the mesh renderer which contains the
	/// mesh the eyes will be attached to.
	/// </summary>
	public SkinnedMeshRenderer skinnedMeshRenderer;

	/// <summary>
	/// A reference to the center of the object, where
	/// the rays will be casted from.
	/// </summary>
	public Transform center;

	/// <summary>
	/// Rate the eyes will rotate to fit their desired
	/// orientation.
	/// </summary>
	public float rotationSpeed = 10;

	/// <summary>
	/// The maximum radius of the character. Avoid small
	/// numbers to prevent the eyes from dissapearing.
	/// </summary>
	public float maxRadius = 10;

	#endregion

	#region Private Fields

	/// <summary>
	/// A reference to the entity's Mesh Collider component.
	/// </summary>
	private MeshCollider _meshCollider;

	/// <summary>
	/// Mesh where the skinned mesh will be baked on.
	/// </summary>
	private Mesh _bakedMesh;

	/// <summary>
	/// Rotation difference from the left eye's original
	/// rotation to the surface normal.
	/// </summary>
	private Quaternion _leftEyeOriginalRotation;

	/// <summary>
	/// Rotation difference from the right eye's original
	/// rotation to the surface normal.
	/// </summary>
	private Quaternion _rightEyeOriginalRotation;

	/// <summary>
	/// Original scale of the left eye.
	/// </summary>
	private Vector3 _leftEyeOriginalScale;

	/// <summary>
	/// Original scale of the right eye.
	/// </summary>
	private Vector3 _rightEyeOriginalScale;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after this object is created.
	/// </summary>
	void Awake() {
		// Retireves the desired components
		_meshCollider = GetComponent<MeshCollider>();

		// Initialization
		_bakedMesh = new Mesh();
	}

	/// <summary>
	/// Unity's method called on the first frame this object is active.
	/// </summary>
	void Start() {
		// Updates the mesh in the collider
		UpdateMesh();

		// Left eye
		float radius = maxRadius * center.lossyScale.x;
		Vector3 direction = leftEye.position - center.position;
		RaycastHit hit = GetEyePosition(direction, radius);
		_leftEyeOriginalRotation = Quaternion.FromToRotation(hit.normal, leftEye.forward);
		_leftEyeOriginalRotation.x *= -1;
		_leftEyeOriginalScale = leftEye.localScale;

		// Right eye
		direction = rightEye.position - center.position;
		hit = GetEyePosition(direction, radius);
		_rightEyeOriginalRotation = Quaternion.FromToRotation(hit.normal, rightEye.forward);
		_rightEyeOriginalRotation.x *= -1;
		_rightEyeOriginalScale = rightEye.localScale;
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// </summary>
	void LateUpdate () {
		// Updates the mesh in the collider
		UpdateMesh();

		// Left eye
		float radius = maxRadius * center.lossyScale.x;
		Vector3 direction = Quaternion.Euler(eyeSeparation.y, -eyeSeparation.x, 0) * -center.forward;
		RaycastHit hit = GetEyePosition(direction, radius);
		leftEye.position = hit.point - hit.normal * eyePenetration * eyeScale * center.lossyScale.x;
		Quaternion targetRotation = Quaternion.LookRotation(hit.normal) * _leftEyeOriginalRotation;
		leftEye.rotation = Quaternion.Lerp(leftEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		//leftEye.rotation = targetRotation;
		leftEye.localScale = _leftEyeOriginalScale * eyeScale;

		// Right eye
		direction = Quaternion.Euler(eyeSeparation.y, eyeSeparation.x, 0) * -center.forward;
		hit = GetEyePosition(direction, radius);
		rightEye.position = hit.point - hit.normal * eyePenetration * eyeScale * center.lossyScale.x;
		targetRotation = Quaternion.LookRotation(hit.normal) * _rightEyeOriginalRotation;
		rightEye.rotation = Quaternion.Lerp(rightEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		//rightEye.rotation = targetRotation;
		rightEye.localScale = _rightEyeOriginalScale * eyeScale;
	}

	/// <summary>
	/// Updates the mesh information in the Mesh Collider.
	/// </summary>
	private void UpdateMesh() {
		// Copy the mesh from the renderer
		skinnedMeshRenderer.BakeMesh(_bakedMesh);

		// As the baked mesh is already scaled, we should shrink it
		float shrinkFactor = 1.0f / skinnedMeshRenderer.transform.lossyScale.x;
		Vector3[] vertices = _bakedMesh.vertices;
		vertices = vertices.Select(e => e * shrinkFactor).ToArray();
		_bakedMesh.vertices = vertices;

		// Updates the emsh in the collider
		_meshCollider.sharedMesh = _bakedMesh;
	}
	
	/// <summary>
	/// Gets the information about the point an eye should
	/// be standing on.
	/// </summary>
	/// <param name="direction"> The direction for the eye to be</param>
	/// <param name="radius">The maximum distance the eye should be</param>
	/// <returns>A RaycastHit with the position information</returns>
	private RaycastHit GetEyePosition(Vector3 direction, float radius) {
		// Casts a ray to look for the eye's position
		RaycastHit hit;
		_meshCollider.Raycast(new Ray(center.position + direction * radius, -direction), out hit, radius);
		return hit;
	}

	#endregion
}
