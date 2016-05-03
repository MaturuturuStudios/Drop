using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsLanguage : MonoBehaviour, SubOptionInterface {
    #region Attributes
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;
    /// <summary>
    /// Dropdown of languages
    /// </summary>
    public Dropdown languages;
    /// <summary>
    /// The selected language
    /// </summary>
    private int _selectedLanguage = -1;
    /// <summary>
    /// List of available languages
    /// </summary>
    private string[] availableLanguages;
    #endregion


    #region Methods
    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public GameObject GetPanel() {
        return gameObject;
    }
    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    public void GetFocus() {
        //select the option
        EventSystem.current.SetSelectedGameObject(languages.gameObject);
        if (title != null) {
            //mark title as panel selected
            title.color = Color.cyan;
        }
    }

    public void LoseFocus() {
        if (title != null) {
            title.color = Color.white;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void Awake() {
        if (languages == null) {
            Debug.LogError("Don't have the languages!");
        }

        //get the list of languages
        availableLanguages = LanguageManager.Instance.GetLanguages();
        int i = 0;
        //option selected by the user
        string actualLanguage = Application.systemLanguage.ToString();
        //just in case, have the english prepared
        int englishIndex = -1;
        string englishLanguage = SystemLanguage.English.ToString();
        
        //add the languages
        languages.ClearOptions();
        foreach (string language in availableLanguages) {
            if (language == actualLanguage) _selectedLanguage = i;
            if (language == englishLanguage) englishIndex = i;
            languages.options.Add(new Dropdown.OptionData(language));
            i++;
        }

        //if none selected, select the english by default
        if (_selectedLanguage < 0) _selectedLanguage = englishIndex;
        languages.value = _selectedLanguage;
        languages.RefreshShownValue();
    }

    public void LanguageChanged(Dropdown target) {
        LanguageManager.Instance.ChangeLanguage(availableLanguages[target.value]);
    }

    public void Start() {
        languages.onValueChanged.AddListener(delegate {
            LanguageChanged(languages);
        });
    }

    public void Destroy() {
        languages.onValueChanged.RemoveAllListeners();
    }
    #endregion
}
