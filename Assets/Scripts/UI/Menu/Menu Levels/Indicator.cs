using UnityEngine;

public enum StateIndicator {
    NON_AVAILABLE, //non available level -> black
    NON_UNLOCKED, //available but not unlocked level -> gray
    SELECTED, //available, unlocked and selected level -> normal color breathing
    NORMAL //available and unlocked but not selected level -> normal color
}

public class Indicator : MonoBehaviour {
    #region Public attributes
	/// <summary>
	/// The brightness emission.
	/// </summary>
	public Color brightnessEmission;
	/// <summary>
	/// The darkness emission.
	/// </summary>
	public Color darknessEmission;
    /// <summary>
	/// The time breathing. (one cycle of the loop)
    /// </summary>
    public float ratioBreathing = 0.7f;
	/// <summary>
	/// The function to breath. needs to start on zero and finish on zero
	/// </summary>
	public AnimationCurve easeFunctionBreathing;
    /// <summary>
    /// Color for the non available levels
    /// </summary>
    public Material nonAvailableLevelMaterial;
    /// <summary>
    /// Color for the locked levels
    /// </summary>
    public Material lockedLevelMaterial;
    /// <summary>
    /// Particle effects when the level is the last unlocked
    /// </summary>
    public GameObject particleLastUnlocked;
    #endregion

    #region Private attributes
    /// <summary>
    /// reference to the material instance
    /// </summary>
    private MeshRenderer materialInstance;
    /// <summary>
    /// store original material
    /// </summary>
    private Material originalMaterial;
    /// <summary>
    /// Store original emission color
    /// </summary>
    private Color originalEmissionColor;
    /// <summary>
    /// The instantiated particles of last unlocked level
    /// </summary>
    private GameObject instantiatedParticleUnlocked=null;
    /// <summary>
    /// Store actaul state
    /// </summary>
    private StateIndicator _actualState=StateIndicator.NORMAL;
    /// <summary>
    /// Keep progess of the time breathing
    /// </summary>
    private float _breathing=0;
    #endregion

    #region Methods
    // Use this for initialization
    public void Awake() {
        materialInstance = GetComponentInChildren<MeshRenderer>(true);
        originalMaterial = materialInstance.material;
        originalEmissionColor = materialInstance.material.GetColor("_EmissionColor");
    }
	
	// Update is called once per frame
	public void Update () {
        //update breathing
        if (_actualState == StateIndicator.SELECTED) {
			_breathing += Time.unscaledDeltaTime * ratioBreathing;
			float percentage = Mathf.PingPong (_breathing, 1);
			Color finalColor = Color.Lerp (darknessEmission, brightnessEmission, easeFunctionBreathing.Evaluate(percentage));
			materialInstance.material.SetColor("_EmissionColor", finalColor);

        } else if (_breathing != 0 ) {
            //quit breathing
            _breathing = 0;
            materialInstance.material.SetColor("_EmissionColor", originalEmissionColor);
        }
    }

    /// <summary>
    /// Do the module
    /// </summary>
    /// <param name="num"></param>
    /// <param name="div"></param>
    /// <returns></returns>
	private float mod(float num, float div){
		float ratio = num / div;
		return div * (ratio - Mathf.Floor(ratio));
	}

    /// <summary>
    /// Normalize value
    /// </summary>
    /// <param name="val"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <returns></returns>
	private float normalize(float val, float max, float min){
		float range = max - min;
		float cycle = 2 * range;
		float state = mod(val-min, cycle);

		if (state > range)
			state = cycle-state;

		return state + min;
	}

    /// <summary>
    /// Set if is the last unlocked level or not and react to it
    /// </summary>
    /// <param name="lastUnlocked"></param>
    public void setLastUnlockedLevel(bool lastUnlocked=false) {
        if (!lastUnlocked && instantiatedParticleUnlocked == null) return;

        if (!lastUnlocked && instantiatedParticleUnlocked != null) {
            //stop particles
            ParticleSystem[] subSystems = instantiatedParticleUnlocked.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < subSystems.Length; i++) {
                ParticleSystem subSystem = subSystems[i];
                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;
            }
        }

        if (instantiatedParticleUnlocked == null) {
            //create the particles
            instantiatedParticleUnlocked = Instantiate(particleLastUnlocked);
			Vector3 localPosition = instantiatedParticleUnlocked.transform.localPosition;
			Quaternion localRotation = instantiatedParticleUnlocked.transform.localRotation;
			Vector3 localScale = instantiatedParticleUnlocked.transform.localScale;
            instantiatedParticleUnlocked.transform.parent = this.transform;
			instantiatedParticleUnlocked.transform.localPosition=localPosition;
			instantiatedParticleUnlocked.transform.localScale=localScale;
			instantiatedParticleUnlocked.transform.localRotation=localRotation;

            //initialize particle system
            ParticleSystem[] subSystems = instantiatedParticleUnlocked.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < subSystems.Length; i++) {
                ParticleSystem subSystem = subSystems[i];
                subSystem.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
                subSystem.Simulate(0, true, true);
                subSystem.Play();


                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = false;
            }
        }

        if (lastUnlocked) {
            //emmit particles
            ParticleSystem[] subSystems = instantiatedParticleUnlocked.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < subSystems.Length; i++) {
                ParticleSystem subSystem = subSystems[i];
                ParticleSystem.EmissionModule emission = subSystem.emission;
                emission.enabled = true;
            }
        }
    }

    /// <summary>
    /// Set the state of the indicator
    /// </summary>
    /// <param name="state"></param>
    public void SetState(StateIndicator state) {
        _actualState = state;
        switch (state) {
            case StateIndicator.NON_AVAILABLE:
                materialInstance.material = nonAvailableLevelMaterial;
                break;
            case StateIndicator.NON_UNLOCKED:
                materialInstance.material = lockedLevelMaterial;
                break;
            case StateIndicator.SELECTED:
            case StateIndicator.NORMAL:
                materialInstance.material = originalMaterial;
                break;
            default:
                break;
        }
    }
    #endregion
}
