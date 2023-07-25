using UnityEngine;
using System.Collections.Generic;

public class Creature : MonoBehaviour
{
    public bool isPredator;
    public bool isSick;
    private bool hasSymptoms;
    public bool isFull;
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

    private Vector3 initialPosition;
    private Vector3 roamDestination;
    private bool isRoaming;
    private bool tryMate;
    private bool showingInfo;

    private List<Collider> preyList = new List<Collider>();
    private List<Collider> predatorList = new List<Collider>();
    private List<Collider> resourceList = new List<Collider>();

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
                    }
                }
                else if (!isRoaming)
                {
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
                    Transform predator = predatorList[Random.Range(0, predatorList.Count)].transform;
                    Vector3 fleeDirection = transform.position - predator.position;
                    transform.LookAt(transform.position + fleeDirection);
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
                else if (targetResource != null && !tryMate && !isFull)
                {
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

        isRoaming = true;
    }

    private void MateWithAnotherPrey()
    {
        float transmissionChance = 0.5f;
        float inheritChance = 0.9f;
        // Find another prey in the scene that has also eaten enough resources to mate
        Creature[] allPreys = FindObjectsOfType<Creature>();
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

            // Calculate the mating position (midpoint between two mates)
            Vector3 matingPosition = (transform.position + mate.transform.position) / 2f;

            // Instantiate a new prey at the mating position
            GameObject newPrey = Instantiate(gameObject, matingPosition, Quaternion.identity);
            Creature newPreyScript = newPrey.GetComponent<Creature>();
            newPrey.name = newPrey.name.Replace("(Clone)", "");

            // Reset eatenResources counts for both mates and the newborn prey
            eatenResources = 0;
            isFull = false;
            mate.eatenResources = 0;
            if (Random.value < transmissionChance && isSick == true)
            {
                mate.GetComponent<Creature>().isSick = true;
            }
            if (Random.value < inheritChance)
            {
                newPreyScript.lifetime = lifetime;
                newPreyScript.speed = speed;
                newPreyScript.strength = strength;
                if (isSick)
                {
                    newPreyScript.isSick = true;
                }
            }
            newPreyScript.speed = MutateProperty(Mathf.RoundToInt(newPreyScript.speed), 0.1f, 2);
            newPreyScript.strength = MutateProperty(Mathf.RoundToInt(newPreyScript.strength), 0.1f, 2);
            newPreyScript.lifetime = MutateProperty(Mathf.RoundToInt(newPreyScript.lifetime), 0.1f, 2);
            newPreyScript.eatenResources = 0;

            // Set new random destinations for all three preys
            tryMate = false;
            SetNewRoamDestination();
            mate.SetNewRoamDestination();
            newPreyScript.SetNewRoamDestination();
        }
    }

    private void OnMouseDown() {
        infoValues = infoPanel.GetComponent<InfoPanel>();

        infoValues.selectedCreature = gameObject;
        infoValues.fpCam.target = gameObject;
        infoValues.creatureName.text = gameObject.name;
        infoValues.isSick.SetActive(isSick);
        infoValues.lifeTime.text = lifetime.ToString();
        infoValues.speed.text = speed.ToString();
        infoValues.strength.text = strength.ToString();

        infoPanel.SetActive(false);
        infoPanel.SetActive(true);
    }

    private float MutateProperty(int originalValue, float mutationChance, int maxMutationAmount)
    {
        if (Random.value < mutationChance)
        {
            Debug.Log("A mutation has occurred");
            return originalValue + Random.Range(-maxMutationAmount, maxMutationAmount);
        }
        return originalValue;
    }
}
