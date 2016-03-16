using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class GameEndController : MonoBehaviour {

    /// <summary>
    /// End Game configuration options
    /// </summary>
    public EndGame endGame;
    [System.Serializable]
	//  Movement Class
    public class EndGame {
		//wait time showing message untill scene is changed
		float waitTime = 3F;
		//UI text object reference
		Text gameOverText;
		//Displayed phrase
		string phrase = "Level Complete!";
    }
    //time elapsed since trigger called
    private float _timeElapsed = 0F;
	
    //On trigger enter control
    private bool end = false;
	
	/// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start(){
        //Reset Text
        endGame.gameOverText.text = "";
    }

    /// <summary>
    /// Update the state of the trigger
    /// </summary>
	void Update () {
		//if the scene is ended
	    if (end){
            //Wait time
            _timeElapsed += Time.deltaTime;

            //Load Menu
            if (_timeElapsed > endGame.waitTime)
                SceneManager.LoadScene("StartScene");
        }
    }

	/// <summary>
    /// Activated when player enters on the trigger
    /// </summary>
    void OnTriggerEnter(Collider other){
        //only ends game with player
        if (other.tag == "Player"){
            // Activate counter
            end = true;

            //Show message
            endGame.gameOverText.text = endGame.phrase;
        }
    }

}
