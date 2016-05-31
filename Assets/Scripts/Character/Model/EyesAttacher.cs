using UnityEngine;
using System.Linq;

/// <summary>
/// Component that attachs the eyes of the character
/// to it's mesh. It also allows to change the eye
/// separation, scale and penetration.
/// </summary>
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

	#endregion

	#region Private Fields

	/// <summary>
	/// Reference to the MeshRenderer's Transform component.
	/// </summary>
	private Transform _meshTransform;

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

	/// <summary>
	/// Number of blend shapes used in the eyes.
	/// </summary>
	private const int BLEND_SHAPE_COUNT = 4;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after this object is created.
	/// </summary>
	void Awake() {
		// Initialization
		_meshTransform = skinnedMeshRenderer.transform;
        _bakedMesh = new Mesh();
    }

	/// <summary>
	/// Unity's method called on the first frame this object is active.
	/// </summary>
	void Start() {
		// Updates the mesh from the mesh renderer
		UpdateMesh();

		// Left eye
		Vector3 direction = leftEye.position - center.position;
		RaycastHit hit = GetEyePosition(direction);
		_leftEyeOriginalRotation = Quaternion.FromToRotation(hit.normal, leftEye.forward);
		_leftEyeOriginalRotation.x *= -1;
		_leftEyeOriginalScale = leftEye.localScale;

		// Right eye
		direction = rightEye.position - center.position;
		hit = GetEyePosition(direction);
		_rightEyeOriginalRotation = Quaternion.FromToRotation(hit.normal, rightEye.forward);
		_rightEyeOriginalRotation.x *= -1;
		_rightEyeOriginalScale = rightEye.localScale;
	}

	/// <summary>
	/// Unity's method called when the component becomes enabled.
	/// </summary>
	void OnDisable() {
		// Resets the eye's blend shapes
		SkinnedMeshRenderer eyeRenderer = leftEye.GetComponentInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < BLEND_SHAPE_COUNT; i++)
			eyeRenderer.SetBlendShapeWeight(i, 0);
		eyeRenderer = rightEye.GetComponentInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < BLEND_SHAPE_COUNT; i++)
			eyeRenderer.SetBlendShapeWeight(i, 0);
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// </summary>
	void LateUpdate () {
		// Updates the mesh from the mesh renderer
		UpdateMesh();

		// Left eye
		Vector3 direction = Quaternion.Euler(eyeSeparation.y, -eyeSeparation.x, 0) * -center.forward;
		RaycastHit hit = GetEyePosition(direction);
		leftEye.position = hit.point - hit.normal * eyePenetration * eyeScale * center.lossyScale.x;
		Quaternion targetRotation = Quaternion.LookRotation(hit.normal) * _leftEyeOriginalRotation;
		leftEye.rotation = Quaternion.Lerp(leftEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		leftEye.localScale = _leftEyeOriginalScale * eyeScale;

		// Right eye
		direction = Quaternion.Euler(eyeSeparation.y, eyeSeparation.x, 0) * -center.forward;
		hit = GetEyePosition(direction);
		rightEye.position = hit.point - hit.normal * eyePenetration * eyeScale * center.lossyScale.x;
		targetRotation = Quaternion.LookRotation(hit.normal) * _rightEyeOriginalRotation;
		rightEye.rotation = Quaternion.Lerp(rightEye.rotation, targetRotation, rotationSpeed * Time.deltaTime);
		rightEye.localScale = _rightEyeOriginalScale * eyeScale;
	}

	/// <summary>
	/// Updates the mesh information from the SkinnedMeshRenderer.
	/// </summary>
	private void UpdateMesh() {
		// Copy the mesh from the renderer
		skinnedMeshRenderer.BakeMesh(_bakedMesh);

		// Transforms the mesh points to global coordinates. As the baked mesh is already scaled, ignores it
		float shrinkFactor = 1.0f / _meshTransform.lossyScale.x;
		Vector3[] vertices = _bakedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
			vertices[i] *= shrinkFactor;
		_bakedMesh.vertices = vertices;
	}
	
	/// <summary>
	/// Gets the information about the point an eye should
	/// be standing on.
	/// </summary>
	/// <param name="direction"> The direction for the eye to be</param>
	/// <returns>A RaycastHit with the position information</returns>
	private RaycastHit GetEyePosition(Vector3 direction) {
		// Casts a ray to look for the eye's position
		RaycastHit hit;
		if (!_bakedMesh.Raycast(_meshTransform.InverseTransformPoint(center.position + 10 * direction), _meshTransform.InverseTransformDirection(-direction), out hit))
			Debug.LogError("Error: Eye raycast failed it's target!");
		hit.point = _meshTransform.TransformPoint(hit.point);
		hit.normal = _meshTransform.TransformDirection(hit.normal);
		return hit;
	}

	#endregion
}
