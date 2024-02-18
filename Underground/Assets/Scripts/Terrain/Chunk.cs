using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector3Int coord;
    [HideInInspector] public ChunkData chunkData = new ChunkData();

    [HideInInspector]
    public Mesh mesh;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
    bool generateCollider;


    [HideInInspector] public bool isDirty = false;

    void Start()
    {
        LayerMask mask = LayerMask.NameToLayer("Terrain");
        gameObject.layer = mask;
        Physics.IgnoreLayerCollision(LayerMask.GetMask("default"), mask);
    }
    public void addPrefab(GameObject obj, Vector3 pos)
    {
        GameObject instance = Instantiate(obj, pos, Quaternion.identity);
        instance.transform.parent = this.transform;
    }

    public void clearPrefabs()
    {
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
    public void DestroyOrDisable()
    {
        clearPrefabs();
        if (Application.isPlaying)
        {
            mesh.Clear();
            gameObject.SetActive(false);
        }
        else
        {
            DestroyImmediate(gameObject, false);
        }

    }

    public void Reset()
    {
        clearPrefabs();
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = null;
            meshFilter.mesh = null;
        }

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
        }
    }
public static float NextDouble(float mean, float stdDev, System.Random rand)
{

    float u1 = 1.0f - (float)rand.NextDouble(); // Uniform(0,1] random doubles
    float u2 = 1.0f - (float)rand.NextDouble();
    float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                          Mathf.Sin(2.0f * Mathf.PI * u2); // Box-Muller transform

    return mean + stdDev * randStdNormal;
}
public static float NextFloat(float min, float max, System.Random rand)
{
    return (float)rand.NextDouble() * (max - min) + min;
}

private void CreateObjects(Chunk chunk)
{   
    clearPrefabs();
    var generator = FindObjectOfType<MeshGenerator>();
    Vector3 start = generator.CentreFromCoord(chunk.coord);
    int seed = chunk.coord.GetHashCode();
    System.Random rand = new System.Random(seed);

    for (int i = 0; i < 8; i++)
    {
        Vector3 spawnPosition = new Vector3(
            NextFloat(start.x - generator.boundsSize / 2, start.x + generator.boundsSize / 2, rand), 
            start.y + generator.boundsSize / 2, 
            NextFloat(start.z - generator.boundsSize / 2, start.z + generator.boundsSize / 2, rand)
            );
        
        RaycastHit hit;
        float spawn = UniformNoise.calculate(spawnPosition.x * 0.001f, spawnPosition.z * 0.001f, seed) 
                      //+ NextDouble(0f, 0.1f, rand)
                      ;
        if (NextFloat(0f, 1f, rand) < spawn && Physics.Raycast(spawnPosition, Vector3.down, out hit))
        {
            if (hit.collider == meshCollider)
            {
                spawnPosition.y = hit.point.y;
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Tree_3");
                GameObject cube = Instantiate(prefab, Vector3.zero, Quaternion.identity);

                Quaternion randomRotation = Quaternion.Euler(0f, NextFloat(0f, 360f, rand), 0f);
                Vector3 randomScale = Vector3.one * NextFloat(4, 8, rand);
                cube.transform.position = spawnPosition;
                cube.transform.rotation = randomRotation;
                cube.transform.localScale = randomScale;
                cube.transform.SetParent(chunk.transform);
            }
        }
    }
}

    // Add components/get references in case lost (references can be lost when working in the editor)
    public void SetUp(Material[] mat, bool generateCollider)
    {
        this.generateCollider = generateCollider;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }

        if (meshCollider == null && generateCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        if (meshCollider != null && !generateCollider)
        {
            DestroyImmediate(meshCollider);
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();

            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshFilter.sharedMesh = mesh;
        }

        if (generateCollider)
        {
            if (meshCollider.sharedMesh == null)
            {
                meshCollider.sharedMesh = mesh;

            }
            // force update
            meshCollider.enabled = false;
            meshCollider.enabled = true;

        }

        meshRenderer.materials = mat;
        CreateObjects(this);

    }
}