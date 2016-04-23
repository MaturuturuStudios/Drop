using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the position where characters will appear grounded
/// </summary>
public class CharacterSpawner : MonoBehaviour {

    #region Public Attributes

    /// <summary>
    /// Variable indicates if character is under controll
    /// </summary>
    public bool controlled = false;

    /// <summary>
    /// Variable indicates if character is current character controller
    /// </summary>
    public bool addToControlList = false;

    /// <summary>
    /// Variable indicates the size of the created character
    /// </summary>
    [Range(1, 20)]
    public int size = 1;

    #endregion


    #region Private Attributes

    /// <summary>
    /// Variable indicates the max controlled distance to ground a character
    /// </summary>
    private float _maxDistance = 100F;

    /// <summary>
    /// Position where character will be spawned
    /// </summary>
    private Vector3 _spawnPosition;

    /// <summary>
    /// Reference to independent cotrol to create the objects
    /// </summary>
    private GameControllerIndependentControl _independentControl;

    #endregion


    #region Methods

    /// <summary>
    /// Unity's method called as soon as this entity is created.
    /// It will be called even if the entity is disabled.
    /// </summary>
    public void Awake() {

        // If we can ground drop
        if (CalculateSpawnPos() && CheckControl()) {

            // Get reference to independent control
            _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();

            // Create the character
            GameObject drop = _independentControl.CreateDrop(controlled, addToControlList);

            // Set the character to its size
            drop.GetComponent<CharacterSize>().SetSize(size);

            // Set character to its position
            drop.transform.position = _spawnPosition;
        }

        Object.Destroy(this.gameObject);
    }
    
    

    /// <summary>
    /// Unity's method called by the editor in order to draw the gizmos.
    /// Draws the path on the editor.
    /// </summary>
    public void OnDrawGizmos() {

        if (!Application.isPlaying && CalculateSpawnPos() && CheckControl()) {

            // Set the color
            Color color = new Color(0.5f, 1, 1, 0.75f);
            Gizmos.color = color;

            // Draw a sphere in character appear position
            if (_spawnPosition != Vector3.zero) {
                Vector3 gizmoSpawnPos = _spawnPosition;
                gizmoSpawnPos.y += size * 0.65f / 2;
                Gizmos.DrawSphere(gizmoSpawnPos, size * 0.625f);
            }
        }
    }


    /// <summary>
    /// Checks if the values have been inserted correctly and calculate the spawn position
    /// </summary>
    /// <returns>If values are correctly inserted</returns>
    private bool CalculateSpawnPos() {

        bool res = false;

        // Force Spawn in z = 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // Force Spawn in z = 0
        if (controlled)
            addToControlList = true;

        // Get the position
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, _maxDistance, LayerMask.GetMask("Scene"))) {

            // Get hit position
            _spawnPosition = hitInfo.point;

            // Cast size to match with drop size
            _spawnPosition.y += size * 0.65f / 2;

            // Set result value
            res = true;

        } else {
            Debug.LogWarning("Spawn can't ground drop.");
        }
        
        return res;
    }



    /// <summary>
    /// Checks if there is a character setted as controller
    /// </summary>
    /// <returns>If there is a character setted as controller</returns>
    private bool CheckControl() {

        // Validate there allmost one drop under controll
        int found = 0;

        // Look for characters setted as controlled
        CharacterSpawner[] spawners = (CharacterSpawner[])FindObjectsOfType(typeof(CharacterSpawner));
        for (int i = 0; i < spawners.Length; ++i)
            if (spawners[i].controlled == true) ++found;

        // Check for invalid values
        if (found == 0)
            Debug.LogWarning("There isn't any character setted as controlled, please mark one as controlled.");

        if (found > 1)
            Debug.LogWarning("There are more than one character setted as controlled, please mark only one as controlled.");

        return found == 1;
    }





    #endregion
}
