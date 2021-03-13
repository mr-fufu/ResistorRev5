using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarInspector : MonoBehaviour
{
    public GameObject spawnController;
    public GameObject inspector;
    public GameObject displayPoint;

    public GameObject inspectArrow;

    public List<SpriteRenderer> statSymbols;
    public List<Text> statValue;
    public List<float> maxStats;

    public Gradient valueColor;

    public GameObject bar_box_1;
    public GameObject bar_box_2;
    public GameObject bar_box_3;
    public GameObject bar_box_4;

    private GameObject[] botLoadout;
    private GameObject[] barBox;

    private List<int> botStats;

    void Start()
    {
        botLoadout = new GameObject[4];
        barBox = new GameObject[4];

        barBox[0] = bar_box_1;
        barBox[1] = bar_box_2;
        barBox[2] = bar_box_3;
        barBox[3] = bar_box_4;
    }

    void Update()
    {
        if (Input.GetButtonDown("1"))
        {
            bar_box_1.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_1.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("2"))
        {
            bar_box_2.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_2.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("3"))
        {
            bar_box_3.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_3.GetComponent<BarSelect>().box_no;
        }
        else if (Input.GetButtonDown("4"))
        {
            bar_box_4.transform.parent.GetComponent<BarSelectParentLink>().selected_box = bar_box_4.GetComponent<BarSelect>().box_no;
        }
            
        // update bot_loadouts
        for (int i = 1; i < 5; i++)
        {
            var curSpawnBot = spawnController.GetComponent<SpawnController>().spawnBots[i-1];
            if (curSpawnBot != null)
            {
                if (curSpawnBot.transform.GetChild(0).childCount != 0)
                {
                    botLoadout[i - 1] = curSpawnBot.transform.GetChild(0).transform.GetChild(0).gameObject;
                }
                else
                {
                    botLoadout[i - 1] = null;
                }
            }
        }

        var mouse_position = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D ray_hit = (Physics2D.Raycast(mouse_position, Vector2.zero, Mathf.Infinity));
        if (ray_hit.collider != null)
        {
            for (int check_no = 0; check_no < 4; check_no++)
            {
                if (ray_hit.collider.gameObject == barBox[check_no])
                {
                    barBox[check_no].GetComponent<BarSelect>().moused_over = true;

                    if (botLoadout[check_no] != null)
                    {
                        inspector.SetActive(true);
                        inspectArrow.transform.position = new Vector2(barBox[check_no].transform.position.x, inspectArrow.transform.position.y);

                        botLoadout[check_no].transform.position = displayPoint.transform.position;
                        botLoadout[check_no].transform.localScale = displayPoint.transform.localScale;

                        botStats = botLoadout[check_no].GetComponent<StandardStatBlock>().GetBotStats();

                        for (int i = 0; i< 8; i++)
                        {
                            statValue[i].text = ("" + botStats[i]);

                            statValue[i].color = valueColor.Evaluate(botStats[i] / maxStats[i]);
                            statSymbols[i].color = valueColor.Evaluate(botStats[i] / maxStats[i]);
                        }
                    }
                }
                else
                {
                    barBox[check_no].GetComponent<BarSelect>().moused_over = false;

                    if (botLoadout[check_no] != null)
                    {
                        botLoadout[check_no].transform.localPosition = new Vector2(0, 0);
                    }
                }

                if (ray_hit.collider.gameObject != barBox[0] && ray_hit.collider.gameObject != barBox[1] && ray_hit.collider.gameObject != barBox[2] && ray_hit.collider.gameObject != barBox[3])
                {

                }
            }
        }
        else
        {
            inspector.SetActive(false);
            for (int box_no = 0; box_no < 4; box_no++)
            {
                barBox[box_no].GetComponent<BarSelect>().moused_over = false;

                if (botLoadout[box_no] != null)
                {
                    botLoadout[box_no].transform.localPosition = new Vector2(0, 0);
                }
            }
        }
    }
}
