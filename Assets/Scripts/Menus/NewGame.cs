using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour {

	public void LoadSceneNewGame(){
        SceneManager.LoadScene("Test Scene", LoadSceneMode.Single);
    }
}
