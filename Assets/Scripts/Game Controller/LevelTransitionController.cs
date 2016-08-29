using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    /// The animation for the max drop indicator
    /// </summary>
    public GameObject dropShape;


    /// <summary>
    /// The animation of one drop collected
    /// </summary>
    public GameObject dropCounterUnitCollected;


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
    /// Text displayed when the level is completed with less than max drops
    /// </summary>
    public string levelCompleteText1;


    /// <summary>
    /// Text displayed when the level is completed with max drops
    /// </summary>
    public string levelCompleteText2;


    /// <summary>
    /// Text displayed when the level is completed with more than max drops
    /// </summary>
    public string levelCompleteText3;


    /// <summary>
    /// Drops required to complete the level 100%
    /// </summary>
    [Range(1, 5)]
    public int maxDropsRequired = 1;


    /// <summary>
    /// Drops required to complete the level 100%
    /// </summary>
    [Range(1, 6)]
    public int dropsCollected = 1;


    /// <summary>
    /// Height position of displayed drops
    /// </summary>
    [Range(-.5f, .5f)]
    public float dropsHeightPosition = -0.15f;


    /// <summary>
    /// Duration of the animation when creating drops
    /// </summary>
    public float dropCreationDuration = 1f;

    /// <summary>
    /// Wait before start animation
    /// </summary>
    public float startDelay = 1f;

    /// <summary>
    /// Duration of the animation when creating drops
    /// </summary>
    [Range(0, 100)]
    public float canvasDistance = 4.5f;

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

        BeginLevelTransition(dropsCollected);
    }
	
	// Update is called once per frame
	void Update () {
	
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

        // Get the referencec to the camera
        Camera cameraTransition = this.gameObject.transform.GetChild(0).GetComponent<Camera>();

        // Get the canvas reference
        Canvas canvasTransition = GetComponent<Canvas>();
        canvasTransition.sortingLayerName = "level_transition_back";

        // Instialize the message and the sound
        if (dropsGetted < maxDropsRequired) {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText1;
            _audioSources[0].clip = levelCompleteSound1;
        } else if (dropsGetted == maxDropsRequired) {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText2;
            _audioSources[0].clip = levelCompleteSound2;
        } else {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText3;
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
        yield return new WaitForSeconds(startDelay);

        // Look if player exceded max drops
        int dropsToShow = dropsGetted > maxDropsRequired ? dropsGetted : maxDropsRequired;

        // Update canvases
        Canvas.ForceUpdateCanvases();


        for (int i = 0; i < dropsToShow; ++i) {

            // Instantiate the shape
            GameObject drop2DAnim = GameObject.Instantiate(dropShape);

            // Set animation UI Component
            drop2DAnim.transform.SetParent(canvasTransition.transform, false);
            drop2DAnim.transform.SetAsLastSibling();

            // Calculate size of animation
            float width = canvasTransition.GetComponent<RectTransform>().sizeDelta.x;
            // We use this patron BDB / BDBDB / BDBDBDB Allways blank posiotions at sides and the same size for drops and blanks Position
            float animSize = width / (dropsToShow + ((dropsToShow - 1) / 2) + 2);

            // Set size of animation
            drop2DAnim.GetComponent<RectTransform>().sizeDelta = new Vector2(animSize, animSize);

            // Set position of animation
            float horizontalPosition = (animSize / 2) + animSize + (animSize * i) + (animSize * i / 2);
            if (dropsToShow % 2 == 0)
                horizontalPosition -= animSize / 4;
            drop2DAnim.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalPosition, Screen.height * dropsHeightPosition);
        }

        yield return new WaitForSeconds(1);


        // Set start pitch and calculate the pitch to increase
        _audioSources[1].pitch = startingPitch;
        float increasePitch = (maxPitch - startingPitch) / dropsToShow;

        for (int i = 0; i < dropsGetted; ++i) {

            GameObject drop2DAnim = GameObject.Instantiate(dropCounterUnitCollected);
            _audioSources[1].clip = countDropCollectedSound;

            // Play sound
            _audioSources[1].Play();

            // Set animation UI Component
            drop2DAnim.transform.SetParent(canvasTransition.transform, false);

            drop2DAnim.transform.SetAsLastSibling();

            // Calculate size of animation
            float width = canvasTransition.GetComponent<RectTransform>().sizeDelta.x;
            // We use this patron BDB / BDBDB / BDBDBDB Allways blank posiotions at sides and the same size for drops and blanks Position
            float animSize = width / (dropsToShow + ((dropsToShow - 1) / 2) + 2);

            // Set size of animation
            drop2DAnim.GetComponent<RectTransform>().sizeDelta = new Vector2(animSize, animSize);

            // Set position of animation
            float horizontalPosition = (animSize / 2) + animSize + (animSize * i) + (animSize * i / 2);
            if (dropsToShow % 2 == 0)
                horizontalPosition -= animSize / 4;
            drop2DAnim.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalPosition, Screen.height * dropsHeightPosition);

            // Increase pitch
            _audioSources[1].pitch += increasePitch;

            // Look for delay between drops animations
            float delayBetweenDrops = dropsGetted > maxDropsRequired ? dropCreationDuration / dropsGetted : dropCreationDuration / maxDropsRequired;
            // Wait for next animation
            yield return new WaitForSeconds(delayBetweenDrops);
        }
    }


        #endregion
    }
