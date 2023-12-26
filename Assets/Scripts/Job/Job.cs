using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Job : MonoBehaviour
{
    public int hp;
    public int attack;

    public abstract void Initialize();

    public void Awake()
    {
        Initialize();
    }
}
