using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    public Vector3Int coord;
    [HideInInspector] public ChunkData chunkData = new ChunkData();

    [HideInInspector]
    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    bool generateCollider;

    [HideInInspector] public bool isDirty = false;

    public void addPrefab(GameObject obj, Vector3 pos){
        GameObject instance = Instantiate(obj, pos, Quaternion.identity);
        instance.transform.parent = this.transform;
    }

    public void clearPrefabs(){
        
        if (Application.isPlaying )
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject); 
            }
        else
            foreach (Transform child in transform)
         {
             StartCoroutine(Destroy(child.gameObject));
         }
    }
    IEnumerator Destroy(GameObject go)
     {
        yield return null;
        DestroyImmediate(go);
     }
    public void DestroyOrDisable () {
        if (Application.isPlaying) {
            mesh.Clear ();
            gameObject.SetActive (false);
        } else {
            DestroyImmediate (gameObject, false);
        }
    }

    public void Reset(){
        meshFilter = GetComponent<MeshFilter> ();
        if(meshFilter != null){
            meshFilter.sharedMesh = null;
            meshFilter.mesh = null;
        }
        
        meshCollider = GetComponent<MeshCollider> ();
        if(meshCollider != null){
            meshCollider.sharedMesh = null;
        }
        
    }

    // Add components/get references in case lost (references can be lost when working in the editor)
    public void SetUp (Material[] mat, bool generateCollider) {
        this.generateCollider = generateCollider;

        meshFilter = GetComponent<MeshFilter> ();
        meshRenderer = GetComponent<MeshRenderer> ();
        meshCollider = GetComponent<MeshCollider> ();

        if (meshFilter == null) {
            meshFilter = gameObject.AddComponent<MeshFilter> ();
        }

        if (meshRenderer == null) {
            meshRenderer = gameObject.AddComponent<MeshRenderer> ();
        }

        if (meshCollider == null && generateCollider) {
            meshCollider = gameObject.AddComponent<MeshCollider> ();
        }
        if (meshCollider != null && !generateCollider) {
            DestroyImmediate (meshCollider);
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null) {
            mesh = new Mesh ();
            
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }

        if (generateCollider) {
            if (meshCollider.sharedMesh == null) {
                meshCollider.sharedMesh = mesh;
                
            }
            // force update
            meshCollider.enabled = false;
            meshCollider.enabled = true;

        }

        meshRenderer.materials = mat;
    }
}