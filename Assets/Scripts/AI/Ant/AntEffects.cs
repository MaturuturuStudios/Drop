using UnityEngine;

[RequireComponent(typeof(AIAnt))]
public class AntEffects : MonoBehaviour, EnemyBehaviourListener {

	public GameObject scaredEffectPrefab;

	public Transform scaredEffectTransform;

	public Color eyesIdleColor = Color.yellow;

	public Color eyesChaseColor = Color.red;

	public Color eyesScaredColor = Color.cyan;

	[Range(0, 1)]
	public float colorTransitionSpeed = 0.5f;

	public Renderer[] eyeRenderers;

	private AIAnt _ai;

	private Material[] _eyeMaterials;

	private Color _targetEyeColor;

	void Awake() {
		// Retrieves the desired components
		_ai = GetComponent<AIAnt>();
	}

	void Start() {
		// Subscribes itself to the AI events
		_ai.AddListener(this);

		// Sets the default eyes' color
		_targetEyeColor = eyesIdleColor;

		// Looks for the renderers' materials
		_eyeMaterials = new Material[eyeRenderers.Length];
        for (int i = 0; i < eyeRenderers.Length; i++) {
			// Saves a reference to the material
			_eyeMaterials[i] = eyeRenderers[i].material;
        }

		// Sets the eyes' color to the default one
		SetAllMaterialsColor(_targetEyeColor);
	}

	void Update() {
		// Lerps the eyes' color to the target one
		SetAllMaterialsColor(_targetEyeColor, colorTransitionSpeed);
	}

	/// <summary>
	/// Sets all the materials' tint color to the specified one.
	/// </summary>
	/// <param name="color">The tint color</param>
	/// <param name="speed">The speed for the eye color to change</param>
	private void SetAllMaterialsColor(Color color, float speed = 1.0f) {
		foreach (Material material in _eyeMaterials)
			SetMaterialColor(material, color);
	}

	/// <summary>
	/// Sets the material's tint color to the specified one.
	/// </summary>
	/// <param name="material">The material to tint</param>
	/// <param name="color">The tint color</param>
	/// <param name="speed">The speed for the eye color to change</param>
	private void SetMaterialColor(Material material, Color color, float speed = 1.0f) {
		material.color = Color.Lerp(material.color, color, speed);
	}

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		// Do nothing
	}

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		// Sets the eyes' color to the chase one
		_targetEyeColor = eyesChaseColor;
	}
	
	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		// Returns the eyes' color to the idle one
		_targetEyeColor = eyesIdleColor;
	}

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		// Sets the eyes' color to the scared one
		_targetEyeColor = eyesScaredColor;

		// Creates the scared prefab
		GameObject scaredEffect = Instantiate(scaredEffectPrefab, scaredEffectTransform.position, scaredEffectTransform.rotation) as GameObject;
		scaredEffect.transform.parent = scaredEffectTransform;
    }

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		// Do nothing
	}
}
