using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Magic_Spell : MonoBehaviour
{
    public int damage;
    public PhotonView ownerPV;
    public Vector3 direction;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            if (ownerPV == null || ownerPV.Owner != PhotonNetwork.LocalPlayer)
                return;
            var player = collision.gameObject.GetComponent<Player>();
            if (!player.pv.IsMine)
            {
                player.pv.RPC("Knockback", RpcTarget.All, direction.normalized * 50f);
                player.pv.RPC("TakeDamage", RpcTarget.All, damage);
                return;
            }
        }
    }
}
