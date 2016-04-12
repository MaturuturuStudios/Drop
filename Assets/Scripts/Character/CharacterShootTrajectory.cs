using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterShootTrajectory : MonoBehaviour
{

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
    private CharacterShoot s;

    private Vector3 vel, pVelocity;
    private float shootsize = 1;
    public float limitshoot = 5;
    private bool stopcourutine = false;

    private RaycastHit hit;
    private Vector3 fwd;
    private bool colisiondetected = false;
    private GameObject sphere, ball;
    private float delay = 2, next;
    private float oldvelocity;
    private float h, v;
    private float shootlimiet = 1;
    private bool selecting = false;
    private float velocity = 1;

    private RaycastHit hitpoint;
    private float angle;
    // Use this for initialization
    void Awake()
    {
        next = Time.time + delay;
        this.enabled = false;

        angle = 45;

        c = GetComponent<CharacterController>();
        ccc = GetComponent<CharacterControllerCustom>();
        s = GetComponent<CharacterShoot>();

        trajectoryPoints = new List<GameObject>();

        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
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
    //This fuctions create prefabs that we are going to use and initialite some variables
    public void OnEnable()
    {
        shootsize = 1;
        sphere = (GameObject)Instantiate(TrajectoryParticlePrefeb);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().enabled = false;

        ball = (GameObject)Instantiate(TrajectorySizeIndicator);
        ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
        ball.GetComponent<Collider>().enabled = false;


        shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);
        Debug.Log(" limit shoot " + limitshoot + " character ssize  " + ccc.GetComponent<CharacterSize>().GetSize() + " diasparada " + shootsize);

        ball.GetComponent<Renderer>().enabled = true;

        stopcourutine = false;
        StartCoroutine(Example());
        
    }
    //This fuctions delete prefabs that we ar not using
    public void OnDisable()
    {
        
        if (ball != null)
            ball.GetComponent<Renderer>().enabled = false;
        stopcourutine = false;
        stopcourutine = true;
        StopCoroutine(Example());
        Destroy(sphere);
        Destroy(ball);
        
    }
    // Update is called once per frame
    void Update()
    {

        if (selecting)
        {
            ball.transform.localScale = new Vector3(shootsize, shootsize, shootsize);
            shootlimiet = limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize);

            selecting = false;
        }

        h = Input.GetAxis(Axis.Vertical);
        //v = Input.GetAxis(Axis.Vertical);

        angle += h;

        // transform.eulerAngles = new Vector3(0, 0, angle);    this is to face in the direction you are aming

        Vector3 pos = this.transform.position + new Vector3(1,1,0).normalized*(c.radius * this.transform.lossyScale.x + c.radius * this.transform.lossyScale.x);
        float speed = Mathf.Sqrt(limitshoot * (ccc.GetComponent<CharacterSize>().GetSize() - shootsize) / ccc.Parameters.Gravity.magnitude);
        // float speed = shootlimiet;
        Debug.Log(" angulo " + angle + " speed " + speed);
        setTrajectoryPoints(pos, angle, speed);
        setvisibility();
        canshooot();

    }

    //This fuctions calculate if there is a colision with the raycast of the first shoot trajectory  before it
    public bool canshooot()
    {
        float dis = 0;
        Vector3 spheredis = transform.position + getvect().normalized * (c.radius * this.transform.lossyScale.x);
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

    //This fuctions delete the trajectory
    public void QuitTrajectory()
    {
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<Renderer>().enabled = false;
        }
        sphere.GetComponent<Renderer>().enabled = false;
        stopcourutine = true;
        StopCoroutine(Example());
    }

    //This fuctions return the shoot vector for the shoot script
    public Vector3 GetpVelocity()
    {
        return pVelocity;
    }

    //This fuctions calculate the points of the trajectory and the shoot vector which is pVelocity
    void setTrajectoryPoints(Vector3 pStartPosition, float angle, float speed )
    {

        pVelocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * speed;
        Debug.Log(" APUNTANDO " + pVelocity);
        oldvelocity = velocity;
        velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));

        float fTime = 0;
        bool notsame = false;

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints && !notsame; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (ccc.Parameters.Gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, 0);
            trajectoryPoints[i].transform.position = Vector3.MoveTowards(trajectoryPoints[i].transform.position, pos, 100);
            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (ccc.Parameters.Gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }

    }

    //This fuctions draw the particle trip along the trajectory
    public IEnumerator Example()
    {
        sphere.transform.position = trajectoryPoints[0].transform.position;
        while (!stopcourutine)
        {
            if (trajectoryPoints[jk].GetComponent<Renderer>().enabled == true)
            {
                sphere.transform.position = trajectoryPoints[jk].transform.position;      
            }
            if ((jk == numOfTrajectoryPoints - 1))
            {
                jk = 0;
            }
            if (trajectoryPoints[jk].GetComponent<Renderer>().enabled == false)
            {
                jk = -1;
            }
            jk++;
            yield return new WaitForSeconds(particletrajectoryspeed);
        }
    }



    //This fuction draw the trajectory prefab depending on the colisions with their raycast
    public void setvisibility()
    {
        float dis = 0;
        sphere.GetComponent<Renderer>().enabled = true;

        for (int i = 0; i < numOfTrajectoryPoints - 1 && !colisiondetected; i++)
        {

            trajectoryPoints[i].GetComponent<Renderer>().enabled = true;
            fwd = trajectoryPoints[i + 1].transform.position - trajectoryPoints[i].transform.position;

            dis = fwd.magnitude;

            if (Physics.Raycast(trajectoryPoints[i].transform.position, fwd, out hitpoint, dis))
            {
                ball.GetComponent<Renderer>().enabled = true;
                Vector3 hitting = hitpoint.point;
                hitting.y += 0.5f;
                ball.transform.position = hitting;
                colisiondetected = true;
                for (int j = i + 1; j < numOfTrajectoryPoints - 1; j++)
                {
                    trajectoryPoints[j].GetComponent<Renderer>().enabled = false;

                }
                trajectoryPoints[numOfTrajectoryPoints - 1].GetComponent<Renderer>().enabled = false;
            }

            Debug.DrawRay(trajectoryPoints[i].transform.position, fwd, Color.green);

        }
        colisiondetected = false;

    }

}
