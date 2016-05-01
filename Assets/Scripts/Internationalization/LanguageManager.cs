using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class LanguageManager : ScriptableObject {
    #region Language Configuration
    /// <summary>
    /// folder of languages
    /// </summary>
    private string _folder = "Languages";
    /// <summary>
    /// prefix for debug mode
    /// </summary>
    private string _appendDebug = "$#$";
    /// <summary>
    /// Language by default
    /// </summary>
    private string _fallbackLanguage = SystemLanguage.English.ToString();

    /// <summary>
    /// Names of the supported languages
    /// </summary>
    private string[] languages={SystemLanguage.English.ToString(),
                                SystemLanguage.Spanish.ToString(),
                                SystemLanguage.Catalan.ToString()};
    #endregion

    #region Private attributes
    /// <summary>
    /// Dictionary with the keys and string
    /// </summary>
    private Dictionary<string, string> _textTable;
    /// <summary>
    /// Debug mode
    /// </summary>
    private bool _debugMode=false;
    /// <summary>
    /// Path of the stored languages
    /// </summary>
    private string _languagePath;
    /// <summary>
    /// Observers for pattern observer
    /// </summary>
    List<InterfaceLanguage> _observers;
    #endregion

    #region Singleton
    /// <summary>
    /// Static instance
    /// </summary>
    private static LanguageManager instance = null;
    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static LanguageManager Instance {
        get{
            if (instance == null) instance = CreateInstance<LanguageManager>();
            return instance;
        }
    }
    #endregion


    #region Methods
    /// <summary>
    /// Recolect all the supported languages
    /// </summary>
    public void Awake() {
        _observers = new List<InterfaceLanguage>();
        _textTable = new Dictionary<string, string>();
        _languagePath = _folder + "/";
        //Get available languages
        //CollectLanguages();
        //Set the default language
        LoadLanguage(_fallbackLanguage);
    }

    /// <summary>
    /// Set the debug mode
    /// </summary>
    /// <param name="debug"></param>
    public void DebugMode(bool debug = true) {
        _debugMode = debug;
    }

    /// <summary>
    /// Check if is debug mode
    /// </summary>
    /// <returns></returns>
    public bool IsDebugMode() {
        return _debugMode;
    }

    /// <summary>
    /// Get the text matching the key
    /// </summary>
    /// <param name="key">key of the string</param>
    /// <returns>The string, with a prefix if is on debug mode</returns>
    public string GetText(string key) {
        if (key != null && _textTable != null) {
            if (_textTable.ContainsKey(key))
                key = _textTable[key];
        }
        if (_debugMode) key = _appendDebug + key;
        return key;
    }

    /// <summary>
    /// Get all available languages
    /// </summary>
    /// <returns>List of the names of languages</returns>
    public string[] GetLanguages() {
        if (languages == null) {
            languages = new string[0];
        }
        return languages;
    }

    public void ChangeLanguage(string language) {
        LoadLanguage(language);
        //update all listeners
        foreach(InterfaceLanguage ob in _observers) {
            ob.OnChangeLanguage(instance);
        }
    }

    /// <summary>
	/// Subscribes a listener to the language changes.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(InterfaceLanguage listener) {
        if (_observers.Contains(listener))
            return false;
        _observers.Add(listener);
        return true;
    }

    /// <summary>
	/// Unsubscribes a listener to the language changes.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(InterfaceLanguage listener) {
        if (!_observers.Contains(listener))
            return false;
        _observers.Remove(listener);
        return true;
    }

    /// <summary>
    /// Load a language
    /// </summary>
    /// <param name="language">Name of language</param>
    private void LoadLanguage(string language) {
        _textTable.Clear();

        TextAsset textAsset = Resources.Load(_languagePath + language) as TextAsset;
        string allTexts = "";
        //fallback
        if (textAsset == null) textAsset = Resources.Load(_languagePath  + _fallbackLanguage) as TextAsset;

        allTexts = textAsset.text;
        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        string key, value;
        //get all keys
        for (int i = 0; i < lines.Length; i++) {
            //if there is a key and is not a comment
            if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#")) {
                //get the key
                key = lines[i].Substring(0, lines[i].IndexOf("="));
                //get the value
                value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                        lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
                //add the entry
                _textTable.Add(key, value);
            }
        }
    }

    #endregion
}
