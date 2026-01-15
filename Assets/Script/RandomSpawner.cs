using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{
    [Header("Prefabs to appear")]
    [Tooltip("List of prefab with random spawn ")]
    public GameObject[] prefabs;

    [Header("Parameter spawn")]
    [Tooltip("Minimum_time_spawn")]
    public float minSpawnTime = 3f;
    [Tooltip("Maximum_time_spawn")]
    public float maxSpawnTime = 10f;
    [Tooltip("Minimum_object_spawn")]
    public int minObjectsPerSpawn = 1;
    [Tooltip("Maximum_object_spawn")]
    public int maxObjectsPerSpawn = 3;

    private BoxCollider spawnArea;

    private void Awake()
    {
        // get BoxCollider component
        spawnArea = GetComponent<BoxCollider>();

        // error if no BoxCollider found
        if (spawnArea == null)
        {
            Debug.LogError("No BoxCollider found");
        }
    }

    private void Start()
    {
        // start spawn routine
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // random time to wait for spawn
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // random spawn 
            int objectCount = Random.Range(minObjectsPerSpawn, maxObjectsPerSpawn + 1);

            // spawn objects
            for (int i = 0; i < objectCount; i++)
            {
                SpawnRandomObject();
            }
        }
    }

    private void SpawnRandomObject()
    {
        // Check if prefab available
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("No prefab given");
            return;
        }

        // Random prefab choose
        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

        // Random position spawn
        Vector3 spawnPos = GetRandomPointInBox(spawnArea);

        // Creation of the object
        GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

    }

    private Vector3 GetRandomPointInBox(BoxCollider box)
    {
        // calcul limitation point random spawn
        Vector3 center = box.center + transform.position;
        Vector3 size = box.size * 0.5f;

        float x = Random.Range(center.x - size.x, center.x + size.x);
        float y = Random.Range(center.y - size.y, center.y + size.y);
        float z = Random.Range(center.z - size.z, center.z + size.z);

        return new Vector3(x, y, z);
    }
}
