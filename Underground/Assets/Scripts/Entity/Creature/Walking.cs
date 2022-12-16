using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : Creature
{
    private float normalAttackCounter = 0;
    public float rotationStrength = 2f;
    public float floatHeight = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 target = GetFollowPosition();
        if(target==null)
            return;
        if(status == Status.ANIMATION_STOP)
            return;
        if(status == Status.WAITING)
        {
            if(Vector3.Distance(transform.position + center, target) < size+2f)
                return;
            
            status = Status.ANIMATION_STOP;
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
        
        /*
        Vector3 vel = m_Rigidbody.velocity;
        vel.y = 0;
        if(vel.magnitude>0.3f){
            counter = 0;
        } else
        */     if(counter <=0.5f){
            counter += Time.deltaTime;
            
        } else {

        }
        
        
        Ray ray = new Ray(transform.position + Vector3.up*5, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask)){
            
            Vector3 direction = target - transform.position;
            Vector3 norm = direction.normalized;
            if(counter>=0.5f){
                //float max = Mathf.Max(info.point.y+floatHeight,transform.position.y);
                float max = info.point.y+floatHeight;
                Vector3 test = new Vector3(transform.position.x, max, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, test, speed * Time.deltaTime );
                
                if(Vector3.Distance(transform.position +center, target) >  ((mode == FollowMode.ATTACK_FOLLOW) ? size + enemy.size : size) || mode == FollowMode.WAYPOINT){
                    transform.position = Vector3.MoveTowards( transform.position, target, speed * Time.deltaTime );
                }else 
                {
                    foreach (var item in brothers)
                    {
                        item.run = false;
                    }
                    anim.CrossFade("waiting_start",0,0);
                    status = Status.WAITING;
                }
                

                if(mode == FollowMode.ATTACK_FOLLOW && Vector3.Distance(transform.position +center, target) <= size + enemy.size ){
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
            //m_Rigidbody.velocity += direction * 20;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        /*
        if(mode == FollowMode.ATTACK_FOLLOW)
        {
            GameObject o = collision.collider.gameObject;
            Creature c = o.GetComponent<Creature>();
            if(c == enemy){
                c.TakeDamake(collision.relativeVelocity.magnitude);
            }
            
        }
        */
    }


}
