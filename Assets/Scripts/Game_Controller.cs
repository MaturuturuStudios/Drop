using UnityEngine;
using System.Collections;

public class Game_Controller : MonoBehaviour {

	// External references
	public GameObject character;
    

	void Start () {
        // Do nothing
       
	}

    void Update() {
        //if (character.GetComponent<Character_Shoot2>().Shooting() == false)
        //{
            // Horizontal input
            float h_input = Input.GetAxis("Horizontal");
            character.GetComponent<Character_Movement>().SetInput(h_input);

            // Jump input
            float jump_input = Input.GetAxis("Jump");
            if (jump_input > 0)
                character.GetComponent<Character_Jump>().Jump();

            //Size debug input
            DebugSize();
       // }

    }

    //TODO: must be here or in his script?
    private void DebugSize() {
        Character_Size size_drop = character.GetComponent<Character_Size>();

        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            size_drop.IncrementSize();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow)) {
            size_drop.DecrementSize();
        }

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