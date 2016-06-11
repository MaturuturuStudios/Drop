using UnityEngine;

/// <summary>
/// Allows an object to be irrigated, consuming drops and
/// performing a task in consequence.
/// This is an abstract class. Classes which extend this class
/// will define their own behaviour when irrigated.
/// </summary>
abstract public class Irrigate : ActionPerformer {

    #region Public Attributes

    /// <summary>
    /// Defines the number of drops needed to activate the event.
    /// </summary> 
    public int dropsNeeded;

	/// <summary>
	/// The prefab used for showing the help information on this object.
	/// </summary>
	public GameObject helpPrefab;

    #endregion

    #region Methods

	void Start() {
		// Creates the help item and adds parents it to the object
		if (helpPrefab == null)
			return;
		GameObject helpInstance = Instantiate(helpPrefab);
		helpInstance.transform.parent = transform;
		helpInstance.transform.localPosition = Vector3.zero;
    }

    protected override bool OnAction(GameObject character) {
        CharacterControllerCustom ccc = character.GetComponent<CharacterControllerCustom>();
		CharacterSize cs = character.GetComponent<CharacterSize>();
        if (ccc != null && cs != null)
            if (cs.GetSize() > dropsNeeded) {
                cs.SetSize(cs.GetSize() - dropsNeeded);
                OnIrrigate();
				return true;
            }
		return false;
	}

	/// <summary>
	/// Delegate method. Defines how will the object behave when
	/// it is irrigated. This method will only be called once.
	/// </summary>
	protected abstract void OnIrrigate();

	#endregion
}
