using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    public GameObject selectedCreature;
    public TMP_Text creatureName;
    public GameObject isSick;
    public GameObject isFull;
    public TMP_Text lifeTime;
    public TMP_Text curlifeTime;
    public TMP_Text speed;
    public TMP_Text strength;
    public TMP_Text status;
    public FPperspective fpCam;
    public GameObject birdseyeCam;
    Creature selectedCreatureScript;

    private void Update() {
        lifeTime.text = Mathf.Round(float.Parse(lifeTime.text)).ToString();
        curlifeTime.text = Mathf.Round(float.Parse(curlifeTime.text)).ToString();
        speed.text = Mathf.Round(float.Parse(speed.text)).ToString();
        strength.text = Mathf.Round(float.Parse(strength.text)).ToString();

        if (selectedCreature == null)
        {
            fpCam.gameObject.SetActive(false);
            birdseyeCam.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            selectedCreatureScript = selectedCreature.GetComponent<Creature>();
        }

        if (selectedCreatureScript.isRoaming)
        {
            status.text = "Roaming";
        }
        else if (selectedCreatureScript.isFleeing)
        {
            status.text = "Fleeing";
        }
        else if (selectedCreatureScript.isMating)
        {
            status.text = "Mating";
        }
        else if (selectedCreatureScript.isChasing)
        {
            status.text = "Chasing";
        }
        else if (selectedCreatureScript.isEating)
        {
            status.text = "Eating";
        }
        else
        {
            status.text = "Unknown";
        }
    }
}
