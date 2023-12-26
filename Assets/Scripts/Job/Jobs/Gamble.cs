using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamble : Job
{
    public override void Initialize()
    {
        base.hp = Random.Range(20, 50);
        base.attack = Random.Range(0, 6);
    }
}
