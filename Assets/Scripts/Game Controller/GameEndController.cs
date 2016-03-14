using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndController : MonoBehaviour {

    //On trigger enter control
    private bool end = false;

    //Editable parameters
    public float waitTime = 3F;
    private float _timeElapsed = 0F;
    public Text gameOverText;
    public string phrase = "Level Complete!";

    void Start(){
        //Reset Text
        gameOverText.text = "";
    }

	void Update () {
	    if (end){
            //Wait time
            _timeElapsed += Time.deltaTime;

            //Load Menu
            if (_timeElapsed > waitTime)
                SceneManager.LoadScene("StartScene");
        }
    }

    void OnTriggerEnter(){
        // Activate counter
        end = true;

        //Show message
        gameOverText.text = phrase;
    }

}
