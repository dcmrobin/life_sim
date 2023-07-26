using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool paused;
    public int maxSicknessID = 9;
    public List<Sickness> sicknesses;
    SicknessNameGenerator sicknessNameGenerator = new SicknessNameGenerator();

    private void Start() {
        sicknesses = new List<Sickness>();
        int a = 0;
        while (a < maxSicknessID + 1)
        {
            sicknesses.Add(new Sickness(0, "", 0, 0));
            a++;
        }
        for (int i = 0; i < sicknesses.Count; i++)
        {
            sicknesses[i] = new Sickness(i, sicknessNameGenerator.GenerateRandomSicknessName(), Random.Range(10f, 50f), Random.Range(0.1f, 0.9f));
        }
    }

    // Update is called once per frame
    void Update()
    {
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
