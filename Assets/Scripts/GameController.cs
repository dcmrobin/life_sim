using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool paused;

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
