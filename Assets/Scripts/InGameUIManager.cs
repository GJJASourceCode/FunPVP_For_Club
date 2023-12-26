using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private Job job;
    public Text hpText;

    // Start is called before the first frame update
    void Start()
    {
        job = GetComponentInParent<Job>();
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "HP : " + job.hp;
    }
}
