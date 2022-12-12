using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    
    public Transform cam;
    public Transform pos;
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        //transform.position = pos.position+ Vector3.up;
    }
}
