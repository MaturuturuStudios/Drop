using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class BuildData {
   // [XmlAttribute("BuildName")]
    public string buildName;

   // [XmlAttribute("Path")]
    public string path;

   // [XmlAttribute("Windows")]
    public bool win;

    //[XmlAttribute("Linux")]
    public bool lin;

   // [XmlAttribute("Mac")]
    public bool mac;

    //[XmlAttribute("Scenes")]
    public List<string> scenes;

    public int test;

    public BuildData() {
        buildName = "drop-v";
        path = "D:\\Google Drive\\WorkSpace\\DropOfficial\\Realeases";
        win = true;
        lin = true;
        mac = true;
        scenes = new List<string>();
    }

    public BuildData(string buildName, string path, bool win, bool lin, bool mac) {
        this.buildName = buildName;
        this.path = path;
        this.win = win;
        this.lin = lin;
        this.mac = mac;
        this.scenes = new List<string>();
    }

    public BuildData(string buildName, string path, bool win, bool lin, bool mac, List<string> scenes) {
        this.buildName = buildName;
        this.path = path;
        this.win = win;
        this.lin = lin;
        this.mac = mac;
        this.scenes = scenes;
    }

    public void SetConfig(string buildName, string path, bool win, bool lin, bool mac) {
        this.buildName = buildName;
        this.path = path;
        this.win = win;
        this.lin = lin;
        this.mac = mac;
    }

    public void SetScenes(List<string> scenes) {
        this.scenes = scenes;
    }

    public static void Save(BuildData bdc) {

        XmlSerializer serializer = new XmlSerializer(typeof(BuildData));
        using (FileStream stream = new FileStream(Application.persistentDataPath + "/build.conf", FileMode.Create)) {
            serializer.Serialize(stream, bdc);
            Debug.Log("Data saved");
        }
    }

    public static BuildData Load() {

        BuildData bd = null;
        if (File.Exists(Application.persistentDataPath + "/build.conf")) {

            XmlSerializer serializer = new XmlSerializer(typeof(BuildData));
            using (FileStream stream = new FileStream(Application.persistentDataPath + "/build.conf", FileMode.Open)) {
                bd = (BuildData)serializer.Deserialize(stream);
                Debug.Log("Data readed");
            }

        }
        return bd;
    }
}