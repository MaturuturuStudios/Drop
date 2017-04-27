using UnityEngine;

/// <summary>
/// Component that attachs the eyes of the character
/// to it's mesh. It also allows to change the eye
/// separation, scale and penetration.
/// </summary>
public class EyesController : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// A reference to the character's left eye bone.
	/// </summary>
	[Header("References")]
	public Transform leftEye;

	/// <summary>
	/// A reference to the character's right eye bone.
	/// </summary>
	public Transform rightEye;

	/// <summary>
	/// A reference to the center of the object, where
	/// the rays will be casted from.
	/// </summary>
	public Transform center;


	/// <summary>
	/// Color for the eyes to be tinted. The left side
	/// will be used when no blendshape is applied, while
	/// the right one will be used when one is used.
	/// </summary>
	[Header("Eyes Color")]
	public Gradient blendShapeEyeColor;

	/// <summary>
	/// The max weight the blend shapes will have. Used
	/// to normalize.
	/// </summary>
	public float maxBlendShapeWeight = 100.0f;

	/// <summary>
	/// The horizontal separation distance between the
	/// eyes and their vertical displacement.
	/// </summary>
	[HideInInspector]
	public Vector2 eyeSeparation;

	/// <summary>
	/// Scales the eyes to make them bigger or smaller.
	/// </summary>
	[HideInInspector]
	public float eyeScale = 1;

	/// <summary>
	/// Moves the eyes inside the model to prvent big eyes
	/// from floating in the air.
	/// </summary>
	[HideInInspector]
	public float eyePenetration = 0;

	#endregion

	#region Private Fields

	/// <summary>
	/// Original distance from the center to the left eye.
	/// </summary>
	private float _leftEyeOriginalDistance;

	/// <summary>
	/// Original distance from the center to the right eye.
	/// </summary>
	private float _rightEyeOriginalDistance;

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
	/// Original color of the left eye.
	/// </summary>
	private Color _leftEyeOriginalColor;

	/// <summary>
	/// Original color of the right eye.
	/// </summary>
	private Color _rightEyeOriginalColor;

	/// <summary>
	/// Reference to the left eye's renderer.
	/// </summary>
	private SkinnedMeshRenderer _leftEyeRenderer;

	/// <summary>
	/// Reference to the right eye's renderer.
	/// </summary>
	private SkinnedMeshRenderer _rightEyeRenderer;

	#endregion

	#region Methods

	void Awake() {
		// Retrieves the references to the desired components.
		_leftEyeRenderer = leftEye.GetComponentInChildren<SkinnedMeshRenderer>();
		_rightEyeRenderer = rightEye.GetComponentInChildren<SkinnedMeshRenderer>();
	}

	/// <summary>
	/// Unity's method called on the first frame this object is active.
	/// </summary>
	void Start() {

		// Saves the left eye's configuration
		Vector3 direction = leftEye.position - center.position;
		_leftEyeOriginalDistance = direction.magnitude / center.lossyScale.x;
		_leftEyeOriginalRotation = Quaternion.FromToRotation(direction, leftEye.forward);
		_leftEyeOriginalRotation.x *= -1;
		_leftEyeOriginalScale = leftEye.localScale;
		_leftEyeOriginalColor = _leftEyeRenderer.material.color;

		// Saves the right eye's configuration
		direction = rightEye.position - center.position;
		_rightEyeOriginalDistance = direction.magnitude / center.lossyScale.x;
		_rightEyeOriginalRotation = Quaternion.FromToRotation(direction, rightEye.forward);
		_rightEyeOriginalRotation.x *= -1;
		_rightEyeOriginalScale = rightEye.localScale;
		_rightEyeOriginalColor = _rightEyeRenderer.material.color;

		// Mirrors the right eye's texture coordinates
		rightEye.GetComponentInChildren<Renderer>().material.SetFloat("Mirror", 1);
	}

	/// <summary>
	/// Unity's method called when the component becomes enabled.
	/// </summary>
	void OnDisable() {
		// Checks if the object has been destroyed
		if (leftEye == null || rightEye == null)
			return;

		// Resets the eye's blend shapes
		for (int i = 0; i < _leftEyeRenderer.sharedMesh.blendShapeCount; i++)
			_leftEyeRenderer.SetBlendShapeWeight(i, 0);
		for (int i = 0; i < _rightEyeRenderer.sharedMesh.blendShapeCount; i++)
			_rightEyeRenderer.SetBlendShapeWeight(i, 0);
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// </summary>
	void LateUpdate () {

		// Updates the left eye
		Vector3 direction = Quaternion.Euler(eyeSeparation.y, -eyeSeparation.x, 0) * -center.forward;
		leftEye.position = center.position + direction * (_leftEyeOriginalDistance - eyePenetration * eyeScale) * center.lossyScale.x;
		leftEye.rotation = Quaternion.LookRotation(direction) * _leftEyeOriginalRotation;
		leftEye.localScale = _leftEyeOriginalScale * eyeScale;
		_leftEyeRenderer.material.color = _leftEyeOriginalColor * blendShapeEyeColor.Evaluate(GetMaxBlendShapeValue(_leftEyeRenderer));

		// Updates the right eye
		direction = Quaternion.Euler(eyeSeparation.y, eyeSeparation.x, 0) * -center.forward;
		rightEye.position = center.position + direction * (_rightEyeOriginalDistance - eyePenetration * eyeScale) * center.lossyScale.x;
		rightEye.rotation = Quaternion.LookRotation(direction) * _rightEyeOriginalRotation;
		rightEye.localScale = _rightEyeOriginalScale * eyeScale;
		_rightEyeRenderer.material.color = _rightEyeOriginalColor * blendShapeEyeColor.Evaluate(GetMaxBlendShapeValue(_rightEyeRenderer));
	}

	private float GetMaxBlendShapeValue(SkinnedMeshRenderer renderer) {
		float maxBlendShapeValue = 0.0f;
		for (int blendShape = 0; blendShape < renderer.sharedMesh.blendShapeCount; ++blendShape)
			maxBlendShapeValue = Mathf.Max(maxBlendShapeValue, renderer.GetBlendShapeWeight(blendShape));
		return maxBlendShapeValue / maxBlendShapeWeight;
	}

	#endregion
}
