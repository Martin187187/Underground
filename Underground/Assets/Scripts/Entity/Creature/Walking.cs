using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : Creature
{
    private float normalAttackCounter = 0;
    private float rdmWalkTime = 0;
    private float rdmWalkDuration = 10;


    void Update()
    {
        if(normalAttackCounter < 2)
        {
            normalAttackCounter += Time.deltaTime;
        }

        if(mode == FollowMode.WAYPOINT && status == Status.WAITING)
        {
            if(rdmWalkTime>rdmWalkDuration)
            {
                rdmWalkDuration= Random.Range(2f,5f);
                rdmWalkTime = 0;
                SetFollowPosition(GetRandomWaypoint());
            }
            else
            {
                rdmWalkTime+=Time.deltaTime;
            }
        }

    }

    
    public override void NormalAttack()
    {
        if(normalAttackCounter<2)
            return;

        normalAttackCounter = 0;
        exe.DelayExecute(1.3f, x => {
            enemy.GetComponent<Rigidbody>().velocity += (((Transform)x[0]).position - ((Transform)x[1]).position).normalized * 10f;
            enemy.TakeDamake(5);
        },enemy.transform, transform);
        if(anim.runtimeAnimatorController!=null)
        anim.CrossFade("Armature|Attack1",0,0);
        
    }
    public override void ActivateAbilityOne()
    {
        if(!IsStunned() && enemy != null){
            Vector3 direction = (enemy.transform.position - transform.position).normalized+Vector3.up*0.3f;
            m_Rigidbody.velocity += direction * 20;
        }
    }
    
    public override void ActivateAbilityTwo()
    {
        anim.CrossFade("Standing 1H Magic Attack 01",0.1f,0);
        Vector3 target = GetFollowPosition();
        Vector3 position = transform.position;
        if(rHand!= null)
        {
            position = rHand.position;
        }
        if(enemy!=null)
        {
            target =enemy.transform.position + enemy.center;
        }
        
        exe.DelayExecute(.65f, x => {
            effectFactory.CreateEffect("FireBall.prefab", position,Quaternion.LookRotation(-target + position, Vector3.up), transform);
        });
        
    }


}
