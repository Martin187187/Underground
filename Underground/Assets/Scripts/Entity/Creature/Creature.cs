using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    protected LayerMask mask;
    protected Rigidbody m_Rigidbody;
    protected Animator anim;
    protected Executer exe;
    public enum FollowMode
    {
        WAYPOINT, SILENT_FOLLOW, ATTACK_FOLLOW
    };
    public FollowMode mode = FollowMode.SILENT_FOLLOW;
    [SerializeField] protected Vector3 waypoint;
    [SerializeField] protected Transform follow;
    [SerializeField] protected Creature enemy;
    public float speed = 1f;

    protected float counter = 0;

    //health bar
    public HealthBar healthBar;

    //stats
    public float currentHP, maxHp = 100;
    public float attack = 1;
    

    void Start()
    {
        Debug.Log("2");
        mask = LayerMask.GetMask("Terrain");
        m_Rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        exe = new Executer(this);

        currentHP = maxHp;
        healthBar.SetHealth(maxHp);
    }
        void OnEnable()
    {
        exe = new Executer(this);
    }

    public abstract void NormalAttack();
    public abstract void ActivateAbilityOne();

    public void TakeDamake(float damage)
    {
        currentHP-=damage;
        healthBar.SetHealth(currentHP);
    }

    protected Vector3 GetFollowPosition(){
        switch (mode)
        {
            case FollowMode.SILENT_FOLLOW:
                return follow ? follow.transform.position : transform.position;
            case FollowMode.ATTACK_FOLLOW:
                return enemy ? enemy.transform.position : transform.position;            
            default:
                return waypoint;
        }
    }
    public void SetFollowPosition(Vector3 pos)
    {
        waypoint = pos;
        mode = FollowMode.WAYPOINT;
    }

    public void SetFollowTarget(Transform trans)
    {
        follow = trans;
        mode = FollowMode.SILENT_FOLLOW;
    }

    public void SetAttackTarget(Creature creature)
    {
        enemy = creature;
        mode = FollowMode.ATTACK_FOLLOW;
    }

    
}
