using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ReadWritetxt : MonoBehaviour {

    #region Private attributes

    /// <summary>
    /// List of options values
    /// </summary>
    private List<string> _listoptions = new List<string>();

    /// <summary>
    /// Retrieves the value of enviroment variable of current process
    /// </summary>
    private string _winDir = System.Environment.GetEnvironmentVariable("windir");

    /// <summary>
    /// The path of the txt file
    /// </summary>
    private string path;

    #endregion

    #region Method

    /// <summary>
    /// Fuction to add data to de list of options
    /// </summary>
    private void addListItem(string value) { this._listoptions.Add(value); }

    /// <summary>
    /// Fuction to read a file
    /// </summary>
    public void Readfile(string file_path) {

        _listoptions.Clear();

        //Read all the file and keep the data in _listoptions list
        StreamReader reader = new StreamReader(file_path); try { do { addListItem(reader.ReadLine()); } while (reader.Peek() != -1); }

        catch { addListItem("El archivo está vacío"); }

        finally { reader.Close(); }

    }

    /// <summary>
    /// Fuction to write a file
    /// </summary>
    public void Writedata(string path) {

        _listoptions.Clear();

        StreamWriter writer = new StreamWriter(path);

        for(int i=0; i < _listoptions.Capacity; i++){
            //write from the _listoptions to the file
            writer.WriteLine(_listoptions[i]);
        }
        
        writer.Close();
        //this._listoptions.Clear();
        //addListItem(_listoptions.Capacity,"Archivo escrito en " +path);
    }

    /// <summary>
    /// Fuction put new information in the _listoptions list
    /// </summary>
    public void KeepOption(string option) {
        _listoptions.Add(option);
    }

    /// <summary>
    /// Fuction to clear the _listoptions list
    /// </summary>
    public void ClearOption(){
        _listoptions.Clear();
    }

    /// <summary>
    /// Fuction to take all the _listoptions data 
    /// </summary>
    public List<string> GetOptions()
    {
        return _listoptions;
    }

    #endregion

}
