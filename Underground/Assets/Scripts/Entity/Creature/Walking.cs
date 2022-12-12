using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : Creature
{
    private float normalAttackCounter = 0;
    // Start is called before the first frame update
    public float rotationStrength = 2f;
    public float floatStrength = 8f;
    public float floatHeight = 3f;

    public List<IKFootSolver2> brothers = new List<IKFootSolver2>();

    public Status status = Status.IDLE;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 target = GetFollowPosition();
        if(target==null)
            return;
        if(status == Status.WAITING_STOP)
            return;
        if(status == Status.WAITING)
        {
            if(Vector3.Distance(transform.position, target) < 5f)
                return;
            
            status = Status.WAITING_STOP;
            anim.CrossFade("waiting_stop",0,0);
            exe.DelayExecute(1f, x => {
                status = Status.IDLE;
                foreach (var item in brothers)
                    {
                        item.run = true;
                    }
            });

        }
        if(normalAttackCounter <= 2)
        {
            normalAttackCounter += Time.deltaTime;
        }
        

        Vector3 vel = m_Rigidbody.velocity;
        vel.y = 0;
        if(vel.magnitude>0.3f){
            counter = 0;
            
        } else if(counter <=0.5f){
            counter += Time.deltaTime;
            
        } else {

        }
        
        
        Ray ray = new Ray(transform.position + Vector3.up*5, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask)){
            
            Vector3 direction = target - transform.position;
            Vector3 norm = direction.normalized;
            if(counter>=0.5f){
                Vector3 test = new Vector3(transform.position.x, Mathf.Max(info.point.y+floatHeight,transform.position.y), transform.position.z);
            transform.position = test;
                
                if(Vector3.Distance(transform.position, target) > 4f || mode == FollowMode.WAYPOINT){
                    transform.position = Vector3.MoveTowards( transform.position, target, speed * Time.deltaTime );
                }else {
                        foreach (var item in brothers)
                        {
                            item.run = false;
                        }
                        anim.CrossFade("waiting_start",0,0);
                        status = Status.WAITING;
                    }
                

                    if(Vector3.Distance(transform.position, target) <= 4f && mode == FollowMode.ATTACK_FOLLOW){
                        if(normalAttackCounter>2){
                            normalAttackCounter = 0;
                            NormalAttack();
                        }
                    } 
            
            }

            //transform.LookAt( follow.transform.position, Vector3.up );
            direction = target - transform.position;
            direction.y = 0;
            if(direction.magnitude>0)
            {
                Quaternion toRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime *rotationStrength);
            }

            
            //m_Rigidbody.MoveRotation(toRotation);
        }
    }


    
    public override void NormalAttack()
    {
        exe.DelayExecute(1.3f, x => {
            enemy.GetComponent<Rigidbody>().velocity += (((Transform)x[0]).position - ((Transform)x[1]).position).normalized * 10;
            enemy.TakeDamake(5);
        },enemy.transform, transform);
        anim.CrossFade("Armature|Attack1",0,0);
        
    }
    public override void ActivateAbilityOne()
    {
        if(counter>=0.5f && FollowMode.ATTACK_FOLLOW == mode){
            Vector3 direction = (enemy.transform.position - transform.position).normalized+Vector3.up*0.3f;
            m_Rigidbody.velocity += direction * 20;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(mode == FollowMode.ATTACK_FOLLOW)
        {
            GameObject o = collision.collider.gameObject;
            Creature c = o.GetComponent<Creature>();
            if(c == enemy){
                c.TakeDamake(collision.relativeVelocity.magnitude);
            }
            
        }
    }

    public enum Status{
        IDLE, WAITING, WAITING_STOP
    }

}
