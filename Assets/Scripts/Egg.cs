using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("Attributes for newborn to inherit")]
    public List<Sickness> sicknesses = new List<Sickness>();
    public bool isFull;
    public bool isSick;
    public float lifetime;
    public float speed;
    public float strength;
    public float detectionRadius;
    public int eatenResources;
    public int generation;

    [Header("Egg attributes")]
    public float timeUntilHatch = 10;
    public GameObject controller;
    public GameObject mutationSpherePrefab;
    public GameObject preyPrefab;
    private Vector3 mutationSpherePosition;
    private float mutationSphereSize;
    private Color mutationSphereColor;

    private void Start() {
        for (int i = 0; i < controller.GetComponent<GameController>().maxSicknessID; i++)
        {
            sicknesses.Add(new Sickness(0, "", 0, 0));
        }
    }

    private void Update() {
        for (int i = 0; i < sicknesses.Count; i++)
        {
            sicknesses[i].sicknessID = controller.GetComponent<GameController>().sicknesses[i].sicknessID;
            sicknesses[i].sicknessName = controller.GetComponent<GameController>().sicknesses[i].sicknessName;
            sicknesses[i].duration = controller.GetComponent<GameController>().sicknesses[i].duration;
            sicknesses[i].statReductionFactor = controller.GetComponent<GameController>().sicknesses[i].statReductionFactor;
        }
        timeUntilHatch -= 0.1f;
        if (timeUntilHatch <= 0)
        {
            SpawnPrey();
        }
    }

    public void SpawnPrey()
    {
        float transmissionChance = 0.8f;

        // Instantiate a new prey at the mating position
        GameObject newPrey = Instantiate(preyPrefab, transform.position, Quaternion.identity);
        Creature newPreyScript = newPrey.GetComponent<Creature>();
        newPreyScript.generation += 1;
        newPreyScript.planeObject = controller.GetComponent<CreatureSpawner>().planeObject;
        newPreyScript.infoPanel = controller.GetComponent<CreatureSpawner>().infoPanel;
        newPreyScript.controller = controller;
        newPrey.name = "Prey Generation " + newPreyScript.generation;
        newPrey.name = newPrey.name.Replace("(Clone)", "");

        if (isSick)
        {
            if (Random.value < transmissionChance)
            {
                newPreyScript.isSick = true;
                newPreyScript.sicknesses = sicknesses;
            }
        }

        newPreyScript.lifetime = lifetime;
        newPreyScript.speed = speed;
        newPreyScript.strength = strength;

        newPreyScript.speed = MutateProperty(Mathf.RoundToInt(newPreyScript.speed), Random.Range(0.01f, 0.1f), 3);
        newPreyScript.strength = MutateProperty(Mathf.RoundToInt(newPreyScript.strength), Random.Range(0.01f, 0.1f), 3);
        newPreyScript.lifetime = MutateProperty(Mathf.RoundToInt(newPreyScript.lifetime), Random.Range(0.01f, 0.1f), 4);
        newPreyScript.detectionRadius = MutateProperty(Mathf.RoundToInt(newPreyScript.detectionRadius), Random.Range(0.01f, 0.1f), 1);

        newPreyScript.eatenResources = 0;
        newPreyScript.isFull = false;
        newPreyScript.SetNewRoamDestination();

        Destroy(gameObject);
    }

    private float MutateProperty(int originalValue, float mutationChance, int maxMutationAmount)
    {
        if (Random.value < mutationChance)
        {
            float randomValue = 0;
            while (randomValue == 0)
            {
                randomValue = Random.Range(Mathf.Round(-maxMutationAmount/2), maxMutationAmount);
            }
            Debug.Log("A mutation has occurred");
            if (randomValue == 0)
            {
                Debug.Log("randomValue is zero!");
            }
            SpawnMutationSphere(transform);
            return originalValue + randomValue;
        }
        return originalValue;
    }

    private void SpawnMutationSphere(Transform preyTransform)
    {
        // Instantiate the mutation sphere prefab
        GameObject mutationSphere = Instantiate(mutationSpherePrefab, preyTransform);

        // Randomly set the size of the mutation sphere (0.5 to 1.5 units)
        float randomSize = Random.Range(0.2f, 0.7f);
        mutationSphere.transform.localScale = Vector3.one * randomSize;

        // Randomly set the color of the mutation sphere
        Renderer sphereRenderer = mutationSphere.GetComponent<Renderer>();

        Color randomColor = new Color(Random.value, Random.value, Random.value);
        sphereRenderer.material.color = randomColor;

        // Randomly set the position of the mutation sphere within the prey
        Vector3 randomPosition = Random.insideUnitSphere * 0.5f;
        mutationSphere.transform.localPosition = randomPosition;

        mutationSpherePosition = mutationSphere.transform.localPosition;
        mutationSphereSize = randomSize;
        mutationSphereColor = randomColor;
    }
}
