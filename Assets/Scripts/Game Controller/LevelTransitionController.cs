using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class LevelTransitionController : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Level complete text title congratulations
    /// </summary>
    public GameObject levelCompleteText1;

    /// <summary>
    /// Level complete text title awesome
    /// </summary>
    public GameObject levelCompleteText2;

    /// <summary>
    /// Level complete text title ay camba
    /// </summary>
    public GameObject levelCompleteText3;


    /// <summary>
    /// Text with contact info, if you complete the level with mor drops as expected
    /// </summary>
    public GameObject contactText;


    /// <summary>
    /// Music played when the level is completed with less than max drops
    /// </summary>
    public AudioClip levelCompleteSound1;


    /// <summary>
    /// Music played when the level is completed with max drops
    /// </summary>
    public AudioClip levelCompleteSound2;


    /// <summary>
    /// Music played when the level is completed with more than max drops
    /// </summary>
    public AudioClip levelCompleteSound3;


    /// <summary>
    /// Pitch setted to the first drop appeared
    /// </summary>
    public float startingPitch = 4;


    /// <summary>
    /// Max pitch to reach when all drops saved
    /// </summary>
    public float maxPitch = 8f;


    /// <summary>
    /// Height position of displayed drops counters
    /// </summary>
    [Range(-5f, 5f)]
    public float dropsHeightPosition = -0.15f;


    /// <summary>
    /// Duration when spawning drops counters
    /// </summary>
    public float dropCreationDuration = 1f;

    /// <summary>
    /// Wait before start animation
    /// </summary>
    public float startDelay = 1.1f;

    /// <summary>
    /// Distance from camera to canvas
    /// </summary>
    [Range(0, 100)]
    public float canvasDistance = 5f;

	/// <summary>
	/// List of fireworks
	/// </summary>
	public List<GameObject> fireworkFX;

	/// <summary>
	/// Conffeti effect
	/// </summary>
	public GameObject confettiCanonFX;

	/// <summary>
	/// Drop to show as counter unfilled
	/// </summary>
	public GameObject dropCounter;

	/// <summary>
	/// Text to show the number of drops saved
	/// </summary>
	public GameObject dropCounterText;

    /// <summary>
    /// Drop to show as counter filled
    /// </summary>
    public GameObject dropCounterFilled;

	/// <summary>
	/// The appear effect for counters
	/// </summary>
    public GameObject appearFX;

    #endregion

    #region Private Attributes


    /// <summary>
    /// Reference to the different audiosources
    /// </summary>
    private AudioSource[] _audioSources;


    /// <summary>
    /// Handles if the end has been skipped
    /// </summary>
    private bool _skippedEnd = false;

    /// <summary>
    /// Handles if the end animation can be skiped
    /// </summary>
    private bool _canSkip = false;

    /// <summary>
    /// the data to ask for unlock the level
    /// </summary>
    private GameControllerData _data;

    #endregion

    #region Public methods

    void Start () {

        // Get reference to audio sources
        _audioSources = GetComponents<AudioSource>();

        //Get data game and store the next level
        if (Application.isPlaying)
            _data = GameObject.FindGameObjectWithTag(Tags.GameData).GetComponent<GameControllerData>();
    }
	

    /// <summary>
    /// This method calls the apropiate animations for the level transitions
    /// </summary>
    /// <param name="dropsGetted">Number of drops collected in game</param>
    public void BeginLevelTransition(int maxDrops) {

        // Start counter animation
        StartCoroutine(LevelTransitionAnimation(maxDrops));
    }

    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="dropsGetted">Number of drops collected  in the level</param>
    public IEnumerator LevelTransitionAnimation(int maxDropsRequired) {

		// Get the refenece to the temporal objects container in the scene
		GameObject parent = GameObject.Find("Temporal Objects");

		// Get out the blur effect to cleary see the effects
        Camera.main.GetComponent<DepthOfField>().maxBlurSize = 0f;

		// Delay for allow drop leave the fustrum
        yield return new WaitForSeconds(startDelay);

        // Get the camera reference
        Camera cam = Camera.main;

		// Set the canvas in the corect position
        GetComponentInChildren<RectTransform>().position = new Vector3(cam.transform.position.x , cam.transform.position.y, cam.transform.position.z + 5f);

		// Calculate the size of the canvas and set it
		float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * canvasDistance * 2f;
        GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(h * cam.aspect * 1.005f, h);

        // Get the canvas reference
        Canvas canvasTransition = GetComponentInChildren<Canvas>();

        // Calculate saved drops
        Collider[] posibleDrops = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size / 2);
        int savedDrops = 0;
        foreach (Collider posibleDrop in posibleDrops) {
            CharacterSize sizeComponent = posibleDrop.GetComponent<CharacterSize>();
            if(sizeComponent) {
                savedDrops += sizeComponent.GetSize();
            }
        }

        //// Actualize game data
        //_data.SetLevelScore(_data.GetInfoScene(SceneManager.GetActiveScene().name), savedDrops);

        // Select the correct objects to instantiate
        GameObject levelCompleteMessage;
        if (savedDrops < maxDropsRequired) {
            levelCompleteMessage = Instantiate(levelCompleteText1, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound1;
        } else if (savedDrops == maxDropsRequired) {
            levelCompleteMessage = Instantiate(levelCompleteText2, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound2;
        } else {
            levelCompleteMessage = Instantiate(levelCompleteText3, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound3;
            GameObject _ContactText = Instantiate(contactText);
            _ContactText.transform.SetParent(canvasTransition.transform, false);
			_ContactText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        // Play the clip loaded
        _audioSources[0].Play();

        // Set object an UI object
		levelCompleteMessage.transform.SetParent(canvasTransition.transform, false);

		// Look if player exceded max drops
		int dropsToShow = savedDrops > maxDropsRequired ? savedDrops : maxDropsRequired;

        // Wait to start drops counter
        yield return new WaitForSeconds(1.5f);

		// Instantiate drops text counter
		GameObject counterText = Instantiate(dropCounterText, new Vector3(0f, 2.3f, 0f), Quaternion.identity) as GameObject;
		counterText.transform.SetParent(canvasTransition.transform, false);

		// Buld the string to show and set it to the text object
		string savedDropsString = LanguageManager.Instance.GetText("SavedDrops") + ": " + 0 + " /" + maxDropsRequired;
		counterText.GetComponent<Text> ().text = savedDropsString;

		// Create a container to drops counter
        List<GameObject> dropsContainer = new List<GameObject>();
		// Calculate metrics for the spawns of the counters
        float startPosition = -(float)(dropsToShow - 1);

        // Start drops counter spawn
        for (int i = 0; i < dropsToShow; ++i) {
       		
			// Calculate desired position
            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i *2 , Camera.main.transform.position.y + dropsHeightPosition, 0f); 

            // Instantiate the object an put it in the correct place in hierarchy
            GameObject dropCounterIns = GameObject.Instantiate(dropCounter, dropCounterPosition, Quaternion.identity) as GameObject;
			dropsContainer.Add(dropCounterIns);
			dropCounterIns.transform.parent = parent.transform;

        }

		// Wait to spawn filled counters
        yield return new WaitForSeconds(0.5f);

		// Set pitch to the minium and calculate haw much pitch we have to increase in each drop
		_audioSources[1].pitch = startingPitch;
        float increasePitch = (maxPitch - startingPitch) / dropsToShow;

		// Start drops filled counter spawn
        for (int i = 0; i < savedDrops; ++i) {

			// Actualize drops text counter 
			counterText.GetComponent<Text> ().text = LanguageManager.Instance.GetText("SavedDrops") + ": " + (i + 1f) + "/" + maxDropsRequired;

			// Play animation of text counter
			Animator couterTextAnimator = counterText.GetComponent<Animator> ();
			couterTextAnimator.SetBool ("Jump", false);
			couterTextAnimator.SetBool ("Jump", true);

			// Play sound default sound for drop counter
            _audioSources[1].Play();

            // Increase pitch
            _audioSources[1].pitch += increasePitch;

			// Calculate drop counter filled position, spawn it and add it to the container and child of it's father
            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i * 2, Camera.main.transform.position.y + dropsHeightPosition, 0f);
			GameObject dropCounterFilledIns = GameObject.Instantiate(dropCounterFilled, dropCounterPosition, Quaternion.identity) as GameObject;
			dropsContainer.Add(dropCounterFilledIns);
			dropCounterFilledIns.transform.parent = parent.transform;

			// Make drop look at the camera
			Vector3 direction = dropCounterFilledIns.transform.position - (Camera.main.transform.position - 8f * (16f / 9f - 1f) * Vector3.forward);
			direction = Vector3.ProjectOnPlane (direction, Vector3.up);
			dropCounterFilledIns.transform.rotation = Quaternion.LookRotation (direction);

			// Instantiate appear effect and order it
			GameObject appearFXIns = GameObject.Instantiate(appearFX, dropCounterPosition, Quaternion.identity) as GameObject;
			appearFXIns.transform.parent = parent.transform;
			StartCoroutine(DeleteFX(2f, appearFXIns));

			// Delete drop counter not filled
			Destroy(dropsContainer[i]);

            // Look for delay between drops animations
            float delayBetweenDrops = savedDrops > maxDropsRequired ? dropCreationDuration / savedDrops : dropCreationDuration / maxDropsRequired;
            // Wait for next animation
            yield return new WaitForSeconds(delayBetweenDrops);
        }

		// If level complete with max drops play special effects
		if (savedDrops >= maxDropsRequired) {
			StartCoroutine (FireworksFX (true, parent));
			StartCoroutine (ConfettiFX (true, parent));
		}
	}
	

    /// <summary>
    /// This method calls the apropiate firework effect for the level transitions
    /// </summary>
    /// <param name="fireFXon">Handles if fireworks can be played</param>
    /// <param name="parent">Object to set as parent</param>
	public IEnumerator FireworksFX(bool fireFXon, GameObject parent) {

		while (fireFXon) {

			// Calculate the position of the effect
			float zPos = Random.Range (1f, 15f);
			float xBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float xPos = Random.Range (-xBounce, 5f + xBounce);
			float yBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float yPos = Random.Range (yBounce / 2, yBounce);
			Vector3 spawnPosition = new Vector3 (transform.position.x + xPos, transform.position.y + yPos, zPos);

			// Get a random effect
            int randomInt = Random.Range(0, fireworkFX.Count - 1);

			// Instantiate the effect and set child of its parent
			GameObject fireworkFXIns = GameObject.Instantiate(fireworkFX[randomInt], spawnPosition, Quaternion.identity) as GameObject;
			fireworkFXIns.transform.parent = parent.transform;

			// Wait a delay to delete the effect and wait for next effect
			StartCoroutine(DeleteFX(3f, fireworkFXIns));
            yield return new WaitForSeconds (Random.Range(0.1f, 2f));
		}
	}

    /// <summary>
    /// This method calls the apropiate confetti effect for the level transitions
    /// </summary>
    /// <param name="ConfettiFXon">Handles if confetti can be played</param>
    /// <param name="parent">Object to set as parent</param>
    public IEnumerator ConfettiFX(bool ConfettiFXon, GameObject parent) {
		
        while (ConfettiFXon) {

			// Calculate position of effect and instantiate it
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotarion = Quaternion.identity;
            if (Random.value > 0.5f) {
                spawnPosition = new Vector3(Camera.main.transform.position.x + -8f, 3f, 6f);

            } else {
                spawnPosition = new Vector3(Camera.main.transform.position.x + 8f, 3f, 6f);
                spawnRotarion = Quaternion.Inverse(Quaternion.identity);
            }
			
			// Instantiate the effect and set child of its parent
			GameObject confettiCanonFXIns = GameObject.Instantiate(confettiCanonFX, spawnPosition, spawnRotarion) as GameObject;
			confettiCanonFXIns.transform.parent = parent.transform;

			// Wait a delay to delete the effect and wait for next effect
			StartCoroutine(DeleteFX(1f, confettiCanonFXIns));
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        }
	}

    /// <summary>
    /// Wait desired time to delete the gameobject effect
    /// </summary>
	public IEnumerator DeleteFX(float waitTime, GameObject FX) {

		yield return new WaitForSeconds(waitTime);

		DestroyImmediate(FX);
    }

    /// <summary>
    /// Skips the intro and sets all the object to its position
    /// </summary>
    public IEnumerator WaitMinTimeToSkip(float waitTime) {

        yield return new WaitForSeconds(waitTime);

        _canSkip = true;
    }

    /// <summary>
    /// Skips the intro and begin fade
    /// </summary>
    public void SkipEnd() {

        if (!_skippedEnd && _canSkip) {
            _skippedEnd = true;

            StartCoroutine(SkipEndCoroutine(0.8f));
        }
    }

    /// <summary>
    /// Plays the fade effect
    /// </summary>
    /// <param name="waitTime">Fade time</param>
    public IEnumerator SkipEndCoroutine(float waitTime) {
		
		// Get reference to scene fade
        SceneFadeInOut sfio = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<SceneFadeInOut>();
		
		// Start fading and wait to the end of it
        sfio.BeginFade(false, waitTime);
        yield return new WaitForSeconds(waitTime);
		
		// Load next scene
        sfio.op.allowSceneActivation = true;
    }

    #endregion
}
