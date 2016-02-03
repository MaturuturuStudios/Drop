using UnityEngine;
using System.Collections;

public class CharacterShoot : MonoBehaviour{

    public Rigidbody bulletPrefab;
    float attackSpeed = 2f;
    float cooldown;
    public GameObject focus;

    private bool Shoot_position;

    public float smooth = 2.0F;
    public float tiltAngle = 30.0F;

    private Quaternion target;

    void Start(){
        Shoot_position = false;
        focus.SetActive(false);
        
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot_position = false;
            focus.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shoot_position = true;
        }
        if (Time.time >= cooldown && Shoot_position){
            focus.SetActive(true);

            float tiltAroundZ = Input.GetAxis("Horizontal") * -tiltAngle;
                
            target = Quaternion.Euler(0, 0, tiltAroundZ);
            focus.transform.rotation = Quaternion.Slerp(focus.transform.rotation, target, Time.deltaTime * smooth);

            if (Input.GetKeyDown(KeyCode.Space))
                {
                //transform.rotation = focus.transform.rotation;
                    Fire();
                    Debug.Log("Shoot");
                }
            }
        }

        // Fire a bullet
   void Fire(){
            Rigidbody bPrefab = Instantiate(bulletPrefab, transform.position, target) as Rigidbody;
            bPrefab.AddForce(Vector3.up * 100f);
            cooldown = Time.time + attackSpeed;
    }

    public bool Shooting(){
        return Shoot_position;
    }
}
