using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class Gamble : Job
{
      public override void Initialize()
    {
        base.hp = Random.Range (0, 31) ; 
        base.attack = Random.Range (0, 6);
    }
}
