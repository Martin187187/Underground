using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFollow : Creature
{
    private float normalAttackCounter = 0;
    // Start is called before the first frame update
    



    // Update is called once per frame
    void FixedUpdate()
    {
        if(normalAttackCounter <= 2)
        {
            normalAttackCounter += Time.deltaTime;
        }
        Vector3 target = GetFollowPosition();
        if(target==null)
            return;
        

        Vector3 vel = m_Rigidbody.velocity;
        vel.y = 0;
        if(vel.magnitude>0.3f){
            counter = 0;
            
        } else if(counter <=0.5f){
            counter += Time.deltaTime;
            
        } else {

        }
        
        
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask)&&info.distance<2f){
            
            Vector3 direction = target - transform.position;
            Vector3 norm = direction.normalized;
            if(counter>=0.5f){
                Vector3 test = new Vector3(transform.position.x, Mathf.Max(info.point.y+0.8f,transform.position.y), transform.position.z);
                transform.position = Vector3.MoveTowards( transform.position, test, speed*8 * Time.deltaTime );
                if(Vector3.Distance(transform.position, target) > 2f || mode == FollowMode.WAYPOINT){
                    transform.position = Vector3.MoveTowards( transform.position, target, speed * Time.deltaTime );
                }
                

                    if(Vector3.Distance(transform.position, target) <= 2f && mode == FollowMode.ATTACK_FOLLOW){
                        if(normalAttackCounter>2){
                            normalAttackCounter = 0;
                            NormalAttack();
                        }
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

}
