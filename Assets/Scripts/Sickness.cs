using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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