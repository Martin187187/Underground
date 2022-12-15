using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    
    private Camera cam;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }    
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
        //transform.position = pos.position+ Vector3.up;
    }
}
