﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class active, desactive shoot-mode and shoot a drop 
/// </summary>
public class CharacterShoot : MonoBehaviour {

    #region Private Attributes

    /// <summary>
	/// Defines the object that will use to create the drop shooted.
	/// </summary> 
    private GameObject _ball;

    /// <summary>
	/// Defines the size of the drop shooted.
	/// </summary> 
    private float _sizeshot = 1;

    /// <summary>
    /// List of observers subscribed to the character shoot's
    /// events.
    /// </summary>
    private List<CharacterShootListener> _listeners = new List<CharacterShootListener>();

	/// <summary>
	/// Reference to the object which will be shown when the
	/// player is unable to shoot.
	/// </summary>
	private ShootUnableIndicator _unableIndicator;

    #endregion

    #region Public Attributes

    /// <summary>
	/// Defines the boolean to know if we are in shootmode or out of shootmode.
	/// </summary> 
    public bool shootmode = false;

    /// <summary>
    /// Defines the scripts objects that we will use it.
    /// </summary> 
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
        
        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        size= GetComponent<CharacterSize>();

        _gcic = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();

		_unableIndicator = GetComponentInChildren<ShootUnableIndicator>();
    }

    /// <summary>
    /// Subscribes a listener to the shoot's events.
    /// Returns false if the listener was already subscribed.
    /// </summary>
    /// <param name="listener">The listener to subscribe</param>
    /// <returns>If the listener was successfully subscribed</returns>
    public bool AddListener(CharacterShootListener listener) {
        if (_listeners.Contains(listener))
            return false;
        _listeners.Add(listener);
        return true;
    }

    /// <summary>
    /// Unsubscribes a listener to the shoot's events.
    /// Returns false if the listener wasn't subscribed yet.
    /// </summary>
    /// <param name="listener">The listener to unsubscribe</param>
    /// <returns>If the listener was successfully unsubscribed</returns>
    public bool RemoveListener(CharacterShootListener listener) {
        if (!_listeners.Contains(listener))
            return false;
        _listeners.Remove(listener);
        return true;
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
        if (shootmode && !st.Animation()) {
            float oldsize;

            oldsize = _sizeshot;
            _sizeshot++;
            if (_sizeshot <= ( GetComponent<CharacterSize>().GetSize()/2))
                st.selectingsize(_sizeshot);
            else _sizeshot = oldsize;
        }
    }

    /// <summary>
	/// Method to look to the other side
	/// </summary>
    public void LookatRight() {
        if (shootmode && !st.Lookingat()){           
            st.LookatRight();
        }
    }

    /// <summary>
	/// Method to look to the other side
	/// </summary>
    public void LookatLeft(){
        if (shootmode && !st.Lookingat()){            
            st.LookatLeft();
        }
    }

    /// <summary>
    /// Method to decrease the size of the drop shooted
    /// </summary>
    public void DecreaseSize(){
        if (shootmode  && !st.Animation()) {
            float oldsize;
      
            oldsize = _sizeshot;
            _sizeshot--;

            if (_sizeshot > 0)
                st.selectingsize(_sizeshot);
            else _sizeshot = oldsize;
        }
    }

    /// <summary>
	/// Method to start the shootmode
	/// </summary>
    public void Aim(){
        float size_component=GetComponent<CharacterSize>().GetSize();

        if (ccc.State.IsGrounded == true && (size_component < 10) && _gcic.allCurrentCharacters.Count < 4 && !st.Lookingat())
        {
			if (size_component <= 1) {
				_unableIndicator.Show();
				return;
			}
			if (!shootmode ) {
                shootmode = true;
                st.enabled = true;
                //_sizeshot = 1;
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;

                // Notifies the listeners
                foreach (CharacterShootListener listener in _listeners)
                    listener.OnEnterShootMode(this);
            }
            else if (shootmode && !st.Animation()) {               
                st.Endingd();
            }
        }
    }

    public void Endshootmode() {
        shootmode = false;

        // Notifies the listeners
        foreach (CharacterShootListener listener in _listeners)
            listener.OnExitShootMode(this);
    }

    /// <summary>
	/// Method to shoot the drop
	/// </summary>
    public void Shoot(){
        if (shootmode && !st.Animation() && !st.SizeAnimation() && !st.Lookingat()) {
            ccc.Parameters = null;
            shootmode = false;
            st.Finishing();
            st.enabled = false;
            GetComponent<CharacterSize>().SetSize((int)(GetComponent<CharacterSize>().GetSize()-_sizeshot));

            // Notifies the listeners
            foreach (CharacterShootListener listener in _listeners)
                listener.OnExitShootMode(this);

            throwBall();          
        }
    }

    /// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update (){

        //check if we shouldn't be in shootmode
        if (shootmode && (ccc.State.IsGrounded == false || size.GetSize()==1 )){
            shootmode = false;
            st.Finishing();
            st.enabled = false;
            ccc.Parameters = null;

            // Notifies the listeners
            foreach (CharacterShootListener listener in _listeners)
                listener.OnExitShootMode(this);
        }
     }

    /// <summary>
	/// Method to shoot the drop.
	/// </summary>
	private void throwBall(){

        _ball = _gcic.CreateDrop(this.transform.position, true); //AddDrop -> CreateDrop
        _ball.GetComponent<CharacterSize>().SetSize((int)_sizeshot);
        _ball.SetActive(false);

        _ball.SetActive(true);

        _ball.GetComponent<CharacterControllerCustom>().SendFlying(st.GetpVelocity());

        // Notifies the listeners
        foreach (CharacterShootListener listener in _listeners)
            listener.OnShoot(this, _ball, st.GetpVelocity());
	}

    

   public void OnDrawGizmosSelected () {
       if (!Application.isPlaying){
            Awake();
            Update();
       }
           
       for (int i = 1; i < ccc.GetComponent<CharacterSize>().GetSize(); ++i){

                //Handles.color= Color.Lerp(Color.white, Color.black, (float)1/i );
                //Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1),5 * (ccc.GetComponent<CharacterSize>().GetSize() - i));
                //Debug.Log(" gizmo position " + transform.position + " gizmo distance " + Mathf.Sqrt((5 * (ccc.GetComponent<CharacterSize>().GetSize() - i)) * 25));
            }
        }
    

    #endregion
}