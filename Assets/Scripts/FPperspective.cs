using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPperspective : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
        }
    }
}
