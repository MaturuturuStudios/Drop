using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the orientation of the character's model.
/// </summary>
public class CharacterModelController : MonoBehaviour, CharacterSize.CharacterSizeListener {

	#region Custom Enumerations

	/// <summary>
	/// Determines which direction will the charater be
	/// facing when the game starts.
	/// </summary>
	public enum InitialRotation {
		/// <summary>
		/// The character will use it's default rotation.
		/// </summary>
		None,
		/// <summary>
		/// The character will be facing right.
		/// </summary>
		Right,
		/// <summary>
		/// The character will be facing left.
		/// </summary>
		Left
	}

	/// <summary>
	/// Determines which interpolation method will be
	/// used to scale the character's attributes.
	/// </summary>
	public enum InterpolationMethod {
		/// <summary>
		/// Linear interpolation between the minimum and
		/// maximum scale.
		/// </summary>
		Linear,
		/// <summary>
		/// Cuadratic interpolation between the minimum and
		/// maximum scale.
		/// </summary>
		Cuadratic,
		/// <summary>
		/// Square rooted interpolation between the minimum
		/// and maximum scale.
		/// </summary>
		Squared
	}

	#endregion

	#region Public Attributes

	#region Rotation

	/// <summary>
	/// Determines wich direction will the charater be
	/// facing when the game starts.
	/// </summary>
	[Header("Rotation")]
	public InitialRotation initialRotation = InitialRotation.None;

	/// <summary>
	/// The angle this entity will face when walking
	/// sideways.
	/// </summary>
	public float rotationAngle = 60.0f;

	/// <summary>
	/// The angular velocity the character will use
	/// to turn.
	/// </summary>
	public float rotationAngularSpeed = 720.0f;

	#endregion

	#region Scale

	/// <summary>
	/// The minimum scale the character will have.
	/// </summary>
	[Header("Scale")]
	public float minScale = 1;

	/// <summary>
	/// The maximum scale the character will have.
	/// </summary>
	public float maxScale = 9;

	/// <summary>
	/// The interpolation method used to scale the character's
	/// attributes.
	/// </summary>
	public InterpolationMethod interpolationMethod = InterpolationMethod.Linear;

	#endregion

	#region Eyes

	/// <summary>
	/// Eye separation when the character is at minimum
	/// scale.
	/// </summary>
	[Header("Eyes")]
	public Vector2 minEyeSeparation = new Vector2(20, 0);

	/// <summary>
	/// Eye separation when the character is at maximum
	/// scale.
	/// </summary>
	public Vector2 maxEyeSeparation = new Vector2(20, 0);

	/// <summary>
	/// Eye size when the character is at minimum scale.
	/// </summary>
	public float minEyeScale = 1;

	/// <summary>
	/// Eye size when the character is at maximum scale.
	/// </summary>
	public float maxEyeScale = 1;

	/// <summary>
	/// Eye penetration when the character is at minimum scale.
	/// </summary>
	public float minEyePenetration = 0;

	/// <summary>
	/// Eye penetration when the character is at maximum scale.
	/// </summary>
	public float maxEyePenetration = 0;

	#endregion

	#region Hat

	/// <summary>
	/// Reference to the hat's root bone (the one with the
	/// kinematic Rigidbody).
	/// </summary>
	[Header("Hat")]
	public Transform hatRootBone;

	/// <summary>
	/// Hat rotation when the character is at minimum scale.
	/// </summary>
	public Vector3 minHatRotation = new Vector3(0, 30, 0);

	/// <summary>
	/// Hat rotation when the character is at maximum scale.
	/// </summary>
	public Vector3 maxHatRotation = new Vector3(0, 30, 0);

	/// <summary>
	/// Hat scale when the character is at minimum scale.
	/// </summary>
	public float minHatScale = 1;

	/// <summary>
	/// Hat scale when the character is at maximum scale.
	/// </summary>
	public float maxHatScale = 1;

	#endregion

	#region Deformation

	/// <summary>
	/// Reference to the deformation script on the model.
	/// </summary>
	[Header("Deformation")]
	public SkinnedSphereDeform deformationScript;

	/// <summary>
	/// The deformation the character will have when at
	/// minimum scale. Not this will change the character's
	/// visual scale.
	/// </summary>
	public float minDeformationScale = 1;

	/// <summary>
	/// The deformation the character will have when at
	/// maximum scale. Not this will change the character's
	/// visual scale.
	/// </summary>
	public float maxDeformationScale = 1;

	/// <summary>
	/// The chamf scale the character will have when at
	/// minimum scale;
	/// </summary>
	public float minChamfScale = 1;

	/// <summary>
	/// The chamf scale the character will have when at
	/// maximum scale;
	/// </summary>
	public float maxChamfScale = 1;

	#endregion

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to the CharacterControllerCustom on
	/// this entity's parent.
	/// </summary>
	private CharacterControllerCustom _ccc;

	/// <summary>
	/// A reference to the CharacterSize component on
	/// this entity's parent.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// A reference to the EyesAttacher component on
	/// this entity.
	/// </summary>
	private EyesAttacher _eyesAttacher;

	/// <summary>
	/// A reference to the model's parent.
	/// </summary>
	private Transform _parent;

	/// <summary>
	/// A reference to this entity's Transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// A reference to all physical joints in the model.
	/// </summary>
	private Joint[] _joints;

	/// <summary>
	/// A reference to the physical joint's Rigidbody components.
	/// </summary>
	private Rigidbody[] _jointsConnectedBodies;

	/// <summary>
	/// The saved position of all physical joints in the model.
	/// </summary>
	private Vector3[] _originalJointsPosition;

	/// <summary>
	/// The saved offset of the model when the game starts.
	/// </summary>
	private Vector3 _originalModelOffset;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_eyesAttacher = GetComponent<EyesAttacher>();
		_transform = transform;
		_parent = _transform.parent;

		// Saves the model offset
		_originalModelOffset = _transform.localPosition;
	}

	/// <summary>
	/// Unity's method called on the first frame this object is active.
	/// </summary>
	void Start() {
		// Gets the joint information
		_joints = GetComponentsInChildren<Joint>();
		_jointsConnectedBodies = _joints.Select(e => e.connectedBody).ToArray();
		_originalJointsPosition = _joints.Select(e => e.transform.localPosition).ToArray();

		// Resets the joints to fit the character's current scale
		float hatScale = Mathf.Lerp(minHatScale, maxHatScale, GetInterpolatedScale(_transform.lossyScale.x));
		for (int i = 0; i < _joints.Length; i++) {
			Rigidbody temp = _joints[i].connectedBody;
			_joints[i].connectedBody = null;
			_joints[i].transform.localPosition = _originalJointsPosition[i] * hatScale;
			_joints[i].transform.localRotation = Quaternion.identity;
			_joints[i].connectedBody = temp;
		}

		// Sets the right orientation to the object
		switch (initialRotation) {
			case InitialRotation.Right:
				_transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
				break;
			case InitialRotation.Left:
				_transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
				break;
		}

		// Sets the eyes, hat and deformation scale
		ScaleEyes(_parent.lossyScale.x);
		ScaleHat(_parent.lossyScale.x);
		ScaleDeformation(_parent.lossyScale.x);

		// Subscribes itself to the size's changes
		_characterSize.AddListener(this);
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Makes the object face the right direction
		float speed = _ccc.Velocity.x;
		float desiredAngle = speed > 0 ? -rotationAngle : rotationAngle;
		Quaternion desiredRotation = Quaternion.Euler(0, desiredAngle, 0);
		float turnSpeed = rotationAngularSpeed * Mathf.Abs(speed) / (_ccc.Parameters.maxSpeed * _characterSize.GetSize());
		_transform.rotation = Quaternion.RotateTowards(_transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);

		// Scales the eyes
		ScaleEyes(_parent.lossyScale.x);
	}

	/// <summary>
	/// Unity's method called at the end of each frame.
	/// </summary>
	void LateUpdate() {
		// Scales the hat
		ScaleHat(_parent.lossyScale.x);
	}
	
	/// <summary>
	/// Callback called when the character starts changing it's size.
	/// Called each frame prior to the scale change.
	/// </summary>
	/// <param name="character">The character changing size.</param>
	/// <param name="previousScale">The scale prior to the scale change.</param>
	/// <param name="nextScale">The scale after the scale change.</param>
	public void OnChangeSizeStart(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Scales the hat positions and rotations
		float hatScale = Mathf.Lerp(minHatScale, maxHatScale, GetInterpolatedScale(nextScale.x));

		// Saves the joints connected bodies and dettachs them
		for (int i = 0; i < _joints.Length; i++) {
			_joints[i].connectedBody = null;
			_joints[i].transform.localPosition = _originalJointsPosition[i] * hatScale;
			_joints[i].transform.localRotation = Quaternion.identity;
		}
	}

	/// <summary>
	/// Callback called when the character stops changing it's size.
	/// Called each frame after the scale change.
	/// </summary>
	/// <param name="character">The character changing size.</param>
	/// <param name="previousScale">The scale prior to the scale change.</param>
	/// <param name="nextScale">The scale after the scale change.</param>
	public void OnChangeSizeEnd(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Scales the deformation
		ScaleDeformation(_parent.lossyScale.x);

		// Reattachs the connected bodies
		for (int i = 0; i < _joints.Length; i++)
			_joints[i].connectedBody = _jointsConnectedBodies[i];
	}

	/// <summary>
	/// Scales the eyes to fit the parameters.
	/// </summary>
	/// <param name="scale">The current scale of the character</param>
	private void ScaleEyes(float scale) {
		float scaleFactor = GetInterpolatedScale(scale);
		_eyesAttacher.eyeScale = Mathf.Lerp(minEyeScale, maxEyeScale, scaleFactor);
		_eyesAttacher.eyeSeparation = Vector2.Lerp(minEyeSeparation, maxEyeSeparation, scaleFactor);
		_eyesAttacher.eyePenetration = Mathf.Lerp(minEyePenetration, maxEyePenetration, scaleFactor);
	}

	/// <summary>
	/// Scales the hat to fit the parameters.
	/// </summary>
	/// <param name="scale">The current scale of the character</param>
	private void ScaleHat(float scale) {
		float scaleFactor = GetInterpolatedScale(scale);
		hatRootBone.localRotation = Quaternion.Euler(Vector3.Lerp(minHatRotation, maxHatRotation, scaleFactor));
	}

	/// <summary>
	/// Scales the deformation to fit the parameters.
	/// </summary>
	/// <param name="scale">The current scale of the character</param>
	private void ScaleDeformation(float scale) {
		float scaleFactor = GetInterpolatedScale(scale);
		deformationScript.chamfScale = Mathf.Lerp(minChamfScale, maxChamfScale, scaleFactor);

		// Scales the model to scale the deformation. Simulates a "size change"
		float modelScale = Mathf.Lerp(minDeformationScale, maxDeformationScale, scaleFactor);
		_transform.localScale = new Vector3(modelScale, modelScale, modelScale);
		_transform.localPosition = _originalModelOffset * modelScale;
	}

	/// <summary>
	/// Interpolates the scale between it's minimum and maximum
	/// values and returns a normalized factor.
	/// </summary>
	/// <param name="scale">The current scale of the character</param>
	/// <returns>Normalized scale factor</returns>
	private float GetInterpolatedScale(float scale) {
		float factor = Mathf.InverseLerp(minScale, maxScale, scale);
		switch (interpolationMethod) {
			case InterpolationMethod.Linear:
				return factor;
			case InterpolationMethod.Cuadratic:
				return factor * factor;
			case InterpolationMethod.Squared:
				return Mathf.Sqrt(factor);
			default:
				Debug.LogError("Error: No valid interpolation method: " + interpolationMethod);
				return -1;
		}
	}

	/// <summary>
	/// Returns the direction the character is facing as a fraction
	/// of the maximum rotation. 1 means full right, -1 full left and
	/// 0 middle orientation.
	/// </summary>
	/// <returns>Fraction of the rotation indicating the character's looking direction</returns>
	public float GetLookingDirection() {
		float angle = _transform.eulerAngles.y;
		// If the angle is bigger than 180º, it's using the wrong sign
		if (angle > 180)
			angle -= 360;
        return -angle / rotationAngle;
	}

	#endregion
}
