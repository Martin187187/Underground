using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootSolver2 : MonoBehaviour
{
    public Transform body;
    public Transform castPosition;
    public Transform tip;
    public float maxStepLength;
    public float stepDistance = 1f;
    public float speed = 1f;
    public float stepHeight = 0.3f;

    private Vector3 currentPosition, oldPosition, newPosition;
    private float lerp;

    private LayerMask mask;
    private bool isJump = false;
    private float counter = 1;
    
    public List<IKFootSolver2> brothers = new List<IKFootSolver2>();

    public bool run = true;
    // Start is called before the first frame update
    void Start()
    {
        newPosition = body.position + castPosition.position;
        mask = LayerMask.GetMask("Terrain");
    }


    // Update is called once per frame
    public void Run(bool IsStunned)
    {
        if(IsStunned)
        {
            oldPosition = this.transform.position;
            isJump = false;
            lerp = 0;
            return;
        }
        
        this.transform.position = currentPosition;

        Ray ray = new Ray(castPosition.position + body.forward*stepDistance*.33f + Vector3.up, Vector3.down);
        
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask)&&info.distance<10f){
            
            if(IsReady() && Vector3.Distance(newPosition, info.point) > stepDistance)
            {
                isJump = true;
                oldPosition = this.transform.position;
                float distance = Vector3.Distance(tip.position, info.point);
                float scale = Mathf.Min(maxStepLength/distance,1);
                Vector3 direction = info.point - tip.position;

                newPosition = tip.position + direction * scale;
                lerp = 0;
            }
        }

        if(lerp < 1)
        {
            Vector3 footPosition = Vector3.Slerp(oldPosition, newPosition, lerp);
            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = footPosition;
            lerp += Time.deltaTime * speed;
        }
        else
        {
            isJump = false;
            oldPosition = newPosition;
        }
        
        
    }

    public bool IsJump()
    {
        return isJump;
    }

    private bool IsReady(){
        if(IsJump())
            return false;
        foreach(IKFootSolver2 solver in brothers)
        {
            if(solver.IsJump())
                return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = body.forward*stepDistance*.33f;
        Gizmos.DrawSphere(castPosition.position + direction, 0.2f);
    }
}
