using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShootTrajectory : MonoBehaviour {

    

    public GameObject TrajectoryPointPrefeb;
    public GameObject TrajectorySizeIndicator;
    public GameObject TrajectoryParticlePrefeb;
    public int numOfTrajectoryPoints = 30;
    public float particletrajectoryspeed = 0.08f;
    private int jk = 0;

    private List<GameObject> trajectoryPoints;
    private List<GameObject> bolas;
    private CharacterControllerCustom ccc;
    private CharacterController c;
    private  CharacterShoot s;
    
    private Vector3 vel,aiming,old;
    private float power = 25;
    private float shootsize=1;
    public float limitshoot = 10;
    private bool stopcourutine=false;
    

    private RaycastHit hit;
    private Vector3 fwd,aux;
    private bool colisiondetected = false;
    private GameObject sphere,ball;
    private float delay = 2,next;
    private float oldvelocity;
    private float h, v;
    private bool horizontal;
    private bool vertical;
    private float shootlimiet = 1;
    private bool selecting = false;
    private float oldshootlimit;
    private float velocity = 1;
    private bool correcting = false;
    private RaycastHit hitpoint;
    private float angle;
    // Use this for initialization
    void Awake() {

        horizontal = true;
        vertical = true;

        next = Time.time + delay;
        this.enabled = false;

        //sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
       

        vel.x = 2;
        vel.y = 2;

        c = GetComponent<CharacterController>();
        ccc = GetComponent<CharacterControllerCustom>();
        s = GetComponent<CharacterShoot>();

        trajectoryPoints = new List<GameObject>();


        for (int i = 0; i < numOfTrajectoryPoints; i++) {
            GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
            dot.GetComponent<Renderer>().enabled = false;
            //dot.tag = ("Trajectory"+i);
            dot.transform.parent = ccc.transform;

            trajectoryPoints.Insert(i, dot);


       }
       
    }

    public void selectingsize(float size)
    {
        shootsize = size;
        selecting = true;
        
    }

    public void OnEnable() {
        vel.x = 2;
        vel.y = 2;
        horizontal = true;
        shootsize = 1;
        sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().enabled = false;

        ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
        ball.GetComponent<Collider>().enabled = false;
       
        
        shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);
        //Debug.Log("entra" + shootlimiet);
        ball.GetComponent<Renderer>().enabled = true;
                
        stopcourutine = false;
        StartCoroutine(Example());
       // Debug.Log("Limitshot " + limitshoot);
    }
    public void OnDisable()
    {
        //Debug.Log("end " + shootlimiet);
        if(ball!= null)
            ball.GetComponent<Renderer>().enabled = false;
        stopcourutine = false;
        stopcourutine = true;
        StopCoroutine(Example());
        Destroy(sphere);
        Destroy(ball);
        //Debug.Log("PARANDOOOOO");
    }
    // Update is called once per frame
    void Update () {

        if (selecting)
        {

            oldshootlimit = shootlimiet;
            ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
            shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);
            
            if (oldshootlimit > shootlimiet)
            {
                
            }
            selecting = false;

        }

        h = Input.GetAxis(Axis.Horizontal);
        v = Input.GetAxis(Axis.Vertical);

        if (horizontal)
        {        
            old.x = vel.x;
            old.y = vel.y;

            vel.x += h;
            vel.y += v;


        }
        else if(horizontal== false)
        {

                if (vel.x > 0)
                {
                    old.x = vel.x;
                    old.y = vel.y;
                    vel.x -=0.3f;
                     vel.y -= 0.3f;

                    Debug.Log(" vely" + vel);
            }
                else if (vel.x < 0)
                {
                    vel.x += 0.3f;
                    vel.y -= 0.3f;
                   // Debug.Log(" vely" + vel.y + " velx " + vel.x);
                }

                if (vel.y < -20) vel.y =0;

               // Debug.Log(" vely" + vel.y + " velx " + vel.x + " V " +v);
            }


        
        // transform.eulerAngles = new Vector3(0, 0, angle);    this is to face in the direction you are aming

         Vector3 pos = this.transform.position + getvect().normalized * (c.radius * this.transform.lossyScale.x + c.radius * this.transform.lossyScale.x);
        //Vector3 pos = this.transform.position;
        setTrajectoryPoints(pos, vel);
        setvisibility();
        canshooot();

    }
    public bool canshooot()
    {
        float dis = 0;
        Vector3 spheredis = transform.position + getvect().normalized * (c.radius * this.transform.lossyScale.x  );
        fwd = trajectoryPoints[0].transform.position - spheredis;
       
        dis = fwd.magnitude;

        Debug.DrawRay(spheredis, fwd, Color.green);

        if (Physics.Raycast(spheredis, fwd, dis))
        {
            ball.GetComponent<Renderer>().enabled = false;
            for (int j = 1; j < numOfTrajectoryPoints - 1; j++)
            {
                trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

            }
            return false;
 
        }
        else return true;

        
    }

    public void QuitTrajectory() {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
        }
        sphere.GetComponent<Renderer>().enabled = false;
        stopcourutine = true;
        StopCoroutine(Example());
    }

    public Vector3 getvect() {
        return vel;
    }
    //---------------------------------------	
    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos) {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;//*ball.rigidbody.mass;
    }
    //--------------------------------------	
    // It displays projectile trajectory path
    //---------------------------------------	
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity) {

        oldvelocity = velocity;
        velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));

        //Debug.Log(" velocity " + velocity + " shoolitmit " + shootlimiet);
        //Debug.Log(" velocity x  " + pVelocity.x + " velocity y  " + pVelocity.y);

        if (velocity > shootlimiet)
        {
            horizontal = false;

        }
        else if (velocity < shootlimiet)
        {
            horizontal = true;

             angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));

            float fTime = 0;
            float oldx = 0;
            float oldy = 0;
            bool notsame = false;

            fTime += 0.1f;
            for (int i = 0; i < numOfTrajectoryPoints && !notsame; i++)
            {
                float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
                float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
                Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
                // if (oldx == pos.x && oldy == pos.y)
                //  notsame = true;
                trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100);
                // trajectoryPoints[i].transform.position = pos;
                //bolas[i].GetComponent<Renderer>().enabled = false;
                trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
                trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (ccc.Parameters.Gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
                fTime += 0.1f;
                oldx = pos.x;
                oldy = pos.y;
                //Debug.Log(" zasca " + trajectoryPoints[i]);
            }
        }
    }
    public IEnumerator Example()
    {
        sphere.transform.position = trajectoryPoints[0].transform.position;
        while (!stopcourutine)
        {
            if (trajectoryPoints[jk].GetComponent<Renderer>().enabled == true){
                sphere.transform.position = trajectoryPoints[jk].transform.position;
                // Debug.Log(" zasca " + trajectoryPoints[jk].transform.position);       

            }
            if ((jk == numOfTrajectoryPoints - 1))
            {
                jk = 0;
                //sphere.transform.position = trajectoryPoints[jk].transform.position;
                // Debug.Log(" end " + numOfTrajectoryPoints);
            }
            if (trajectoryPoints[jk].GetComponent<Renderer>().enabled == false)
            {
                jk = -1;
            }
            
            
            jk++;
            
            yield return new WaitForSeconds(particletrajectoryspeed);
        }
            
        
    }
  
   


    public void setvisibility() {
        float dis = 0;
        //sphere.transform.position = trajectoryPoints[1].transform.position;
        sphere.GetComponent<Renderer>().enabled = true;

        for (int i = 0; i < numOfTrajectoryPoints-1 && !colisiondetected; i++) {
            
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;

            //fwd = trajectoryPoints[i].transform.TransformDirection(Vector3.right);
            
            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            // dis = Mathf.Sqrt((Mathf.Pow(trajectoryPoints[i + 1].transform.position.x - trajectoryPoints[i].transform.position.x,2)  + Mathf.Pow(trajectoryPoints[i + 1].transform.position.y - trajectoryPoints[i].transform.position.y,2)));

            dis = fwd.magnitude;

            if (Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint,dis))
            {
                ball.GetComponent<Renderer>().enabled = true;
                Vector3 hitting = hitpoint.point;
                hitting.y += 0.5f;
                ball.transform.position = hitting;
                colisiondetected = true;
                for (int j = i+1; j < numOfTrajectoryPoints-1 ; j++) {
                    trajectoryPoints[j].GetComponent<Renderer>().enabled = false;
                    
                }
                trajectoryPoints[numOfTrajectoryPoints-1].GetComponent<Renderer>().enabled = false;
            }

            Debug.DrawRay(trajectoryPoints[i].transform.position, fwd , Color.green);
            
        }
        colisiondetected = false;
        
    }

}
