using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the orientation of the character's model.
/// </summary>
public class CharacterModelController : MonoBehaviour, CharacterSize.CharacterSizeListener {

	#region Custom Enumerations

	/// <summary>
	/// Determines wich direction will the charater be
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

	#endregion

	#region Public Attributes

	/// <summary>
	/// Determines wich direction will the charater be
	/// facing when the game starts.
	/// </summary>
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

	/// <summary>
	/// Minimum speed to start rotating.
	/// </summary>
	public float speedTolerance = 0.1f;

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

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_transform = transform;
	}

	/// <summary>
	/// Unity's method called on the first frame this object is active.
	/// </summary>
	void Start() {
		// Gets the joint information
		_joints = GetComponentsInChildren<Joint>();
		_jointsConnectedBodies = _joints.Select(e => e.connectedBody).ToArray();
		_originalJointsPosition = _joints.Select(e => e.transform.localPosition).ToArray();

		// Sets the right orientation to the object
		switch (initialRotation) {
			case InitialRotation.Right:
				_transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
				break;
			case InitialRotation.Left:
				_transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
				break;
		}

		// Subscribes itself to the size's changes
		_characterSize.AddListener(this);
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Makes the object face the right direction
		float speed = _ccc.Velocity.x;
		if (Mathf.Abs(speed) >= speedTolerance) {
			float desiredAngle = speed > 0 ? -rotationAngle : rotationAngle;
			Quaternion desiredRotation = Quaternion.Euler(0, desiredAngle, 0);
			float turnSpeed = rotationAngularSpeed * Mathf.Abs(speed) / (_ccc.Parameters.maxSpeed * _characterSize.GetSize());
			_transform.rotation = Quaternion.RotateTowards(_transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
		}
	}
	
	/// <summary>
	/// Callback called when the character starts changing it's size.
	/// Called each frame prior to the scale change.
	/// </summary>
	/// <param name="character">The character changing size.</param>
	/// <param name="previousScale">The scale prior to the scale change.</param>
	/// <param name="nextScale">The scale after the scale change.</param>
	public void OnChangeSizeStart(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Saves the joints connected bodies and dettachs them
		for (int i = 0; i < _joints.Length; i++) {
			_joints[i].connectedBody = null;
			_joints[i].transform.localPosition = _originalJointsPosition[i];
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
		// Reattachs the connected bodies
		for (int i = 0; i < _joints.Length; i++)
			_joints[i].connectedBody = _jointsConnectedBodies[i];
	}

	#endregion
}
