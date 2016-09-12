using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class LevelTransitionController : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Level complete text
    /// </summary>
    public GameObject levelCompleteText1;

    /// <summary>
    /// Level complete text
    /// </summary>
    public GameObject levelCompleteText2;

    /// <summary>
    /// Level complete text
    /// </summary>
    public GameObject levelCompleteText3;


    /// <summary>
    /// Text with contact info
    /// </summary>
    public GameObject contactText;


    /// <summary>
    /// Sound played when the character enters shoot mode.
    /// </summary>
    public AudioClip countDropCollectedSound;


    /// <summary>
    /// Sound played when the level is completed with less than max drops
    /// </summary>
    public AudioClip levelCompleteSound1;


    /// <summary>
    /// Sound played when the level is completed with max drops
    /// </summary>
    public AudioClip levelCompleteSound2;


    /// <summary>
    /// Sound played when the level is completed with more than max drops
    /// </summary>
    public AudioClip levelCompleteSound3;


    /// <summary>
    /// Pitch setted to the first drop apeared
    /// </summary>
    public float startingPitch = 4;


    /// <summary>
    /// Max pitch to reach when all drops getted
    /// </summary>
    public float maxPitch = 8f;


    /// <summary>
    /// Drops required to complete the level 100%
    /// </summary>
    [Range(1, 5)]
    public int maxDropsRequired = 	1;


    /// <summary>
    /// Drops required to complete the level 100%
    /// </summary>
    [Range(1, 6)]
    public int dropsCollected = 1;


    /// <summary>
    /// Height position of displayed drops
    /// </summary>
    [Range(-5f, 5f)]
    public float dropsHeightPosition = -0.15f;


    /// <summary>
    /// Duration of the animation when creating drops
    /// </summary>
    public float dropCreationDuration = 1f;

    /// <summary>
    /// Wait before start animation
    /// </summary>
    public float startDelay = 1.1f;

    /// <summary>
    /// Duration of the animation when creating drops
    /// </summary>
    [Range(0, 100)]
    public float canvasDistance = 4.5f;

	/// <summary>
	/// Background to set the texture
	/// </summary>
	public List<GameObject> fireworkFX;

	/// <summary>
	/// Background to set the texture
	/// </summary>
	public GameObject confettiCanonFX;

	/// <summary>
	/// Drop to show as counter
	/// </summary>
	public GameObject dropCounter;

	/// <summary>
	/// Text to show the number of drops
	/// </summary>
	public GameObject dropCounterText;

    /// <summary>
    /// Drop to show as counter
    /// </summary>
    public GameObject dropCounterFilled;

    public Material dropMaterial;

    public GameObject appearFX;

    #endregion

    #region Private Attributes


    /// <summary>
    /// Reference to the object's original AudioSource component which
    /// the values for the new ones will be copied from.
    /// </summary>
    private AudioSource[] _audioSources;


    /// <summary>
    /// Duration while fading
    /// </summary>
    private bool _skippedEnd = false;

    /// <summary>
    /// variable to know if we can skip the end
    /// </summary>
    private bool _canSkip = false;

    #endregion

    #region Public methods

    // Use this for initialization
    void Start () {

        // Get reference to audio source
        _audioSources = GetComponents<AudioSource>();
    }
	

    /// <summary>
    /// This method calls the apropiate animations for the level transitions
    /// </summary>
    /// <param name="dropsGetted">Number of drops collected in game</param>
    public void BeginLevelTransition(int dropsGetted) {

        // Start counter animation
        StartCoroutine(LevelTransitionAnimation(dropsGetted));
    }

    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="dropsGetted">Number of drops collected  in the level</param>
    public IEnumerator LevelTransitionAnimation(int dropsGetted) {

		GameObject parent = GameObject.Find("Temporal Objects");

        Camera.main.GetComponent<DepthOfField>().maxBlurSize = 0f;

        yield return new WaitForSeconds(startDelay);

        // Get the camera reference
        Camera cam = Camera.main;

        // Calculate and set the cube to canvas
        float pos = (5f);

        GetComponentInChildren<RectTransform>().position = new Vector3(cam.transform.position.x , cam.transform.position.y, cam.transform.position.z + 5f);

        float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;

        GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(h * cam.aspect * 1.005f, h);

        // Get the referencec to the camera
        //Camera cameraTransition = Camera.main;

        // Get the canvas reference
        Canvas canvasTransition = GetComponentInChildren<Canvas>();
        //canvasTransition.sortingLayerName = "level_transition_back";


        GameObject levelCompleteMessage;
        // Instialize the message and the sound
        if (dropsGetted < maxDropsRequired) {
            levelCompleteMessage = Instantiate(levelCompleteText1, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound1;
        } else if (dropsGetted == maxDropsRequired) {
            levelCompleteMessage = Instantiate(levelCompleteText2, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound2;
        } else {
            levelCompleteMessage = Instantiate(levelCompleteText3, Vector3.zero, Quaternion.identity) as GameObject;
            _audioSources[0].clip = levelCompleteSound3;
            GameObject _ContactText = Instantiate(contactText);
            _ContactText.transform.SetParent(canvasTransition.transform, false);
			_ContactText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        //if (maxDropsRequired == 1)
            //dropsHeightPosition = 0.15f;

        // Instantiate the message and the sound
        _audioSources[0].Play();

        // Set object an UI object
        levelCompleteMessage.transform.SetParent(canvasTransition.transform, false);

        // Wait to start fading
        yield return new WaitForSeconds(1);

        // Look if player exceded max drops
        int dropsToShow = dropsGetted > maxDropsRequired ? dropsGetted : maxDropsRequired;

		yield return new WaitForSeconds(0.5f);

		GameObject counterText = Instantiate(dropCounterText, new Vector3(0f, 2.3f, 0f), Quaternion.identity) as GameObject;
		counterText.transform.SetParent(canvasTransition.transform, false);

		string gettedDropsString = LanguageManager.Instance.GetText("SavedDrops") + ": " + 0 + " /" + maxDropsRequired;

		counterText.GetComponent<Text> ().text = gettedDropsString;

        // Update canvases
        Canvas.ForceUpdateCanvases();

        List<GameObject> dropsContainer = new List<GameObject>();

        //float width  = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 8 * 2f) * cam.aspect;

        float startPosition = -(float)(dropsToShow - 1);

        //float height = canvasTransition.GetComponent<RectTransform>().sizeDelta.y;

        Debug.Log("dropsToShow:" + dropsToShow);
        for (int i = 0; i < dropsToShow; ++i) {
       
            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i *2 , Camera.main.transform.position.y + dropsHeightPosition, 0f); 

            Debug.Log("Spawn position:" + dropCounterPosition);
            GameObject dropCounterIns = GameObject.Instantiate(dropCounter, dropCounterPosition, Quaternion.identity) as GameObject;
			dropsContainer.Add(dropCounterIns);
			dropCounterIns.transform.parent = parent.transform;

        }

        yield return new WaitForSeconds(0.5f);


        float increasePitch = (maxPitch - startingPitch) / dropsToShow;
        // Set start pitch and calculate the pitch to increase
        _audioSources[1].pitch = startingPitch;
        for (int i = 0; i < dropsGetted; ++i) {

			gettedDropsString = LanguageManager.Instance.GetText("SavedDrops") + ": " + (i + 1f) + "/" + maxDropsRequired;

			Animator couterTextAnimator = counterText.GetComponent<Animator> ();
			couterTextAnimator.SetBool ("Jump", true);
			StartCoroutine(ResetCounterTextState(0.2f, couterTextAnimator));

            _audioSources[1].clip = countDropCollectedSound;

            // Play sound
            _audioSources[1].Play();

			counterText.GetComponent<Text> ().text = gettedDropsString;

            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i * 2, Camera.main.transform.position.y + dropsHeightPosition, 0f);

            //Debug.Log("Spawn position:" + dropCounterPosition);
			GameObject dropCounterFilledIns = GameObject.Instantiate(dropCounterFilled, dropCounterPosition, Quaternion.identity) as GameObject;
			dropsContainer.Add(dropCounterFilledIns);

			Vector3 direction = dropCounterFilledIns.transform.position - (Camera.main.transform.position - 8f * (16f / 9f - 1f) * Vector3.forward);
			direction = Vector3.ProjectOnPlane (direction, Vector3.up);
			dropCounterFilledIns.transform.rotation = Quaternion.LookRotation (direction);

			dropCounterFilledIns.transform.parent = parent.transform;

			GameObject appearFXIns = GameObject.Instantiate(appearFX, dropCounterPosition, Quaternion.identity) as GameObject;

			appearFXIns.transform.parent = parent.transform;

			StartCoroutine(DeleteFX(2f, appearFXIns));

			Destroy(dropsContainer[i]);

            // Increase pitch
            _audioSources[1].pitch += increasePitch;

            // Look for delay between drops animations
            float delayBetweenDrops = dropsGetted > maxDropsRequired ? dropCreationDuration / dropsGetted : dropCreationDuration / maxDropsRequired;
            // Wait for next animation
            yield return new WaitForSeconds(delayBetweenDrops);
        }


		if (dropsGetted >= maxDropsRequired) {
			StartCoroutine (FireworksFX (true, parent));
			StartCoroutine (ConfettiFX (true, parent));
		}
        yield return new WaitForSeconds(1);
	}

	public IEnumerator FireworksFX(bool fireFXon, GameObject parent) {

		while (fireFXon) {

			float zPos = Random.Range (1f, 15f);
			float xBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float xPos = Random.Range (-xBounce, 5f + xBounce);
			float yBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float yPos = Random.Range (yBounce / 2, yBounce);
			Vector3 spawnPosition = new Vector3 (transform.position.x + xPos, transform.position.y + yPos, zPos);

            int randomInt = Random.Range(0, fireworkFX.Count - 1);
            Debug.Log("Random int" + randomInt);

			GameObject fireworkFXIns = GameObject.Instantiate(fireworkFX[randomInt], spawnPosition, Quaternion.identity) as GameObject;
			fireworkFXIns.transform.parent = parent.transform;

			StartCoroutine(DeleteFX(3f, fireworkFXIns));
            yield return new WaitForSeconds (Random.Range(0.1f, 2f));
		}
	}

    public IEnumerator ConfettiFX(bool ConfettiFXon, GameObject parent) {
        while (ConfettiFXon) {

            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotarion = Quaternion.identity;
            if (Random.value > 0.5f) {
                spawnPosition = new Vector3(Camera.main.transform.position.x + -8f, 3f, 6f);

            } else {
                spawnPosition = new Vector3(Camera.main.transform.position.x + 8f, 3f, 6f);
                spawnRotarion = Quaternion.Inverse(Quaternion.identity);
            }
			GameObject confettiCanonFXIns = GameObject.Instantiate(confettiCanonFX, spawnPosition, spawnRotarion) as GameObject;
			confettiCanonFXIns.transform.parent = parent.transform;

			StartCoroutine(DeleteFX(1f, confettiCanonFXIns));
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        }
	}


	public IEnumerator DeleteFX(float waitTime, GameObject FX) {

		yield return new WaitForSeconds(waitTime);

		DestroyImmediate(FX);
    }


    public IEnumerator ResetCounterTextState(float waitTime, Animator couterTextAnimator) {

        yield return new WaitForSeconds(waitTime);

        couterTextAnimator.SetBool("Jump", false);
    }


    public IEnumerator WaitMinTimeToSkip(float waitTime) {

        yield return new WaitForSeconds(waitTime);

        _canSkip = true;
    }

    /// <summary>
    /// Skips the intro and sets all the object to its position
    /// </summary>
    public void SkipEnd() {

        if (!_skippedEnd && _canSkip) {
            _skippedEnd = true;

            StartCoroutine(SkipEndCoroutine(0.8f));
        }
    }


    public IEnumerator SkipEndCoroutine(float waitTime) {
        SceneFadeInOut sfio = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<SceneFadeInOut>();
        sfio.BeginFade(false, waitTime);
        yield return new WaitForSeconds(waitTime);
        sfio.op.allowSceneActivation = true;
    }

    #endregion
}
