using UnityEngine;
using System.Collections;

public class DoorTriggerDown : MonoBehaviour {


    private bool isopen = false;
    /// <summary>
    /// Opens the door.
    /// </summary>
    public void Open()
    {
        if (!isopen)
        {
            transform.Translate(new Vector3(0, -20, 0));
            isopen = true;
        }
    }
    /// <summary>
    /// Closes the door.
    /// </summary>
    public void Close()
    {
        if (isopen)
        {
            transform.Translate(new Vector3(0, 20, 0));
            isopen = false;
        }
    }
}
