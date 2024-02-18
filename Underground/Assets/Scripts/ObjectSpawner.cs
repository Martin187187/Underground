using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public float weight;
    }

    public SpawnableObject[] spawnableObjects;
    public int numberOfObjectsToSpawn = 10;
    public float spawnRange = 100f;
    public float minScale = 4f;
    public float maxScale = 8f;

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-spawnRange, spawnRange), 100f, Random.Range(-spawnRange, spawnRange));
            RaycastHit hit;
            float spawn = Mathf.PerlinNoise(spawnPosition.x*0.005f, spawnPosition.z*0.005f);
            float random = Random.Range(0f, 0.5f);
            if (spawn < random && Physics.Raycast(new Vector3(spawnPosition.x, 100f, spawnPosition.z), Vector3.down, out hit))
            {
                if (hit.collider.tag == "Terrain")
                {
                    spawnPosition.y = hit.point.y;
                    SpawnableObject obj = ChooseRandomObject();
                    if (obj != null)
                    {
                        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                        Vector3 randomScale = Vector3.one * Random.Range(minScale, maxScale);
                        GameObject spawnedObject = Instantiate(obj.prefab, spawnPosition, randomRotation);
                        spawnedObject.transform.localScale = randomScale;
                    }
                }
            }
        }
    }

    SpawnableObject ChooseRandomObject()
    {
        float totalWeight = 0f;
        foreach (SpawnableObject obj in spawnableObjects)
        {
            totalWeight += obj.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (SpawnableObject obj in spawnableObjects)
        {
            randomValue -= obj.weight;
            if (randomValue <= 0f)
            {
                return obj;
            }
        }

        return null;
    }
}
