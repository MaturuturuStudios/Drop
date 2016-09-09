using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelTransitionController : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Level complete text
    /// </summary>
    public GameObject levelCompleteText;


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
    public float _fadeDuration = 0.8f;

    #endregion

    #region Public methods

    // Use this for initialization
    void Start () {

        // Get reference to audio source
        _audioSources = GetComponents<AudioSource>();

        //BeginLevelTransition(dropsCollected);

        // Calculate and set the cube to the correct size
        /*
        float _distanceToBorder = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(100));
        _distanceToBorder *= Camera.main.aspect;
        float _invertRatio = (float)Screen.height / Screen.width;
        cubeToTexturize.GetComponent<Transform>().localScale = new Vector3(_distanceToBorder , _distanceToBorder * _invertRatio, 0.1f);
        */


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
        
        yield return new WaitForSeconds(startDelay);

        // Get the camera reference
        Camera cam = Camera.main;

        // Calculate and set the cube to canvas
        float pos = (5f);

        GetComponentInChildren<RectTransform>().position = new Vector3(cam.transform.position.x + 0.24f, cam.transform.position.y, cam.transform.position.z + 5f);

        float h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;

        GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(h * cam.aspect * 1.005f, h);

        // Get the referencec to the camera
        Camera cameraTransition = Camera.main;

        // Get the canvas reference
        Canvas canvasTransition = GetComponentInChildren<Canvas>();
        //canvasTransition.sortingLayerName = "level_transition_back";

        
        
        // Instialize the message and the sound
        if (dropsGetted < maxDropsRequired) {
            //levelCompleteText.GetComponent<Text>().text = levelCompleteText1;
            _audioSources[0].clip = levelCompleteSound1;
        } else if (dropsGetted == maxDropsRequired) {
            //levelCompleteText.GetComponent<Text>().text = levelCompleteText2;
            _audioSources[0].clip = levelCompleteSound2;
        } else {
            //levelCompleteText.GetComponent<Text>().text = levelCompleteText3;
            _audioSources[0].clip = levelCompleteSound3;
            GameObject _ContactText = Instantiate(contactText, Vector3.zero, Quaternion.identity) as GameObject;
            _ContactText.transform.SetParent(canvasTransition.transform, false);
            _ContactText.transform.position = new Vector3(_ContactText.transform.position.x, _ContactText.transform.position.y - 211f, _ContactText.transform.position.z);
        }

        //if (maxDropsRequired == 1)
            //dropsHeightPosition = 0.15f;

        // Instantiate the message and the sound
        GameObject levelCompleteMessage = Instantiate(levelCompleteText, Vector3.zero, Quaternion.identity) as GameObject;
        _audioSources[0].Play();

        // Set object an UI object
        levelCompleteMessage.transform.SetParent(canvasTransition.transform, false);

        // Wait to start fading
        yield return new WaitForSeconds(1);

        // Look if player exceded max drops
        int dropsToShow = dropsGetted > maxDropsRequired ? dropsGetted : maxDropsRequired;

        // Update canvases
        Canvas.ForceUpdateCanvases();

        List<GameObject> dropsContainer = new List<GameObject>();

        float width  = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 8 * 2f) * cam.aspect;

        float startPosition = -(float)(dropsToShow - 1);

        float height = canvasTransition.GetComponent<RectTransform>().sizeDelta.y;

        for (int i = 0; i < dropsToShow; ++i) {
       
            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i *2 , Camera.main.transform.position.y + dropsHeightPosition, 0f); 

            //Debug.Log("Spawn position:" + dropCounterPosition);
            GameObject drop2DAnim = GameObject.Instantiate(dropCounter, dropCounterPosition, Quaternion.identity) as GameObject;
            dropsContainer.Add(drop2DAnim);

        }

        yield return new WaitForSeconds(0.5f);


        float increasePitch = (maxPitch - startingPitch) / dropsToShow;
        // Set start pitch and calculate the pitch to increase
        _audioSources[1].pitch = startingPitch;
        for (int i = 0; i < dropsGetted; ++i) {


            _audioSources[1].clip = countDropCollectedSound;

            // Play sound
            _audioSources[1].Play();

            Vector3 dropCounterPosition = new Vector3(Camera.main.transform.position.x + startPosition + i * 2, Camera.main.transform.position.y + dropsHeightPosition, 0f);

            //Debug.Log("Spawn position:" + dropCounterPosition);
            GameObject drop2DAnim = GameObject.Instantiate(dropCounterFilled, dropCounterPosition, Quaternion.identity) as GameObject;
            dropsContainer.Add(drop2DAnim);

            drop2DAnim.transform.LookAt(Camera.main.transform);

            GameObject dropFX = GameObject.Instantiate(appearFX, dropCounterPosition, Quaternion.identity) as GameObject;

            Destroy(dropsContainer[i]);

            // Increase pitch
            _audioSources[1].pitch += increasePitch;

            // Look for delay between drops animations
            float delayBetweenDrops = dropsGetted > maxDropsRequired ? dropCreationDuration / dropsGetted : dropCreationDuration / maxDropsRequired;
            // Wait for next animation
            yield return new WaitForSeconds(delayBetweenDrops);
        }

        GameObject parent = GameObject.Find("Temporal Objects");

        StartCoroutine(FireworksFX(true, parent));
		StartCoroutine(ConfettiFX(true, parent));


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

            GameObject drop2DAnim = GameObject.Instantiate(fireworkFX[randomInt], spawnPosition, Quaternion.identity) as GameObject;
            drop2DAnim.transform.parent = parent.transform;

            StartCoroutine(DeleteFX(3f, drop2DAnim));
            yield return new WaitForSeconds (Random.Range(0.2f, 3f));
		}
	}

    public IEnumerator ConfettiFX(bool ConfettiFXon, GameObject parent) {
        while (ConfettiFXon) {

            /*float zPos = Random.Range (0f, 15f);
			float xBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float xPos = Random.Range (-xBounce, 5f + xBounce);
			float yBounce = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * zPos * 2f;
			float yPos = Random.Range (0, yBounce);*/
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotarion = Quaternion.identity;
            if (Random.value > 0.5f) {
                spawnPosition = new Vector3(382f + -6.920013f, 8.69f + -3.419998f, 0f + 5.91f);

            } else {
                spawnPosition = new Vector3(382f + 7.920013f, 8.69f + -3.419998f, 0f + 5.91f);
                spawnRotarion = Quaternion.Inverse(Quaternion.identity);
            }
            GameObject drop2DAnim = GameObject.Instantiate(confettiCanonFX, spawnPosition, spawnRotarion) as GameObject;
            drop2DAnim.transform.parent = parent.transform;

            StartCoroutine(DeleteFX(1f, drop2DAnim));
            yield return new WaitForSeconds(Random.Range(0.1f, 3f));
        }
    }


    public IEnumerator DeleteFX(float waitTime, GameObject FX) {

        yield return new WaitForSeconds(waitTime);

        Destroy(FX);
    }

    #endregion
}
