using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class Creature : MonoBehaviour
{
    protected LayerMask mask;
    protected Rigidbody m_Rigidbody;
    protected BoxCollider col;
    protected Animator anim;
    protected Executer exe;
    public Transform rHand;
    public enum FollowMode
    {
        WAYPOINT, SILENT_FOLLOW
    };
    public FollowMode mode = FollowMode.WAYPOINT;
    [SerializeField] protected Vector3 waypoint;
    [SerializeField] protected Transform follow;
    [SerializeField] protected Creature enemy;
    public float speed = 1f;


    //health bar
    public HealthBar healthBar;

    //stats
    public float currentHP, maxHp = 100;
    public float attack = 1;

    public Status status = Status.IDLE;
    public List<IKFootSolver2> brothers = new List<IKFootSolver2>();
    public float followDistance = 4f;
    
    public Vector3 center;
    public float size;

    public float rotationStrength = 2f;
    public float floatHeight = 0f;

    protected float currentStunTime = 0;
    public float stunduration = 0f;

    protected EffectFactory effectFactory;
    void Start()
    {
        effectFactory = GameObject.Find("EffectFactory").GetComponent<EffectFactory>(); 
        mask = LayerMask.GetMask("Terrain");
        m_Rigidbody = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        exe = new Executer(this);
        waypoint = transform.position;

        currentHP = maxHp;
        healthBar.SetHealth(maxHp);

    }
        void OnEnable()
    {
        exe = new Executer(this);
    }

   void FixedUpdate()
    {
        List<Particle> particleList = effectFactory.GetProjectileList();
        for (int i = 0; i < particleList.Count; i++)
        {
            Particle particle = particleList[i];
            if(col.bounds.Contains(particle.transform.position)&&particle.parent.gameObject!=gameObject)
                effectFactory.RemoveParticle(particle);
        }
        if(m_Rigidbody.velocity.magnitude > 0.3f)
        {
            StunCreature(0.1f);
        }
        bool stunned = IsStunned();
        if(stunned)
        {
            currentStunTime += Time.deltaTime;
            GluePosition(transform.position);
        }
        else
        {
            
            if(IsPassivStatus() || IsAnimation())
            {
                
                if(status == Status.WAITING && mode == FollowMode.SILENT_FOLLOW && !IsNear(4f))
                    ChangeDefaultStatus();
                LookStepPartly();
            }
            else
            {
                if(enemy !=null)
                    LookStep(enemy.transform.position + enemy.center);
                else
                    LookStep();
                //if creature is not stunned or passive
                if(IsNear())
                {
                    if(enemy == null)
                    {
                        if(anim.runtimeAnimatorController!=null)
                            anim.CrossFade("waiting_start",0,0);
                        foreach (var item in brothers)
                        {
                            item.run = false;
                        }
                        status = Status.WAITING;
                    }
                    
                }
                else
                {
                    MoveStep();
                }
                

            }
            
            GluePosition(transform.position);
            
            
                
        }
        foreach (var item in brothers)
        {
            if(IsAnimation())
            {
                item.transform.parent.GetComponent<TwoBoneIKConstraint>().weight = 0f;
            }
                else
            {
                item.transform.parent.GetComponent<TwoBoneIKConstraint>().weight = 1f;
                item.Run(stunned);
            }
        }

    }

    public abstract void NormalAttack();
    public abstract void ActivateAbilityOne();
    public abstract void ActivateAbilityTwo();

    public bool IsAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Standing 1H Magic Attack 01");
    }

    public void TakeDamake(float damage)
    {
        currentHP-=damage;
        healthBar.SetHealth(currentHP);
    }

        public void ChangeDefaultStatus()
    {
        ChangeMode(Status.IDLE);
    }
    public void ChangeMode(Status newStatus)
    {
        if(status == Status.WAITING)
        {
            status = Status.ANIMATION_STOP;
            if(anim.runtimeAnimatorController!=null)
            anim.CrossFade("waiting_stop",0,0);
            exe.DelayExecute(1f, x => {
                status = newStatus;
                foreach (var item in brothers)
                    {
                        item.run = true;
                    }
            });

        } else {
            
        }
    }

    public bool IsStunned(){
        return currentStunTime < stunduration;
    }
    public void StunCreature(float amount)
    {
        if(stunduration - currentStunTime < amount)
        {
            currentStunTime = 0;
            stunduration = amount;
        }
    }
    protected void LookStep()
    {
        LookStep(GetFollowPosition());
    }
    protected void LookStep(Vector3 lookTarget)
    {
        Vector3 direction = lookTarget - transform.position;
        direction.y = 0;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime *rotationStrength);
    }

    protected void LookStepPartly()
    {
        Vector3 direction = GetFollowPosition() - transform.position;
        direction.y = 0;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        toRotation.x = 0;
        toRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime *rotationStrength);
    }

    protected void MoveStep()
    {
        Vector3 goal = GetFollowPosition();
        Vector3 newPosition;
        switch(mode)
        {
            case FollowMode.WAYPOINT:
                newPosition = Vector3.MoveTowards( transform.position, goal, speed * Time.deltaTime );
                break;
            case FollowMode.SILENT_FOLLOW:
                newPosition = Vector3.MoveTowards( transform.position, goal + Vector3.down * follow.localScale.y, speed * Time.deltaTime );
                break;
            default:
                newPosition = Vector3.MoveTowards( transform.position, goal- enemy.center, speed * Time.deltaTime );
                break;
        }
        Ray ray = new Ray(newPosition + Vector3.up*5, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 40, mask)){
            newPosition = new Vector3(newPosition.x, info.point.y+floatHeight, newPosition.z);

            //todo: check steigung
            Vector3 dir = newPosition - transform.position;
            if((Mathf.Abs(dir.x)+Mathf.Abs(dir.z))/2>dir.y)
                transform.position = newPosition;
            else
            {
                if(anim.runtimeAnimatorController!=null)
                            anim.CrossFade("waiting_start",0,0);
                        foreach (var item in brothers)
                        {
                            item.run = false;
                        }
                        status = Status.WAITING;
            }
        }
    }

    protected void GluePosition(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up*5, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 40, mask)){
            Vector3 newPosition = new Vector3(position.x, info.point.y+floatHeight, position.z);
            transform.position = newPosition;
        }
    }


    public Vector3 GetRandomWaypoint()
    {
        float maxDistance = size * 5;
        Vector3 target = transform.position;
        float x = Random.Range(-maxDistance, maxDistance), z = Random.Range(-maxDistance, maxDistance);
        return new Vector3(target.x + x, target.y, target.z +z);
    }
    protected bool IsNear()
    {
        return IsNear(0);
    }
    protected bool IsNear(float extra)
    {
        Vector3 goal = GetFollowPosition();
        switch(mode)
        {
            case FollowMode.WAYPOINT:
                return Vector3.Distance(transform.position, goal) < size + extra;
            case FollowMode.SILENT_FOLLOW:
                return Vector3.Distance(transform.position + center, goal) < size + 1 + extra;
            default:
                return Vector3.Distance(transform.position + center, goal) < enemy.size + size + extra;
        }
    }

    protected Vector3 GetFollowPosition(){
        switch (mode)
        {
            case FollowMode.SILENT_FOLLOW:
                return follow ? follow.transform.position : transform.position;    
            default:
                return waypoint;
        }
    }

    public bool IsPassivStatus()
    {
        return status == Status.ANIMATION_STOP || status == Status.WAITING;
    }
    public void SetFollowPosition(Vector3 pos)
    {
        waypoint = pos;
        mode = FollowMode.WAYPOINT;
        ChangeDefaultStatus();
    }

    public void SetFollowTarget(Transform trans)
    {
        follow = trans;
        mode = FollowMode.SILENT_FOLLOW;
        enemy = null;
        ChangeDefaultStatus();
    }

    public void SetAttackTarget(Creature creature)
    {
        enemy = creature;
        ChangeDefaultStatus();
    }
    
    public enum Status{
        IDLE, WAITING, ANIMATION_STOP
    }

    public void DestroyWalker()
    {
        Destroy(this);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.2f);
        Gizmos.DrawSphere(transform.position + center, size);
    }

    
}
