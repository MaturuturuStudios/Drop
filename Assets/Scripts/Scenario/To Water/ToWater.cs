using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
public class ToWater : ActionPerformer
{
    #region Private Attributes

    private float height=1;

    private float oldsize,size;

    private float onguizmos_max_height;

    private Vector3 origin;
    private BoxCollider collider ;

    private bool not_change_guizmo = false;

    private bool lerping = false;

    private CharacterControllerCustom ccc;

    #endregion

    #region Public Attributes

    public float max_height=10;

    public float num_drop_needed=1;

    #endregion

    #region Methods

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    void Start()
    {
        onguizmos_max_height = max_height;
        

    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    /// 

    public void Update()
    {

        if (lerping)
        {
            if (ccc.GetComponent<CharacterSize>().GetSize() - num_drop_needed > 0)
            {
                 
                

                size = ccc.GetComponent<CharacterSize>().GetSize() - num_drop_needed;
                oldsize = ccc.GetComponent<CharacterSize>().GetSize();

                oldsize = Mathf.MoveTowards(oldsize, size, Time.deltaTime); ;
                ccc.GetComponent<CharacterSize>().SetSize((int)oldsize);


                float oldy = this.transform.position.y;
                height = Mathf.MoveTowards(height, max_height, Time.deltaTime); 

                float newy = this.transform.position.y;
                newy += newy - oldy;
                Vector3 aux;
                aux.x = this.transform.position.x;
                aux.z = this.transform.position.z;
                aux.y = height;
                this.transform.position = aux;
            }
            lerping = false;

        }
    }


    protected override void OnAction(GameObject character)
    {
        ccc = character.GetComponent<CharacterControllerCustom>();

        if (ccc != null)
        {
            lerping = true;
        }   
               // Debug.Log(" angle " + transform.eulerAngles);                  

    }



    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // float height = maxHeight - minHeight;
            Color color = Color.yellow;
            color.a = 0.25f;

            // Draws the box
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;

            collider = GetComponent<BoxCollider>();
            origin = collider.center + Vector3.up * collider.size.y / 2;

            Gizmos.DrawWireCube(new Vector3(0, max_height / 2, 0f), new Vector3(collider.size.x, max_height, 0.5f));
        }
    }

    #endregion
}
