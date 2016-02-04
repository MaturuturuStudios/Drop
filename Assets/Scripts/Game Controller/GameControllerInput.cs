using UnityEngine;
using System.Collections;

public class GameControllerInput : MonoBehaviour {

	// External references
	public GameObject current_character;
    
	void Start () {
        // Do nothing
       
	}

    void Update() {
        //if (character.GetComponent<Character_Shoot2>().Shooting() == false)
        //{
            // Horizontal input
            float h_input = Input.GetAxis("Horizontal");
			current_character.GetComponent<CharacterControllerCustomPlayer>().SetInput(h_input);

            // Jump input
            float jump_input = Input.GetAxis("Jump");
            if (jump_input > 0)
				current_character.GetComponent<CharacterControllerCustomPlayer>().Jump();

            //Size debug input
            DebugSize();
       // }

    }

    //TODO: must be here or in his script?
    private void DebugSize() {
		CharacterSize size_drop = current_character.GetComponent<CharacterSize>();

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
