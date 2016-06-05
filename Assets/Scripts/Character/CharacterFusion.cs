using UnityEngine;
using System.Collections.Generic;

public class CharacterFusion : MonoBehaviour {

    #region Private Attributes
    /// <summary>
    /// Size of the character
    /// </summary>
    private CharacterSize _characterSize;

    /// <summary>
    /// Independent control to create or remove drops
    /// </summary>
    private GameControllerIndependentControl _independentControl;

	/// <summary>
	/// List of fusion listeners registered to this character's fusion events.
	/// </summary>
	private List<CharacterFusionListener> _listeners = new List<CharacterFusionListener>();
    #endregion

    #region Methods
    #region Public Methods
    // Use this for initialization
    void Awake () {
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                               .GetComponent<GameControllerIndependentControl>();
        _characterSize = GetComponent<CharacterSize>();
    }

	/// <summary>
	/// Returns the size of the character.
	/// </summary>
	/// <returns>The size of the character</returns>
    public int GetSize() {
        return _characterSize.GetSize();
	}

	/// <summary>
	/// Subscribes a listener to the fusion's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(CharacterFusionListener listener) {
		if (_listeners.Contains(listener))
			return false;
		_listeners.Add(listener);
		return true;
	}

	/// <summary>
	/// Unsubscribes a listener to the fusion's events.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(CharacterFusionListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Fusion between two drops
	/// </summary>
	/// <param name="anotherDrop">The drop to be absorved</param>
	private void DropFusion(GameObject anotherDrop, ControllerColliderHit hit) {
        //always check the other drop because of a posible race condition
        //checking with the active flag, destroy method does not destroy until the end of frame
        //but this method can be called again with the same object on the same frame, just in case checking...
        if(anotherDrop == null || !anotherDrop.activeInHierarchy) {
            return;
        }

		// Notifies the listeners
		foreach (CharacterFusionListener listener in _listeners)
			listener.OnBeginFusion(this, anotherDrop, hit);

        //Get the size of the other drop
        CharacterSize otherDropSize = anotherDrop.GetComponent<CharacterSize>();
        int otherSize = otherDropSize.GetSize();

        int totalSize = otherSize + _characterSize.GetSize();

        //Change control of drop if necessary
        if(anotherDrop == _independentControl.currentCharacter || gameObject == _independentControl.currentCharacter) {
            _independentControl.SetControl(gameObject, true);
        }
        
        //store the direction of hit to spit out the drop in the correct direction
        Vector3 directionSpitDrop = hit.normal;
        directionSpitDrop.z = 0;
        if(hit.gameObject == anotherDrop) {
            directionSpitDrop *= -1;
        }

        //remove the other drop
        _independentControl.DestroyDrop(anotherDrop);

        //increment size of the actual drop
        _characterSize.SetSize(totalSize, directionSpitDrop);

		// Notifies the listeners
		foreach (CharacterFusionListener listener in _listeners)
			listener.OnEndFusion(this);

	}
    #endregion

    #region Override Methods
    /// <summary>
    /// Check if is other drop and need a fusion
    /// </summary>
    /// <param name="hit">The collision data</param>
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        //I'm always the player, is the other a player? or maybe does not exists
        if(hit.gameObject == null || !hit.gameObject.CompareTag(Tags.Player)) {
            return;
        }

        //Get the size of the other drop
        CharacterFusion otherDrop = hit.gameObject.GetComponent<CharacterFusion>();

        //check who's bigger
        int difference = otherDrop.GetSize() - _characterSize.GetSize();

        if(difference > 0) {
            otherDrop.DropFusion(gameObject, hit);
        } else {
            //I' bigger, or has equal size, so lets go with race condition
            //first called will grow up (at least, this one was called)
            DropFusion(hit.gameObject, hit);
        }
    }

    #endregion
    #endregion
}
