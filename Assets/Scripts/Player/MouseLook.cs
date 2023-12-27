using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private PhotonView pv;
    public float sensitivityX = 0.5f;
    public float sensitivityY = 0.5f;
    public float maxYAngle = 70.0f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pv = GetComponentInParent<PhotonView>();
    }

    void Update()
    {
        if (!pv.IsMine)
            return;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationY += mouseY * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, -maxYAngle, maxYAngle);

        rotationX += mouseX * sensitivityX;
        // 카메라와 캐릭터의 회전
        transform.parent.Find("Mesh Object/Bone_Body/Bone_Neck").localRotation = Quaternion.Euler(
            -rotationY,
            0,
            0
        );

        transform.localRotation = Quaternion.Euler(-rotationY, 0, 0);
        transform.parent.rotation = Quaternion.Euler(0, rotationX, 0);
    }
}
