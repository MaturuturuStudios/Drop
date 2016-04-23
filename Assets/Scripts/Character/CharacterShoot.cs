using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class active, desactive shoot-mode and shoot a drop 
/// </summary>
public class CharacterShoot : MonoBehaviour {

    #region Private Attributes

    /// <summary>
	/// Defines the object that will use to create the drop shooted.
	/// </summary> 
    private GameObject ball;
    /// <summary>
	/// Defines the size of the drop shooted.
	/// </summary> 
    private float sizeshot = 1;

    #endregion

    #region Public Attributes

    /// <summary>
	/// Defines the boolean to know if we are in shootmode or out of shootmode.
	/// </summary> 
    public bool shootmode = false;

    /// <summary>
    /// Defines the scripts objects that we will use it.
    /// </summary> 
    CharacterController c;
    CharacterControllerCustom ccc;
    CharacterShootTrajectory st;
    GameControllerIndependentControl _gcic;
    CharacterSize size;

    #endregion

    #region Methods

    /// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
    void Awake (){
        c = GetComponent<CharacterController>();
        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        size= GetComponent<CharacterSize>();

        _gcic = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();

       
    }

    /// <summary>
	/// Method to know if if we are in shootmode or not.
	/// </summary>
    public bool isShooting(){
        return shootmode;
    }

    /// <summary>
	/// Method to increase the size of the drop shooted
	/// </summary>
    public void IncreaseSize() {
        if ((shootmode == true))
        {
            float oldsize;

            oldsize = sizeshot;
            sizeshot++;
            if (sizeshot < GetComponent<CharacterSize>().GetSize())
                st.selectingsize(sizeshot);
            else sizeshot = oldsize;
        }
    }

    /// <summary>
	/// Method to decrease the size of the drop shooted
	/// </summary>
    public void DecreaseSize()
    {
        if ((shootmode == true))
        {
            float oldsize;
      
            oldsize = sizeshot;
            sizeshot--;
            
            if (sizeshot > 0)
                st.selectingsize(sizeshot);
            else sizeshot = oldsize;
        }
    }

    /// <summary>
	/// Method to start the shootmode
	/// </summary>
    public void Aim(){
        if ( ccc.State.IsGrounded == true && (GetComponent<CharacterSize>().GetSize() > 1) && (GetComponent<CharacterSize>().GetSize()<10) )
        {
            if (shootmode == false)
            {
                shootmode = true;
                st.enabled = true;
                sizeshot = 1;
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;
            }
            else if ((shootmode == true))
            {
                shootmode = false;
                st.QuitTrajectory();
                st.enabled = false;
                ccc.Parameters = null;
            }
        }


    }

    /// <summary>
	/// Method to shoot the drop
	/// </summary>
    public void Shoot()
    {
        if ( (shootmode == true))
        {
            ccc.Parameters = null;
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;
            GetComponent<CharacterSize>().SetSize((int)(GetComponent<CharacterSize>().GetSize()-sizeshot));

            throwBall();          
        }
    }

    /// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update (){

        //check if we shouldn't be in shootmode
        if ((shootmode== true) && (ccc.State.IsGrounded == false || size.GetSize()==1 ))
        {
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;
            ccc.Parameters = null;                      
        }
     }

    /// <summary>
	/// Method to prepare the drop to be shooted.
	/// </summary>
    private void prepareDropToFly()
    {
        ball.transform.position = this.transform.position + st.GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + ball.GetComponent<CharacterController>().radius * ball.transform.lossyScale.x);
	    ball.SetActive(false);
    }

    /// <summary>
	/// Method to shoot the drop.
	/// </summary>
	private void throwBall(){

        ball = _gcic.CreateDrop(true); //AddDrop -> CreateDrop
        ball.GetComponent<CharacterSize>().SetSize((int)sizeshot);
        prepareDropToFly();

        ball.SetActive(true);

		ball.GetComponent<CharacterControllerCustom>().SendFlying(st.GetpVelocity());
	}

    

        public void OnDrawGizmosSelected () {
            if (!Application.isPlaying)
            {
                Awake();
                Update();
            }

            Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt((5 * (9))));
        for (int i = 1; i < GetComponent<CharacterSize>().GetSize(); ++i)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), Mathf.Sqrt(5 * (ccc.GetComponent<CharacterSize>().GetSize() - i) * (25)));

        }
       // Debug.Log(" tam " + GetComponent<CharacterSize>().GetSize() + "sizeshot " + sizeshot);
    }
    

    #endregion
}