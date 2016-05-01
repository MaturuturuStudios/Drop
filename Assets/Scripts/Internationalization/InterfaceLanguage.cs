using UnityEngine;
using System.Collections;

public abstract class InterfaceLanguage : MonoBehaviour {
    //suscribe to the language manager
	void OnEnable () {
        //Subscribe to the change language event
        LanguageManager languageManager = LanguageManager.Instance;
        languageManager.AddListener(this);
    }

    public void OnDisable() {
        //Unsuscribe
        LanguageManager languageManager = LanguageManager.Instance;
        languageManager.RemoveListener(this);
    }

    public abstract void OnChangeLanguage(LanguageManager languageManager);
}
