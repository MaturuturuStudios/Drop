using UnityEngine;
using System.Collections;

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
    #endregion

    #region Methods
    #region Public Methods
    // Use this for initialization
    void Awake () {
        _independentControl = GameObject.FindGameObjectWithTag("GameController")
                               .GetComponent<GameControllerIndependentControl>();
        _characterSize = GetComponent<CharacterSize>();
    }

    public int GetSize() {
        return _characterSize.GetSize();
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

        //Get the size of the other drop
        CharacterSize otherDropSize = anotherDrop.GetComponent<CharacterSize>();
        int otherSize = otherDropSize.GetSize();

        int totalSize = otherSize + _characterSize.GetSize();

        //Change control of drop if necessary
        if(anotherDrop == _independentControl.currentCharacter || gameObject == _independentControl.currentCharacter) {
            _independentControl.SetControl(gameObject);
        }
        
        //store the direction of hit to spit out the drop in the correct direction
        Vector3 directionSpitDrop = hit.normal;
        directionSpitDrop.z = 0;
        if(hit.gameObject == anotherDrop) {
            directionSpitDrop *= -1;
        }

        //remove the other drop
        _independentControl.KillDrop(anotherDrop);

        //increment size of the actual drop
        _characterSize.SetSize(totalSize, directionSpitDrop);

    }
    #endregion

    #region Override Methods
    /// <summary>
    /// Check if is other drop and need a fusion
    /// </summary>
    /// <param name="hit">The collision data</param>
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        //I'm always the player, is the other a player? or maybe does not exists
        if(hit.gameObject == null || hit.gameObject.tag != "Player") {
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
