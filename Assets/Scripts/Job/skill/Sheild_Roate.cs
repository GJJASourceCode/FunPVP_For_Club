using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheild_Roate : MonoBehaviour
{
  public float rotationSpeed = 30f; // 회전 속도 (원하는 값으로 조절)

    void Update()
    {
        // 오브젝트를 항상 회전시키기
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
