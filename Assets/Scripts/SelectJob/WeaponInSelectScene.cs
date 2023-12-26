using System.Collections;
using UnityEngine;

public class WeaponInSelectScene : MonoBehaviour
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

    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Debug.Log("Use");
        //     Use();
        // }
    }

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
        else if (type == Type.Wand)
        {
            Debug.Log("CastSpell");
            StopCoroutine("CastSpell");
            StartCoroutine("CastSpell");
        }
        else if (type == Type.Dice)
        {
            Debug.Log("Roll_dice");
            StopCoroutine("Roll_dice");
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
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity); // LookAt removed
        instantBullet.transform.LookAt(bulletPos.position + bulletPos.forward); // LookAt corrected
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = instantBullet.transform.forward * 50; // Velocity corrected
        yield return null;
    }

    IEnumerator CastSpell()
    {
        var playerS = GetComponentInParent<PlayerInSelectScene>();
        //Debug.Log("CastSpell");
        GameObject instantMagic_spell = Instantiate(
            Magic_spell,
            playerS.transform.position + playerS.transform.forward * 4f + Vector3.up * 3f,
            transform.rotation
        );
        Rigidbody magicSpellRigid = instantMagic_spell.GetComponent<Rigidbody>();
        magicSpellRigid.velocity =
            playerS.transform.Find("Mesh Object/Bone_Body/Bone_Neck/Bone_Head").forward * 51;
        yield return null;
    }

    IEnumerator Roll_dice()
    {
        canRollDice = false;

        var playerS = GetComponentInParent<PlayerInSelectScene>();
        //Debug.Log("RollDice");
        GameObject instantRoll_dice = Instantiate(
            Dice,
            transform.position + playerS.transform.right * 1f + playerS.transform.forward * 5f,
            transform.rotation
        );
        Rigidbody Roll_diceRigid = instantRoll_dice.GetComponent<Rigidbody>();
        Roll_diceRigid.velocity =
            playerS.transform.Find("Mesh Object/Bone_Body/Bone_Neck/Bone_Head").forward * 50;

        yield return new WaitForSeconds(0.5f);

        yield return null;
    }
}
