using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
abstract public class Irrigate : ActionPerformer
{
    #region Private Attributes


    private CharacterControllerCustom ccc;



    #endregion

    #region Public Attributes


    #endregion

    #region Methods


    abstract protected override void OnAction(GameObject character);


    #endregion
}
