using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFollow : Creature
{
    public Transform follow;
    public float speed = 1f;
    private LayerMask mask;
    private Rigidbody m_Rigidbody;
    float counter = 0;
    float jumpCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        mask = LayerMask.GetMask("Terrain");
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(follow==null)
            return;
        

        Vector3 vel = m_Rigidbody.velocity;
        vel.y = 0;
        if(vel.magnitude>0.3f)
            counter = 0;
        else if(counter <=0.5f)
            counter += Time.deltaTime;
        
        
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask)&&info.distance<2f){
            
            Vector3 direction = follow.position - transform.position;
            Vector3 norm = direction.normalized;
            if(counter>=0.5f){
                
                Vector3 test = new Vector3(transform.position.x, Mathf.Max(info.point.y+0.8f,transform.position.y), transform.position.z);
                transform.position = Vector3.MoveTowards( transform.position, test, speed*8 * Time.deltaTime );
                if(Vector3.Distance(transform.position, follow.transform.position) > 2f)
                    transform.position = Vector3.MoveTowards( transform.position, follow.transform.position, speed * Time.deltaTime );
                //transform.position = new Vector3(transform.position.x, Mathf.Max(info.point.y+1,transform.position.y), transform.position.z);

                if(jumpCounter<10f)
                    jumpCounter += Time.deltaTime;
                else
                if((Mathf.Abs(norm.x)+Mathf.Abs(norm.z))/2<Mathf.Abs(norm.y))
                {
                    Debug.Log("jump");
                    jumpCounter = 0;
                    m_Rigidbody.velocity += norm*20;
                    
                }
            
            }

            //transform.LookAt( follow.transform.position, Vector3.up );
            
            direction.y = 0;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            toRotation *= Quaternion.Euler(0, -90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime *2);

            
            //m_Rigidbody.MoveRotation(toRotation);
        }
    }
}
