using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject preyPrefab;
    public GameObject predatorPrefab;
    public GameObject resourceObject;
    public GameObject planeObject;
    public GameObject infoPanel;
    public float spawnInterval = 3f;
    public float spawnRadius = 10f;
    public float preyChance = 0.5f;

    private Collider planeCollider;

    private void Start()
    {
        // Get the collider of the plane object
        planeCollider = planeObject.GetComponent<Collider>();

        // Start spawning creatures at regular intervals
        InvokeRepeating("SpawnCreature", 0f, spawnInterval);
        InvokeRepeating("SpawnResource", 0f, spawnInterval);
    }

    private void SpawnCreature()
    {
        if (!GetComponent<GameController>().paused)
        {
            // Generate a random position within the bounds of the plane object
            Vector3 randomPosition = GetRandomPosition();
    
            // Instantiate the creature prefab at the random position
            GameObject creaturePrefab = GetRandomCreaturePrefab();
            GameObject creature = Instantiate(creaturePrefab, new Vector3(randomPosition.x, 0, randomPosition.z), Quaternion.identity);
            //Debug.Log(creature.GetComponent<Creature>().lifetime);
            //Debug.Log(creature.GetComponent<Creature>().currentLifetime);
            creature.name = creature.name.Replace("(Clone)", " Generation 1");
            creature.GetComponent<Creature>().planeObject = planeObject;
            creature.GetComponent<Creature>().controller = gameObject;
            creature.GetComponent<Creature>().infoPanel = infoPanel;
        }
    }

    private void SpawnResource()
    {
        if (!GetComponent<GameController>().paused)
        {
            // Generate a random position within the bounds of the plane object
            Vector3 randomPosition = GetRandomPosition();
            GameObject newResource = Instantiate(resourceObject, new Vector3(randomPosition.x/2, 0, randomPosition.z/2), Quaternion.identity);
            newResource.name = newResource.name.Replace("(Clone)", "");
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Get the bounds of the plane collider
        Bounds bounds = planeCollider.bounds;

        // Generate random x and z coordinates within the bounds
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        // Create a random position within the specified radius around the random x and z coordinates at y-level 0
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);
        randomPosition += Random.insideUnitSphere * spawnRadius;

        return randomPosition;
    }

    private GameObject GetRandomCreaturePrefab()
    {
        // Randomly determine the creature type based on the preyChance
        if (Random.value < preyChance)
        {
            // Return the prey prefab
            return preyPrefab;
        }
        else
        {
            // Return the predator prefab
            return predatorPrefab;
        }
    }
}
