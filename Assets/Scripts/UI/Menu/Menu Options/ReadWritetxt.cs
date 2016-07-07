using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ReadWritetxt : MonoBehaviour {

    #region Private attributes

    /// <summary>
    /// List of options values
    /// </summary>
    private Dictionary<string,string> _listoptions = new Dictionary<string, string>;

    /// <summary>
    /// Retrieves the value of enviroment variable of current process
    /// </summary>
    //private string _winDir = System.Environment.GetEnvironmentVariable("windir");

    #endregion

    #region Public attributes

    /// <summary>
    /// The path of the txt file
    /// </summary>
    public string path= "Assets/Scripts/UI/Menu/Menu Options/menu_options.txt";

    #endregion

    #region Method

    /// <summary>
    /// Fuction to initialite
    /// </summary>
    public void Awake(){

        //read the options file and keep the information in the dictionary _listoptions
        Readfile();

    }

    /// <summary>
    /// Fuction to add data to de list of options
    /// </summary>
    private void addListItem(string key,string value) { this._listoptions.Add(key,value); }

    /// <summary>
    /// Fuction to read a file
    /// </summary>
    public void Readfile() {

        string line;
        // Create a new StreamReader, tell it which file to read and what encoding the file
        // was saved as
        StreamReader theReader = new StreamReader(path);
        // Immediately clean up the reader after this block of code is done.
        // You generally use the "using" statement for potentially memory-intensive objects
        // instead of relying on garbage collection.
        // (Do not confuse this with the using directive for namespace at the 
        // beginning of a class!)
        using (theReader) {
            // While there's lines left in the text file, do this:
            do {
                line = theReader.ReadLine();

                if (line != null){
                    // Do whatever you need to do with the text line, it's a string now
                    // In this example, I split it into arguments based on comma
                    // deliniators, then send that array to DoStuff()
                    string[] entries = line.Split(',');
                    if (entries.Length > 0)
                        addListItem(entries[0], entries[1]);
                }
            }
            while (line != null);
            // Done reading, close the reader and return true to broadcast success    
            theReader.Close();
        }

    }

    /// <summary>
    /// Fuction to write a file
    /// </summary>
    public void Writedata() {
        StreamWriter writer = new StreamWriter(path);

        foreach (KeyValuePair<string, string> entry in _listoptions){

            //write from the _listoptions to the file
            writer.WriteLine(entry.Key+","+entry.Value);
        }
        
        writer.Close();
        //this._listoptions.Clear();
        //addListItem(_listoptions.Capacity,"Archivo escrito en " +path);
    }

    /// <summary>
    /// Fuction put new information in the _listoptions list if the option exist put the new value if not create the new value
    /// </summary>
    public void KeepOption(string key,string option) {
        if (_listoptions.ContainsKey(key))
            _listoptions[key] = option;
        else _listoptions.Add(key, option);
    }

    /// <summary>
    /// Fuction to clear the _listoptions list
    /// </summary>
    public void ClearOption(){
        _listoptions.Clear();
    }

    /// <summary>
    /// Fuction to take the valuee of a key given
    /// </summary>
    public string GetOptions(string key){
        return _listoptions[key];
    }

    #endregion

}
