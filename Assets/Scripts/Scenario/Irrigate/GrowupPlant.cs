using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
public class GrowupPlant : Irrigate
{
    #region Private Attributes

    private float height=1;

    private float oldsize,size;

    private float onguizmos_max_height;

    private Vector3 origin;


    private bool lerping = false;

    private float mfY;

    #endregion

    #region Public Attributes

    public float max_height=10;

    #endregion

    #region Methods

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    void Start()
    {
        onguizmos_max_height = max_height;
        height = transform.localScale.y;

        mfY = transform.position.y - transform.localScale.y / 2.0f;
    }


    public void Update()
    {

        if (lerping)
        {
            Vector3 aux;
            aux.x = transform.position.x;
            aux.y = transform.position.y;
            aux.z = transform.position.z;

            height = Mathf.MoveTowards(height, max_height, 5 * Time.deltaTime);


            this.transform.localScale = new Vector3(1, height, 1);

            transform.position = new Vector3(transform.position.x, mfY + transform.localScale.y / 2.0f, 0);

            if (height == max_height)
            {
                lerping = false;

            }


        }
    }

    /// <summary>
    /// Unity's method called each frame.
    /// </summary>
    /// 

    protected override void OnIrrigate()
    {

        lerping = true;
    }


   



    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            float mheight = max_height - height;
            Color color = Color.yellow;
            color.a = 0.25f;

            // Draws the box
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(new Vector3(0, (height + mheight) / 2, 0f), new Vector3(0.5f, mheight, 0.5f));
        }
    }

    #endregion
}
