using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Control all about the main menu
/// </summary>
public class MainMenu : MonoBehaviour {
	#region Public Atributes
	/// <summary>
	/// The option to be selected
	/// </summary>
	public GameObject firstSelected;
    /// <summary>
    /// The brush behind the selection
    /// </summary>
    public GameObject brushSelection;

    /// <summary>
    /// Time of brush traveling between selections
    /// </summary>
    public float timeTransitionBrush = 0.8f;

    /// <summary>
    /// Minimum widht of the brush including the margin
    /// </summary>
    public float minimumWidth = 280;
    /// <summary>
    /// Extra margin of the brush with the size of the selected option
    /// </summary>
    public float marginWidth = 120;


    /// <summary>
    /// Start time of the smooth movement brush
    /// </summary>
    private float _startTime=0;
    /// <summary>
    /// The final position of the brush to reach
    /// </summary>
    private Vector3 _finalPosition;

    /// <summary>
    /// Final scale the brush should be
    /// </summary>
    private Vector2 _finalScale;
	#endregion

	#region Private Attributes
	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
	#endregion

	#region Methods

	public void OnEnable() {
        _selectOption = true;
        
    }

	public void Update() {
		if (_selectOption) {
			_selectOption = false;
			//select the option
			EventSystem.current.SetSelectedGameObject(firstSelected);
            //set the brush here to give time the UI to set all his variables like size
            _finalPosition = firstSelected.GetComponent<RectTransform>().position;
            _finalScale = firstSelected.GetComponent<RectTransform>().sizeDelta;
        }

        //get the delta time
        float deltaTime = (Time.unscaledTime - _startTime) / timeTransitionBrush;

        //Get the actual and final position, and calculate the actual position
        Vector3 position = brushSelection.transform.position;
        position.y = Mathf.SmoothStep(position.y, _finalPosition.y, deltaTime);
        brushSelection.transform.position = position;

        //Get the actual and final size and calculate the actual size
        Vector2 size = brushSelection.GetComponent<RectTransform>().sizeDelta;
        size.x = Mathf.SmoothStep(size.x, _finalScale.x+marginWidth, deltaTime);
        if (size.x < minimumWidth) size.x = minimumWidth;
        brushSelection.GetComponent<RectTransform>().sizeDelta = size;

    }

    /// <summary>
    /// Update the position and size of the brush selection
    /// with the position and size of the given element (with its rect transform)
    /// </summary>
    /// <param name="selection"></param>
    public void MoveBrushSelection(GameObject selection) {
        RectTransform rect = selection.GetComponent<RectTransform>();
        _finalPosition = rect.position;
        _startTime = Time.unscaledTime;
        _finalScale = rect.sizeDelta;
    }
	
	#endregion

}
