using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool paused;
    public int maxSicknessID = 9;
    public List<Sickness> sicknesses;
    //public List<Sickness> origSicknesses;
    SicknessNameGenerator sicknessNameGenerator = new SicknessNameGenerator();

    private void Start() {
        sicknesses = new List<Sickness>();
        //origSicknesses = new List<Sickness>();
        int a = 0;
        while (a < maxSicknessID + 1)
        {
            sicknesses.Add(new Sickness(0, "", 0, 0));
            a++;
        }
        for (int i = 0; i < sicknesses.Count; i++)
        {
            sicknesses[i] = new Sickness(i, sicknessNameGenerator.GenerateRandomSicknessName(), Random.Range(30f, 100f), Random.Range(0.1f, 0.9f));
        }
        //origSicknesses = sicknesses;
    }

    // Update is called once per frame
    void Update()
    {
        //sicknesses = origSicknesses;
        if (Input.GetKeyDown(KeyCode.P) && paused == false)
        {
            paused = true;
        }
        else if (Input.GetKeyDown(KeyCode.P) && paused == true)
        {
            paused = false;
        }
    }
}
