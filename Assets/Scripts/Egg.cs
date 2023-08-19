using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [Header("Attributes for newborn to inherit")]
    public List<Sickness> sicknesses = new List<Sickness>();
    public bool tryMate;
    public bool isFull;
    public bool isSick;
    public float lifeTime;
    public float speed;
    public float strength;
    public float detectionRadius;
    public int eatenResources;
    public int generation;

    [Header("Egg attributes")]
    public float timeUntilHatch = 10;
}
