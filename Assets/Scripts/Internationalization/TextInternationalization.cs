using UnityEngine;
using UnityEngine.UI;

public class TextInternationalization : InterfaceLanguage {
    public string keyText;

	private Text textObject;

    void Start() {
        textObject = this.GetComponent<Text>();

        //Run the method one first time
        LanguageManager languageManager = LanguageManager.Instance;
        OnChangeLanguage(languageManager);
    }

    public override void OnChangeLanguage(LanguageManager languageManager) {
        textObject.text = LanguageManager.Instance.GetText(keyText);
    }
}
