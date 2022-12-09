using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    public enum FollowMode
    {
        WAYPOINT, SILENT_FOLLOW, ATTACK_FOLLOW
    };
    public FollowMode mode = FollowMode.SILENT_FOLLOW;
    [SerializeField] private Vector3 waypoint;
    [SerializeField] private Transform follow;
    [SerializeField] private Creature enemy;
    public float speed = 1f;

    public abstract void ActivateAbilityOne();

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
