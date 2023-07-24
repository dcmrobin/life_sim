using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    public GameObject selectedCreature;
    public TMP_Text creatureName;
    public GameObject isSick;
    public TMP_Text lifeTime;
    public TMP_Text curlifeTime;
    public TMP_Text speed;
    public TMP_Text strength;

    private void Update() {
        lifeTime.text = Mathf.Round(float.Parse(lifeTime.text)).ToString();
        curlifeTime.text = Mathf.Round(float.Parse(curlifeTime.text)).ToString();
        speed.text = Mathf.Round(float.Parse(speed.text)).ToString();
        strength.text = Mathf.Round(float.Parse(strength.text)).ToString();

        if (selectedCreature == null)
        {
            gameObject.SetActive(false);
        }
    }
}
