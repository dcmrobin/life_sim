using UnityEngine;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
    public List<int> immunityIDs = new List<int>();
    public List<Sickness> sicknesses = new List<Sickness>();
    int contractedSicknesses;
    SicknessNameGenerator sicknessNameGenerator = new SicknessNameGenerator();
    public int generation;
    public bool isPredator;
    public bool isSick;
    private bool hasSymptoms;
    public bool isFull;
    public bool isFleeing;
    public bool isChasing;
    public bool isEating;
    public bool isMating;
    public float speed;
    public float strength;
    public float originalSpeed;
    public float originalStrength;
    public float detectionRadius;
    public Transform targetPrey;
    public Transform targetResource;
    public LayerMask preyMask;
    public LayerMask predatorMask;
    public LayerMask resourceMask;
    public float lifetime = 10f; // Lifetime of prey in seconds
    public GameObject planeObject; // Reference to the plane object for movement constraints
    public GameObject controller;
    public GameObject infoPanel;
    public GameObject mutationSpherePrefab;
    Creature[] allCreatures;
    GameObject[] allResources;
    public GameObject mate;

    private Vector3 initialPosition;
    private Vector3 roamDestination;
    public bool isRoaming;
    public bool tryMate;
    private bool showingInfo;

    private List<Collider> preyList = new List<Collider>();
    private List<Collider> predatorList = new List<Collider>();
    private List<Collider> resourceList = new List<Collider>();

    private Vector3 mutationSpherePosition;
    private float mutationSphereSize;
    private Color mutationSphereColor;

    private float currentLifetime;
    public int eatenResources = 0;

    InfoPanel infoValues;

    private void Start()
    {
        originalSpeed = speed;
        originalStrength = strength;
        //sicknesses = new Sickness[controller.GetComponent<GameController>().maxSicknessID];
        //sicknesses = controller.GetComponent<GameController>().sicknesses;
        initialPosition = transform.position;
        SetNewRoamDestination();
        currentLifetime = lifetime;
    }

    private void Update()
    {
        allCreatures = FindObjectsOfType<Creature>();
        allResources = GameObject.FindGameObjectsWithTag("Resource");
        showingInfo = infoPanel.activeSelf;
        if (infoValues != null)
        {
            if (infoValues.selectedCreature == gameObject && showingInfo)
            {
                infoValues.curlifeTime.text = currentLifetime.ToString();
                infoValues.sicknesses.text = GetSicknessesAsString();
                infoValues.isSick.SetActive(isSick);
                infoValues.isFull.SetActive(isFull);
                infoValues.lifeTime.text = lifetime.ToString();
                infoValues.speed.text = speed.ToString();
                infoValues.strength.text = strength.ToString();
            }
        }
        

        if (!controller.GetComponent<GameController>().paused)
        {
            if (sicknesses.Count > 0)
            {
                for (int i = 0; i < sicknesses.Count; i++)
                {
                    sicknesses[i].duration -= Time.deltaTime;
                    if (sicknesses[i].duration <= 0)
                    {
                        sicknesses.RemoveAt(i);
                        if (sicknesses.Count <= 0)
                        {
                            isSick = false;
                            speed = originalSpeed;
                            strength = originalStrength;
                        }
                    }
                }
            }
        }

        if (!controller.GetComponent<GameController>().paused)
        {
            if (isPredator)
            {
                //preyList.Clear();
                //Collider[] preyColliders = Physics.OverlapSphere(transform.position, detectionRadius, preyMask);
                //preyList.AddRange(preyColliders);

                //if (preyList.Count > 0)
                //{
                //    targetPrey = preyList[0].transform;
                //}

                Transform closestPrey = FindClosestPrey();

                if (closestPrey != null)
                {
                    // SURROUND LOGIC
                    // Check if there are other predators nearby
                    /*
                    List<Transform> nearbyPredators = new List<Transform>();
                    Collider[] predatorColliders = Physics.OverlapSphere(transform.position, detectionRadius, predatorMask);
                    bool reachedDest = false;
                    foreach (var collider in predatorColliders)
                    {
                        if (collider.transform != this.transform)
                        {
                            nearbyPredators.Add(collider.transform);
                        }
                    }

                    if (nearbyPredators.Count > 0)
                    {
                        // Coordinate with nearby predators to surround the prey
                        Vector3 averagePosition = Vector3.zero;
                        foreach (var predator in nearbyPredators)
                        {
                            averagePosition += predator.position;
                        }
                        averagePosition /= nearbyPredators.Count;

                        // Calculate the direction to the prey and move towards it
                        Vector3 directionToPrey = (closestPrey.position - averagePosition).normalized;
                        transform.LookAt(transform.position + directionToPrey);
                        if (transform.position != directionToPrey && !reachedDest)
                        {
                            transform.position += transform.forward * speed * Time.deltaTime;
                        }
                        else
                        {
                            reachedDest = true;
                            targetPrey = closestPrey;
                        }
                    }
                    else
                    {
                        // If no nearby predators, behave as before and chase the closest prey
                        targetPrey = closestPrey;
                        // Rest of the chasing logic remains the same as before
                    }*/

                    targetPrey = closestPrey;
                }

                if (targetPrey != null)
                {
                    isChasing = true;
                    isRoaming = false;
                    transform.LookAt(targetPrey.position);
                    transform.position += transform.forward * speed * Time.deltaTime;

                    if (Vector3.Distance(transform.position, targetPrey.position) <= 1f)
                    {
                        if (targetPrey.GetComponent<Creature>().isSick)
                        {
                            //int randomSicknessID = Random.Range(0, targetPrey.GetComponent<Creature>().contractedSicknesses); // Assuming 1 is the minimum sickness ID
                            for (int i = 0; i < targetPrey.GetComponent<Creature>().sicknesses.Count; i++)
                            {
                                if (!immunityIDs.Contains(targetPrey.GetComponent<Creature>().sicknesses[i].sicknessID) && !hasSymptoms)
                                {
                                    hasSymptoms = true;
                                    isSick = true;
                                    //sicknesses.Capacity = contractedSicknesses;
                                    sicknesses = new List<Sickness>();
                                    int a = 0;
                                    while (a < targetPrey.GetComponent<Creature>().contractedSicknesses)
                                    {
                                        sicknesses.Add(new Sickness(0, "", 0, 0));
                                        a++;
                                    }
                                    for (int y = 0; y < sicknesses.Count; y++)
                                    {
                                        sicknesses[y] = controller.GetComponent<GameController>().sicknesses[y];
                                    }
                                    // Apply the sickness effects on stats (e.g., half the speed and strength)
                                    speed *= controller.GetComponent<GameController>().sicknesses[targetPrey.GetComponent<Creature>().sicknesses[i].sicknessID].statReductionFactor;
                                    strength *= controller.GetComponent<GameController>().sicknesses[targetPrey.GetComponent<Creature>().sicknesses[i].sicknessID].statReductionFactor;
                                }
                            }
                        }
                        Destroy(targetPrey.gameObject);
                        currentLifetime += 3;
                        targetPrey = null;
                        SetNewRoamDestination();
                        isChasing = false;
                    }
                }
                else if (!isRoaming)
                {
                    isChasing = false;
                    SetNewRoamDestination();
                }
            }
            else
            {
                predatorList.Clear();
                Collider[] predatorColliders = Physics.OverlapSphere(transform.position, detectionRadius, predatorMask);
                predatorList.AddRange(predatorColliders);

                /*resourceList.Clear();
                Collider[] resourceColliders = Physics.OverlapSphere(transform.position, detectionRadius, resourceMask);
                resourceList.AddRange(resourceColliders);

                if (resourceList.Count > 0)
                {
                    targetResource = resourceList[0].transform;
                }*/
                Transform closestResource = FindClosestResource();

                if (closestResource != null)
                {
                    targetResource = closestResource;
                    // Rest of the chasing logic remains the same as before
                }

                if (eatenResources >= 2)
                {
                    isFull = true;
                    MateWithAnotherPrey();
                }

                if (predatorList.Count > 0 && !tryMate)
                {
                    isFleeing = true;
                    isEating = false;
                    isRoaming = false;
                    Transform predator = predatorList[Random.Range(0, predatorList.Count)].transform;
                    Vector3 fleeDirection = transform.position - predator.position;
                    transform.LookAt(transform.position + fleeDirection);
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
                else if (targetResource != null && !tryMate && !isFull)
                {
                    isFleeing = false;
                    isEating = true;
                    isRoaming = false;
                    transform.LookAt(targetResource.position);
                    transform.position += transform.forward * speed * Time.deltaTime;

                    if (Vector3.Distance(transform.position, targetResource.position) <= 1f)
                    {
                        float sicknessChance = 0.037f;
                        // Determine if the resource carries a sickness
                        bool carriesSickness = Random.value < sicknessChance;

                        if (carriesSickness)
                        {
                            contractedSicknesses += 1;
                            int randomSicknessID = Random.Range(0, contractedSicknesses); // Assuming 1 is the minimum sickness ID

                            // Check if the prey is immune to the sickness
                            if (!immunityIDs.Contains(randomSicknessID) && !hasSymptoms)
                            {
                                hasSymptoms = true;
                                isSick = true;
                                //sicknesses.Capacity = contractedSicknesses;
                                sicknesses = new List<Sickness>();
                                int a = 0;
                                while (a < contractedSicknesses)
                                {
                                    sicknesses.Add(new Sickness(0, "", 0, 0));
                                    a++;
                                }
                                for (int i = 0; i < sicknesses.Count; i++)
                                {
                                    sicknesses[i] = controller.GetComponent<GameController>().sicknesses[i];
                                }
                                // Apply the sickness effects on stats (e.g., half the speed and strength)
                                speed *= controller.GetComponent<GameController>().sicknesses[randomSicknessID].statReductionFactor;
                                strength *= controller.GetComponent<GameController>().sicknesses[randomSicknessID].statReductionFactor;
                            }
                        }

                        Destroy(targetResource.gameObject);
                        eatenResources += 1;
                        targetResource = null;
                        SetNewRoamDestination();
                        isEating = false;
                        hasSymptoms = false;
                    }
                }
                else if (!isRoaming && !tryMate)
                {
                    SetNewRoamDestination();
                }
            }

            if (isRoaming)
            {
                Quaternion targetRotation = Quaternion.LookRotation(roamDestination - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

                transform.position += transform.forward * speed * Time.deltaTime;

                if (Vector3.Distance(transform.position, roamDestination) <= 0.5f)
                {
                    isRoaming = false;
                }
            }

            // Constrain movement within the plane object bounds
            Vector3 clampedPosition = new Vector3(
                Mathf.Clamp(transform.position.x, planeObject.transform.position.x - planeObject.transform.localScale.x / 2f, planeObject.transform.position.x + planeObject.transform.localScale.x / 2f),
                transform.position.y,
                Mathf.Clamp(transform.position.z, planeObject.transform.position.z - planeObject.transform.localScale.z / 2f, planeObject.transform.position.z + planeObject.transform.localScale.z / 2f)
            );
            transform.position = clampedPosition;

            // Decrease the current lifetime
            if (isPredator || isSick)
            {
                currentLifetime -= Time.deltaTime;
            }

            if (currentLifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void SetNewRoamDestination()
    {
        float roamRadius = 10f;
        Vector3 minBounds = planeObject.transform.position - planeObject.transform.localScale / 2f;
        Vector3 maxBounds = planeObject.transform.position + planeObject.transform.localScale / 2f;

        roamDestination = new Vector3(
            Mathf.Clamp(initialPosition.x + Random.Range(-roamRadius, roamRadius), minBounds.x, maxBounds.x),
            initialPosition.y,
            Mathf.Clamp(initialPosition.z + Random.Range(-roamRadius, roamRadius), minBounds.z, maxBounds.z)
        );

        isFleeing = false;
        isEating = false;
        isRoaming = true;
    }

    private void MateWithAnotherPrey()
    {
        bool closeEnough = false;

        float transmissionChance = 0.8f;
        float immunityChance = 0.05f;
        //float inheritChance = 0.9f;
        // Find another prey in the scene that has also eaten enough resources to mate
        List<Creature> potentialMates = new List<Creature>();
    
        foreach (Creature prey in allCreatures)
        {
            if (prey != this && prey.eatenResources >= 2 && prey.gameObject.layer == 6) // Exclude self and check eatenResources count
            {
                potentialMates.Add(prey);
            }
        }

        // If there's at least one potential mate, choose a random one and mate
        if (potentialMates.Count > 0)
        {
            tryMate = true;
            Creature mate = potentialMates[Random.Range(0, potentialMates.Count)];
            transform.LookAt(mate.transform.position);
            if (Vector3.Distance(transform.position, mate.transform.position) <= 0.5f)
            {
                closeEnough = true;
            }
            else
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }

            if (closeEnough)
            {
                // Instantiate a new prey at the mating position
                GameObject newPrey = Instantiate(gameObject, transform.position, Quaternion.identity);
                Creature newPreyScript = newPrey.GetComponent<Creature>();
                newPreyScript.generation += 1;
                newPrey.name = "Prey Generation " + newPreyScript.generation;
                newPrey.name = newPrey.name.Replace("(Clone)", "");

                // Reset eatenResources counts for both mates and the newborn prey
                eatenResources = 0;
                isFull = false;
                mate.eatenResources = 0;
                mate.isFull = false;
                mate.tryMate = false;
                if (isSick)
                {
                    mate.GetComponent<Creature>().isSick = true;
                    if (Random.value < transmissionChance)
                    {
                        newPreyScript.isSick = true;
                        newPreyScript.sicknesses = sicknesses;
                    }
                }

                if (Random.value < immunityChance && immunityIDs.Count < controller.GetComponent<GameController>().maxSicknessID)
                {
                    immunityIDs.Add(Random.Range(0, controller.GetComponent<GameController>().maxSicknessID));
                }
                //if (Random.value < inheritChance)
                //{
                    newPreyScript.lifetime = lifetime;
                    newPreyScript.speed = speed;
                    newPreyScript.strength = strength;
                //}
                newPreyScript.speed = MutateProperty(Mathf.RoundToInt(newPreyScript.speed), Random.Range(0.01f, 0.1f), 3);
                newPreyScript.strength = MutateProperty(Mathf.RoundToInt(newPreyScript.strength), Random.Range(0.01f, 0.1f), 3);
                newPreyScript.lifetime = MutateProperty(Mathf.RoundToInt(newPreyScript.lifetime), Random.Range(0.01f, 0.1f), 4);

                newPreyScript.eatenResources = 0;
                newPreyScript.isFull = false;
                newPreyScript.tryMate = false;

                // Set new random destinations for all three preys
                tryMate = false;
                SetNewRoamDestination();
                mate.SetNewRoamDestination();
                newPreyScript.SetNewRoamDestination();
            }
        }
        else
        {
            SetNewRoamDestination();
        }
    }

    private void OnMouseDown() {
        infoValues = infoPanel.GetComponent<InfoPanel>();

        infoValues.selectedCreature = gameObject;
        infoValues.fpCam.target = gameObject;
        infoValues.creatureName.text = gameObject.name;
        infoValues.lifeTime.text = lifetime.ToString();
        infoValues.speed.text = speed.ToString();
        infoValues.strength.text = strength.ToString();
        infoValues.isSick.SetActive(isSick);
        infoValues.isFull.SetActive(isFull);

        infoPanel.SetActive(false);
        infoPanel.SetActive(true);
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

    public string GetSicknessesAsString()
    {
        string sicknessesString = "";

        for (int i = 0; i < sicknesses.Count; i++)
        {
            sicknessesString += sicknesses[i].sicknessName;

            if (i < sicknesses.Count - 1)
            {
                sicknessesString += ", ";
            }
        }

        return sicknessesString;
    }

    private Transform FindClosestPrey()
    {
        float closestDistance = detectionRadius;
        Transform closestPrey = null;

        foreach (Creature creature in allCreatures)
        {
            if (creature != this && creature.isMating == false && creature.gameObject.layer == 6)
            {
                float distance = Vector3.Distance(transform.position, creature.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPrey = creature.transform;
                }
            }
        }

        return closestPrey;
    }

    private Transform FindClosestResource()
    {
        float closestDistance = detectionRadius;
        Transform closestResource = null;

        foreach (GameObject resource in allResources)
        {
            if (resource.gameObject.layer == 8)
            {
                float distance = Vector3.Distance(transform.position, resource.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestResource = resource.transform;
                }
            }
        }

        return closestResource;
    }
}

[System.Serializable]
public class Sickness
{
    public int sicknessID;
    public string sicknessName;
    public float duration;
    public float statReductionFactor; // Factor by which stats are reduced when infected

    public Sickness(int id, string name, float dur, float factor)
    {
        sicknessID = id;
        sicknessName = name;
        duration = dur;
        statReductionFactor = factor;
    }
}