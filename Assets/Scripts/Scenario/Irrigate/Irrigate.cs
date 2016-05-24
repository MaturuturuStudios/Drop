using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
abstract public class Irrigate : ActionPerformer
{
    #region Private Attributes

    /// <summary>
    /// Define the character component variable.
    /// </summary> 
    /// 
    private CharacterControllerCustom ccc;

    /// <summary>
    /// Define the bool to know that On action was pressed.
    /// </summary> 
    /// 
    private bool firstime = false;

    #endregion

    #region Public Attributes

    /// <summary>
    /// Defines the number of drops needed to make a plant grow up.
    /// </summary> 
    /// 
    public float _numDrops;

    #endregion

    #region Methods

    protected abstract void OnIrrigate();

    protected override void OnAction(GameObject character)
    {
        ccc = character.GetComponent<CharacterControllerCustom>();
        
        if (ccc != null)
        {
            if (ccc.GetComponent<CharacterSize>().GetSize() - _numDrops > 0 && !firstime)
            {
                float size = ccc.GetComponent<CharacterSize>().GetSize() - _numDrops;
                ccc.GetComponent<CharacterSize>().SetSize((int)size);
                firstime = true;
                
            }
        }

    }

    public bool enter()
    {
        return firstime;
    }


    #endregion
}
