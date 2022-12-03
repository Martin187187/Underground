using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Text;


public class Inventar : MonoBehaviour
{
    public TextMeshProUGUI buildmode;
    public TextMeshProUGUI inventoryText;

    public Tool tool;
    public Transform viewer;
    public MeshGenerator generator;
    public MeshManager manager;
    public Vector3 selectedPosition = new Vector3(0,0,0);

    public Vector3 selectedDirection = Vector3.zero;

    public Marker marker;

    private bool isLocked = false;
    private float lockData;
    public enum Tool{
        None, Building, BuildEdge, Mark
    };
    public Dictionary<Type, float> typeInventory = new Dictionary<Type, float>();

    void OnEnable(){
        typeInventory.Add(Type.Beton, 10000);
    }
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
                selectedDirection = viewer.transform.TransformDirection(Vector3.forward);
                //generator.mat.SetVector("_Selected", stepPosition);
            }
            else
            {
                selectedPosition = Vector3.zero;
                Debug.DrawRay(viewer.position, viewer.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            }


        }
    }

    void Update(){
        float step = generator.getStep();
        
        if(Input.mouseScrollDelta.y>0){
            tool = (Tool)((int)(tool +1) % Enum.GetNames(typeof(Tool)).Length);
            marker.SetActiveBox(tool == Tool.Mark || marker.isMarked());
            buildmode.SetText(tool.ToString());

        }else if(Input.mouseScrollDelta.y<0){
            tool = (Tool)((Enum.GetNames(typeof(Tool)).Length +(int)tool-1) % Enum.GetNames(typeof(Tool)).Length);
            marker.SetActiveBox(tool == Tool.Mark || marker.isMarked());
            buildmode.SetText(tool.ToString());
        }
        
        if(tool == Tool.Mark){
            marker.selectedPosition = selectedPosition;
            if(Input.GetMouseButton(1)){
                marker.Reset();
            }
        }
        
        if(Input.GetMouseButtonDown(0)&&!selectedPosition.Equals(Vector3.zero)&&Tool.Mark==tool&& marker.needSelected()){
            marker.AddPosition(selectedPosition);
        } else if(Input.GetMouseButtonDown(0)&&Tool.Mark==tool&& !marker.needSelected()){
            marker.AddPosition(selectedPosition);
        }
        
        
        if((tool==Tool.Building||tool==Tool.BuildEdge)&&!selectedPosition.Equals(Vector3.zero)){

            if(Input.GetMouseButton(0)){
                if(marker.isMarked())
                    manager.DeleteTerrain(selectedPosition, step*2, 0.4f, typeInventory, tool==Tool.Building ? marker.isInBox : marker.isAtEdge);
                else
                    manager.DeleteTerrain(selectedPosition, step*2, 0.4f, typeInventory);
            }
            
            if(Input.GetMouseButton(1)){
                if(marker.isMarked())
                    manager.AddTerrain(selectedPosition, selectedDirection, step*2, 0.1f, typeInventory, tool==Tool.Building ? marker.isInBox : marker.isAtEdge);
                else
                    manager.AddTerrain(selectedPosition, selectedDirection, step*2, 0.1f, typeInventory);
     
            }
        }
        //inventory display
        
        StringBuilder builder = new StringBuilder();
        List<Type> types = new List<Type>(typeInventory.Keys);
        for(int i = 0; i < typeInventory.Count; i++){
            Type type = types[i];
            builder.Append(type.ToString()).Append(": ").Append(typeInventory[type]).AppendLine();
        }
        
        inventoryText.SetText(builder.ToString());
        
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube (selectedPosition, Vector3.one);
        
    }
    
    
}
