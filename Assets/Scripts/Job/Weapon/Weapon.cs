using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,
        Range,
        Wand,
        Dice
    };

    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;
    public BoxCollider meleeArea;

    public Transform bulletPos;
    public GameObject bullet;
    public GameObject Magic_spell;
    public GameObject Dice;
    private bool canRollDice = true;
    private bool canSpell = true;
    private PhotonView pv;
    private Job job;

    private void Start()
    {
        pv = GetComponentInParent<PhotonView>();
        job = GetComponentInParent<Job>();
    }

    void Update()
    {
        if (pv != null && !pv.IsMine)
            return;

        if (Input.GetMouseButtonDown(0) && canRollDice && canSpell)
        {
            //Debug.Log("Use");

            //Use();
            pv.RPC("Use", RpcTarget.All);
        }
    }

    public void Use()
    {
        if (type == Type.Melee)
        {
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
        else if (type == Type.Wand && canSpell)
        {
            StartCoroutine("CastSpell");
        }
        else if (type == Type.Dice && canRollDice)
        {
            StartCoroutine("Roll_dice");
        }
    }

    IEnumerator Swing()
    {
        if (meleeArea == null)
        {
            meleeArea = gameObject.AddComponent<BoxCollider>();
            meleeArea.isTrigger = true;
        }
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator Shot()
    {
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);
        instantBullet.transform.LookAt(bulletPos.position + bulletPos.forward);
        instantBullet.GetComponent<Bullet>().ownerPV = pv;
        instantBullet.GetComponent<Bullet>().damage = job.attack;
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = instantBullet.transform.forward * 50;
        yield return null;
    }

    IEnumerator CastSpell()
    {
        canSpell = false;

        var player = GetComponentInParent<Player>();
        //Debug.Log("CastSpell");
        GameObject instantMagic_spell = Instantiate(
            Magic_spell,
            player.transform.position + player.transform.forward * 5 + Vector3.up * 4.0f,
            transform.rotation
        );
        instantMagic_spell.GetComponent<Magic_Spell>().ownerPV = pv;
        instantMagic_spell.GetComponent<Magic_Spell>().damage = job.attack;
        Rigidbody magicSpellRigid = instantMagic_spell.GetComponent<Rigidbody>();
        magicSpellRigid.velocity =
            player.transform.Find("Mesh Object/Bone_Body/Bone_Neck/Bone_Head").forward * 51;

        yield return new WaitForSeconds(0.5f);

        canSpell = true;
    }

    IEnumerator Roll_dice()
    {
        canRollDice = false;

        var player = GetComponentInParent<Player>();
        //Debug.Log("RollDice");
        GameObject instantRoll_dice = Instantiate(
            Dice,
            transform.position + player.transform.right * 1f + player.transform.forward * 5f,
            transform.rotation
        );
        instantRoll_dice.GetComponent<Roll_the_Dice>().ownerPV = pv;
        instantRoll_dice.GetComponent<Roll_the_Dice>().damage = Random.Range(-10, 31);
        Rigidbody Roll_diceRigid = instantRoll_dice.GetComponent<Rigidbody>();
        Roll_diceRigid.velocity =
            player.transform.Find("Mesh Object/Bone_Body/Bone_Neck/Bone_Head").forward * 50;

        yield return new WaitForSeconds(0.5f);

        canRollDice = true;
    }
}
