﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShoot : MonoBehaviour {

    public GameObject BallPrefb;

   
    private GameObject ball;
	private bool  isBallThrown;


    //Toni: At the moment I make it public but if you want, you can make a getShootMode() function
    public bool shootmode = false;
    private float sizeshot=1;

    CharacterController c;
    CharacterControllerCustom ccc;
    CharacterShootTrajectory st;
    GameControllerIndependentControl _gcic;
    CharacterSize size;

    //---------------------------------------	
    void Awake (){
        c = GetComponent<CharacterController>();
        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        size= GetComponent<CharacterSize>();

        _gcic = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();

       
    }
    public bool isShooting(){
        return shootmode;
    }

    //---------------------------------------	
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
	void Update (){

        if ((shootmode== true) && (ccc.State.IsGrounded == false || size.GetSize()==1 ))
        {
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;
            // ccc.Parameters = CharacterControllerParameters.ShootingTube;
            ccc.Parameters = null;                      
        }
     }
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
    //Set ball properties like createBall()
    private void prepareDropToFly()
    {

        ball.transform.position = this.transform.position + st.GetpVelocity().normalized * (c.radius * this.transform.lossyScale.x + ball.GetComponent<CharacterController>().radius * ball.transform.lossyScale.x);
	    ball.SetActive(false);
    }

	private void throwBall(){

        ball = _gcic.CreateDrop(true); //AddDrop -> CreateDrop
        ball.GetComponent<CharacterSize>().SetSize((int)sizeshot);
        prepareDropToFly();

        ball.SetActive(true);

		ball.GetComponent<CharacterControllerCustom>().SendFlying(st.GetpVelocity());
	}
}