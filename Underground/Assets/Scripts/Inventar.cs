using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Text;


public class Inventar : MonoBehaviour
{
    public ConcreateFactoryCreature factory;
    public TextMeshProUGUI buildmode;
    public TextMeshProUGUI inventoryText;
    public Transform player;
    public Creature creature;
    public Tool tool;
    public Transform viewer;
    public MeshGenerator generator;
    public MeshManager manager;
    public Vector3 selectedPosition = new Vector3(0,0,0);

    public Vector3 selectedDirection = Vector3.zero;

    public Marker marker;
    public enum Tool{
        None, Building, BuildEdge, Mark
    };
    public Dictionary<Type, float> typeInventory = new Dictionary<Type, float>();


    void OnEnable(){
        creature = factory.CreateCreature("Abra", new Vector3(-11.6953087f,7f,-5.6152215f));
        typeInventory.Add(Type.Beton, 10000);
    }
    void FixedUpdate(){
        //if (tool != Tool.None) {
            
            LayerMask mask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(viewer.position, viewer.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, mask))
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
            

        //}
    }

    void Update(){

        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        /*
        if(creatures.Length< 5)
        {
            float rdmNumber = generator.viewDistance;
            float x = UnityEngine.Random.Range(-rdmNumber, rdmNumber);
            float z = UnityEngine.Random.Range(-rdmNumber, rdmNumber);
            Vector3 randomPos = player.transform.position + new Vector3(x, 20, z);
            LayerMask mask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
            {
            factory.CreateRandomCreature(hit.point);
            }
        }
        */
        for(int i = 0; i < creatures.Length; i++)
        {
            GameObject o = creatures[i];
            Vector3Int chunckIndex = generator.getChunkCoords(o.transform.position);
            if(!generator.IsLoadedChunk(chunckIndex))
            {
                Destroy(o);
            }
        }
        
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

        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            
            if (Physics.Raycast(viewer.position, viewer.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity)){
                if(hit.transform.CompareTag("Creature")){
                    Creature enemy = hit.transform.gameObject.GetComponent<Creature>();
                    creature.SetAttackTarget(enemy);
                }
                else
                    creature.SetFollowPosition(selectedPosition);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftControl)){
            creature.SetFollowTarget(player);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            creature.ActivateAbilityOne();
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            creature.ActivateAbilityTwo();
        }
        if(Vector3.zero != selectedPosition && Input.GetKeyDown(KeyCode.B))
        {
            creature = factory.CreateRandomCreature(selectedPosition);
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
