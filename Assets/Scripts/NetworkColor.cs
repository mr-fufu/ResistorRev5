using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkColor : MonoBehaviourPunCallbacks
{
    public bool enemy;

    private string searchText;
    [SerializeField]
    private GameObject[] colorObjects;
    private BotColorObject colorSource;

    public Color color;
    public bool reColor;
    public bool colored;

    private Color hold_color;

    [PunRPC]
    public void SyncColor(float r, float g, float b)
    {
        color = new Color (r, g, b, 1);
        reColor = true;
    }

    void Start()
    {
        colored = false;
        reColor = false;
        searchText = "ColorObject";

        if (PhotonNetwork.IsMasterClient != enemy)
        {
            colorSource = GameObject.Find(searchText).GetComponent<BotColorObject>();
            color = colorSource.GetColors();

            if (colorSource != null)
            {
                photonView.RPC("SyncColor", RpcTarget.All, color.r, color.g, color.b);
            }
        }
    }

    void Update()
    {
        if (!colored)
        {
            if (reColor)
            {
                for (int i = 0; i < colorObjects.Length; i++)
                {
                    ReColorObject(colorObjects[i]);
                    for (int j = 0; j < colorObjects[i].transform.childCount; j++)
                    {
                        ReColorObject(colorObjects[i].transform.GetChild(j).gameObject);
                    }
                }
                colored = true;
            }
        }
    }

    void ReColorObject(GameObject objectToColor)
    {
        var light = objectToColor.GetComponent<Light>();
        var sprite = objectToColor.GetComponent<SpriteRenderer>();
        var text = objectToColor.GetComponent<Text>();

        if (light)
        {
            light.color = color;
        }

        if (sprite)
        {
            hold_color = color;
            hold_color.a = sprite.color.a;
            sprite.color = hold_color;
        }

        if (text)
        {
            text.color = color;
        }
    }
}


