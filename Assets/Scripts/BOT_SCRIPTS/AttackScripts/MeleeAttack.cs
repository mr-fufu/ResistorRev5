using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeleeAttack : MonoBehaviourPunCallbacks
{
    public bool spawned;
    public int damage_val;
    public float attack_speed;
    public bool engagedCheck;
    public bool attached;

    private Animator anim;

    [PunRPC]
    public void SyncEngagement(bool engaged)
    {
        engagedCheck = engaged;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.speed = attack_speed;

        if (photonView.IsMine)
        {
            if (GetComponent<PartStats>().partType != "LEG")
            {
                engagedCheck = transform.parent.parent.GetComponent<StandardStatBlock>().engaged_check;
            }
            else
            {
                engagedCheck = GetComponent<StandardStatBlock>().engaged_check;
            }

            photonView.RPC("SyncEngagement", RpcTarget.Others, engagedCheck);
        }

        if (engagedCheck)
        {
            anim.SetBool("Engagement", true);
        }
        else
        {
            anim.SetBool("Engagement", false);
        }
    }
}
