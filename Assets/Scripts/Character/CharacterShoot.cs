using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShoot : MonoBehaviour {

    public GameObject BallPrefb;

   
    private GameObject ball;
	private bool  isBallThrown;
    private int lessize=0;
    private int falling = 0;
    //Toni: At the moment I make it public but if you want, you can make a getShootMode() function
    public bool shootmode = false;
    

    CharacterControllerCustom ccc;
    CharacterShootTrajectory st;
    GameControllerIndependentControl _gcic;

    //---------------------------------------	
    void Start (){

        ccc = GetComponent<CharacterControllerCustom>();
        st= GetComponent<CharacterShootTrajectory>();
        //gc = GetComponent<GameControllerIndependentControl>(); //Comentado por toni

        _gcic = GameObject.FindGameObjectWithTag("GameController")
                                .GetComponent<GameControllerIndependentControl>();
	}
    public bool isShooting(){
        return shootmode;
    }
	//---------------------------------------	

    public void Aim(){
        if ( ccc.State.IsGrounded == true && (GetComponent<CharacterSize>().GetSize() > 1) && (GetComponent<CharacterSize>().GetSize()<9))
        {
            if (shootmode == false)
            {
                shootmode = true;
                st.enabled = true;

                ccc.Parameters = CharacterControllerParameters.ShootingParameters;
            }
            else if ((shootmode == true))
            {
                shootmode = false;
                st.QuitTrajectory();
                st.enabled = false;
                ccc.Parameters = CharacterControllerParameters.ShootingEnd;
            }
        }


    }
    public void Shoot()
    {
        if ( (shootmode == true))
        {
            //if (_gcic.allCurrentCharacters.Capacity < _gcic.numOfDrops) //Commented by toni, No longer pool needed
            //{
            ccc.Parameters = CharacterControllerParameters.ShootingEnd;
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;

            lessize = GetComponent<CharacterSize>().GetSize();
            lessize -= 1;
            GetComponent<CharacterSize>().SetSize(lessize);

            throwBall();
            
            
            
            //_gcic.SetControl(1);
            //_gcic.ControlNextDrop();
            //}
        }


    }
	void Update (){
       /* if ((Input.GetKeyDown(KeyCode.X) && ccc.State.IsGrounded == true && (GetComponent<CharacterSize>().GetSize()>1))) {
            if (shootmode == false){
                shootmode = true;
                st.enabled = true;
                
                ccc.Parameters = CharacterControllerParameters.ShootingParameters;
            }else if((shootmode== true))
            {
                shootmode = false;
                st.QuitTrajectory();
                st.enabled = false;
                ccc.Parameters = null;
            }       
        }
        if ((lessize == 1) || ccc.State.IsGrounded == false){
            shootmode = false;
            st.QuitTrajectory();
            st.enabled = false;
            
             //ccc.Parameters = null;                      
        }*/
        
       
       /* if ((Input.GetKeyDown(KeyCode.Space)) && (shootmode==true)){
			
			if(_gcic.allCurrentCharacters.Capacity<_gcic.numOfDrops){
                throwBall();
                lessize = GetComponent<CharacterSize>().GetSize();
                lessize -= 1;
                GetComponent<CharacterSize>().SetSize(lessize);
                shootmode = false;
                //_gcic.SetControl(1);
                _gcic.ControlNextDrop();
            }
		}*/
        
        
	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
    //Set ball properties like createBall()
    private void prepareDropToFly()
    {
        Vector3 pos = transform.position;
        pos.z = 1;
        ball.transform.position = pos;
		ball.SetActive(false);
    }

	private void throwBall(){

        ball = _gcic.CreateDrop(true); //AddDrop -> CreateDrop
        ball.GetComponent<CharacterSize>().SetSize(1);
        prepareDropToFly();

        ball.SetActive(true);

		ball.GetComponent<CharacterControllerCustom>().SendFlying(st.getvect());
	}
}