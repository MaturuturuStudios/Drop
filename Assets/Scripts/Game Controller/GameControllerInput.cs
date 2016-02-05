using UnityEngine;
using System.Collections;

public class GameControllerInput : MonoBehaviour {

	// External references
	public GameObject currentCharacter;
    
	void Start () {
        // Do nothing
       
	}

    void Update() {
		CharacterControllerCustomPlayer cccp = currentCharacter.GetComponent<CharacterControllerCustomPlayer>();

		// Horizontal input
		float hInput = Input.GetAxis("Horizontal");
		cccp.HorizontalInput = hInput;

        // Jump input
        float jumpInput = Input.GetAxis("Jump");
		cccp.JumpInput = jumpInput;

		//Size debug input
		DebugSize();
    }

    //TODO: must be here or in his script?
    private void DebugSize() {
		CharacterSize size_drop = currentCharacter.GetComponent<CharacterSize>();

        if(Input.GetKeyDown(KeyCode.UpArrow)) 
            size_drop.IncrementSize();

        if(Input.GetKeyDown(KeyCode.DownArrow)) 
            size_drop.DecrementSize();
        

        //Some number pressed? we use 1-9 as range 1-9
        bool done = false;
        for(int i = 1; i < 10 && !done; i++) {
            if(Input.GetKeyDown("" + i)) {
                size_drop.SetSize(i);
                done = true;
            }
        }
    }
}
