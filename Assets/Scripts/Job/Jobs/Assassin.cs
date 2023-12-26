using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Assassin : Job
{
    public override void Initialize()
    {
        base.hp = 40;
        base.attack = 5;
    }

    private GameObject assassinEffect;
    public bool isPlayerVisible = true;
    private bool isEffectVisible = false; // 처음에는 이펙트 비활성화
    private PhotonView pv;

    void Start()
    {
        pv = GetComponentInParent<PhotonView>();
        assassinEffect = InstantiateAssassinEffect();
    }

    void Update()
    {
        if (pv != null && !pv.IsMine)
            return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            pv.RPC("UseAssassinSkill", RpcTarget.All);
        }

        // // 'Q' 키가 눌렸을 때만 이펙트가 플레이어를 따라다니도록
        // if (isEffectVisible && assassinEffect != null)
        // {
        //     assassinEffect.transform.position = transform.position;
        // }
    }

    public void TogglePlayerVisibility()
    {
        isPlayerVisible = !isPlayerVisible;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isPlayerVisible;
        }
    }

    public void ToggleEffectVisibility()
    {
        if (assassinEffect != null)
        {
            isEffectVisible = !isEffectVisible;
            assassinEffect.SetActive(isEffectVisible);
        }
    }

    GameObject InstantiateAssassinEffect()
    {
        GameObject effectPrefab = Resources.Load<GameObject>("smoke_thick") as GameObject;
        if (effectPrefab == null)
        {
            Debug.LogError("smoke_thick 프리팹을 로드할 수 없습니다. 프리팹이 Resources 폴더에 위치해 있는지 확인하세요.");
            return null;
        }

        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        effect.SetActive(isEffectVisible);
        return effect;
    }
}
