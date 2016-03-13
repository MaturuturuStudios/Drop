using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour {


    void OnTriggerEnter() {

        SceneManager.LoadScene("StartScene");
    }
}
