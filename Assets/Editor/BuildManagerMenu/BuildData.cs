using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Data structure for save build data into a file and recover it
/// </summary>
public class BuildData {

    /// <summary>
    /// The name of this release
    /// </summary>
    public string buildName;

    /// <summary>
    /// Path where aplication will work on
    /// </summary>
    public string workPath;

    /// <summary>
    /// check it it shares the build to a target parth
    /// </summary>
    public bool share;

    /// <summary>
    /// Path where aplication will share the zip package
    /// </summary>
    public string sharedFolderPath;

    /// <summary>
    /// Path where aplication will find zip exe
    /// </summary>
    public string zipPath;

    /// <summary>
    /// True when build for Windows is active
    /// </summary>
    public bool win;

    /// <summary>
    /// True when build for Linux is active
    /// </summary>
    public bool lin;

    /// <summary>
    /// True when build for Mac is active
    /// </summary>
    public bool mac;

    /// <summary>
    /// List of the scenes that will be included into de build
    /// </summary>
    public List<string> scenes;


    /// <summary>
    /// Default constructor with default setted values
    /// </summary>
    public BuildData() {
        buildName = "drop-v";
        workPath = "../../Builds/Drop";
        sharedFolderPath = "../../Google Drive/WorkSpace/DropOfficial/Realeases";
        share = false;
        zipPath = "../../Program Files (x86)/GnuWin32/bin";
        win = true;
        lin = true;
        mac = true;
        scenes = new List<string>();
    }

    /// <summary>
    /// Saves the configuration data structure to a file using XML
    /// </summary>
	/// <param name="bd">BuildData structure to save in the file</param>
    public static void Save(BuildData bd) {

        // Create a serializer
        XmlSerializer serializer = new XmlSerializer(typeof(BuildData));

        // Save the file in "Application.streamingAssetsPath" folder
        using (FileStream stream = new FileStream(Application.streamingAssetsPath + "/build.conf", FileMode.Create)) {
            serializer.Serialize(stream, bd);
            Debug.Log("Data saved");
        }
    }

    /// <summary>
    /// Load data from XML structure file to a configuration data structure and return it
    /// </summary>
    /// <returns>BuildData object with data found in the file, null if it fails</returns>
    public static BuildData Load() {

        // Return variable
        BuildData bd = null;

        // Look if the file config exist
        if (File.Exists(Application.streamingAssetsPath + "/build.conf")) {

            // Create a serializer
            XmlSerializer serializer = new XmlSerializer(typeof(BuildData));

            //read the file from "Application.streamingAssetsPath" folder
            using (FileStream stream = new FileStream(Application.streamingAssetsPath + "/build.conf", FileMode.Open)) {
                bd = (BuildData)serializer.Deserialize(stream);
                Debug.Log("Data readed");
            }
        }

        // Return result
        return bd;
    }
}