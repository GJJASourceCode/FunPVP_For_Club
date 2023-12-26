using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : Job
{
    public override void Initialize()
    {
        base.hp = 20;
        base.attack = 5;
    }
}
