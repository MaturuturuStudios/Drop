using UnityEngine;
using UnityEngine.UI;

public class TextInternationalizationMesh : InterfaceLanguage {
    /// <summary>
    /// Key for the string
    /// </summary>
    public string keyText;
    /// <summary>
    /// Add to the last of the key text
    /// eg: keytext = "level"
    /// added: " 1"
    /// result: "level 1"
    /// </summary>
    public string addedString;
    /// <summary>
    /// The text in which will write
    /// </summary>
	private TextMesh textObject;

    void Start() {
        textObject = this.GetComponent<TextMesh>();

        //Run the method one first time
		LanguageManager languageManager = LanguageManager.Instance;
        OnChangeLanguage(languageManager);
    }

    public override void OnChangeLanguage(LanguageManager languageManager) {
		if(textObject!=null)
        	textObject.text = LanguageManager.Instance.GetText(keyText) + addedString;
    }
}
