using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Inventar : MonoBehaviour
{
    public Tool tool;
    public Transform viewer;
    public MeshGenerator generator;
    public MeshManager manager;
    public Vector3 selectedPosition = new Vector3(0,0,0);
    private bool isLocked = false;
    private float lockData;
    public enum Tool{
        None, Excevate, Building, ExcevateLimit, BuildingLimit
    };

    void FixedUpdate(){
        if (tool != Tool.None) {
            RaycastHit hit;
            
            int layerMask = 1 << 8;
            
            layerMask = ~layerMask;
            if (Physics.Raycast(viewer.position, viewer.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(viewer.position, viewer.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Vector3 stepPosition = generator.getIndexFromPositionRasterd(hit.point);

                selectedPosition = stepPosition;
                generator.mat.SetVector("_Selected", stepPosition);
            }
            else
            {
                selectedPosition = Vector3.zero;
                Debug.DrawRay(viewer.position, viewer.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            }


        }
    }

    void Update(){

        if(Input.mouseScrollDelta.y>0)
            tool = (Tool)((int)(tool +1) % Enum.GetNames(typeof(Tool)).Length);
        else if(Input.mouseScrollDelta.y<0)
            tool = (Tool)((Enum.GetNames(typeof(Tool)).Length +(int)tool-1) % Enum.GetNames(typeof(Tool)).Length);
        
        
        if(Input.GetMouseButtonDown(0)&&!selectedPosition.Equals(Vector3.zero)&&(tool==Tool.ExcevateLimit||tool==Tool.BuildingLimit)){
            isLocked = true;
            lockData = selectedPosition.y;
        } else if(Input.GetMouseButtonUp(0)||(tool!=Tool.ExcevateLimit&&tool!=Tool.BuildingLimit)){
            isLocked = false;
        }
        
        if(Input.GetMouseButton(0)&&!selectedPosition.Equals(Vector3.zero)){
            int numVoxelsPerAxis = generator.numPointsPerAxis-1;
            float step = generator.boundsSize/numVoxelsPerAxis;

            switch(tool){
                case Tool.Excevate:
                    manager.EditTerrain(selectedPosition, step*2, 0.4f);
                    break;
                case Tool.Building:
                    manager.EditTerrain(selectedPosition, step*2, -0.4f);
                    break;
                case Tool.ExcevateLimit:
                    if(isLocked)
                        manager.EditTerrain(new Vector3(selectedPosition.x, Mathf.Max(lockData+step, selectedPosition.y), selectedPosition.z), step*1.5f, 0.4f);
                    break;
                case Tool.BuildingLimit:
                    if(isLocked)
                        manager.EditTerrain(new Vector3(selectedPosition.x, Mathf.Min(lockData-step, selectedPosition.y), selectedPosition.z), step*1.5f,  -0.4f);
                    break;
                default:
                    break;
            }
        }

    }

    void OnDrawGizmos () {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube (selectedPosition, Vector3.one);
        
    }
    
    
}
