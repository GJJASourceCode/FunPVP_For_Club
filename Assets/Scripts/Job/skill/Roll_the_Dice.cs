using Photon.Pun;
using UnityEngine;

public class Roll_the_Dice : MonoBehaviour
{
    public int damage;
    public PhotonView ownerPV;

    void Start()
    {
        damage = Random.Range(-31, 31);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject, 3);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            if (ownerPV == null || ownerPV.Owner != PhotonNetwork.LocalPlayer)
                return;

            if (collision.gameObject.TryGetComponent(out Shield s) && s.isSkillActive)
            {
                return;
            }
            var player = collision.gameObject.GetComponent<Player>();
            if (!player.pv.IsMine)
            {
                player.pv.RPC("TakeDamage", RpcTarget.All, damage);
                return;
            }
        }
    }
}
