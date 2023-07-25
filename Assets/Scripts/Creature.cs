using UnityEngine;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
    public int generation;
    public bool isPredator;
    public bool isSick;
    private bool hasSymptoms;
    public bool isFull;
    public bool isFleeing;
    public bool isChasing;
    public bool isEating;
    public bool isMating;
    public bool mutated;
    public float speed;
    public float strength;
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
    Creature[] allPreys;
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
        initialPosition = transform.position;
        SetNewRoamDestination();
        currentLifetime = lifetime;
    }

    private void Update()
    {
        allPreys = FindObjectsOfType<Creature>();
        showingInfo = infoPanel.activeSelf;
        if (infoValues != null)
        {
            infoValues.curlifeTime.text = currentLifetime.ToString();
        }

        if (!controller.GetComponent<GameController>().paused)
        {
            if (isPredator)
            {
                preyList.Clear();
                Collider[] preyColliders = Physics.OverlapSphere(transform.position, detectionRadius, preyMask);
                preyList.AddRange(preyColliders);

                if (preyList.Count > 0)
                {
                    targetPrey = preyList[0].transform;
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
                            isSick = true;
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

                resourceList.Clear();
                Collider[] resourceColliders = Physics.OverlapSphere(transform.position, detectionRadius, resourceMask);
                resourceList.AddRange(resourceColliders);

                if (resourceList.Count > 0)
                {
                    targetResource = resourceList[0].transform;
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
                        float sicknessChance = 0.005f;
                        if (Random.value < sicknessChance)
                        {
                            isSick = true;
                        }

                        Destroy(targetResource.gameObject);
                        eatenResources += 1;
                        targetResource = null;
                        SetNewRoamDestination();
                        isEating = false;
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

            if (isSick && !hasSymptoms)
            {
                speed /= 2;
                strength /= 2;
                lifetime /= 2;
                hasSymptoms = true;
            }

            // Decrease the current lifetime
            if (isPredator)
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

        float transmissionChance = 0.5f;
        //float inheritChance = 0.9f;
        // Find another prey in the scene that has also eaten enough resources to mate
        List<Creature> potentialMates = new List<Creature>();
    
        foreach (Creature prey in allPreys)
        {
            if (prey != this && prey.eatenResources >= 2) // Exclude self and check eatenResources count
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
                SpawnMutationSphere(newPrey.transform);
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
                if (Random.value < transmissionChance && isSick == true)
                {
                    mate.GetComponent<Creature>().isSick = true;
                }
                //if (Random.value < inheritChance)
                //{
                    newPreyScript.lifetime = lifetime;
                    newPreyScript.speed = speed;
                    newPreyScript.strength = strength;
                    if (isSick)
                    {
                        newPreyScript.isSick = true;
                    }
                //}
                newPreyScript.speed = MutateProperty(Mathf.RoundToInt(newPreyScript.speed), 0.1f, 2, newPreyScript.mutated);
                newPreyScript.strength = MutateProperty(Mathf.RoundToInt(newPreyScript.strength), 0.1f, 2, newPreyScript.mutated);
                newPreyScript.lifetime = MutateProperty(Mathf.RoundToInt(newPreyScript.lifetime), 0.1f, 2, newPreyScript.mutated);

                if (mutated)
                {
                    GameObject mutationSphere = Instantiate(mutationSpherePrefab, newPrey.transform);
                    mutationSphere.transform.localPosition = mutationSpherePosition;
                    mutationSphere.transform.localScale = new Vector3(mutationSphereSize, mutationSphereSize, mutationSphereSize);
                    mutationSphere.GetComponent<MeshRenderer>().material.color = mutationSphereColor;
                }

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
    }

    private void OnMouseDown() {
        infoValues = infoPanel.GetComponent<InfoPanel>();

        infoValues.selectedCreature = gameObject;
        infoValues.fpCam.target = gameObject;
        infoValues.creatureName.text = gameObject.name;
        infoValues.isSick.SetActive(isSick);
        infoValues.isFull.SetActive(isFull);
        infoValues.lifeTime.text = lifetime.ToString();
        infoValues.speed.text = speed.ToString();
        infoValues.strength.text = strength.ToString();

        infoPanel.SetActive(false);
        infoPanel.SetActive(true);
    }

    private float MutateProperty(int originalValue, float mutationChance, int maxMutationAmount, bool mutateBool)
    {
        if (Random.value < mutationChance)
        {
            mutateBool = true;
            return originalValue + Random.Range(-maxMutationAmount, maxMutationAmount);
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
