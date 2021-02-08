using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SceneManagementScript : MonoBehaviour
{
    public GameObject camera_object;

    public GameObject[] stage_group;

    public GameObject start_nixie;

    public GameObject Stage1;
    public GameObject Stage2;
    public GameObject Stage3;
    public GameObject Stage4;

    public GameObject flash1;
    public GameObject flash2;
    public GameObject flash3;
    public GameObject flash4;

    public GameObject[] flash_group;

    public GameObject CarryOver;

    public Transform hidden_stage_1;
    public Transform hidden_stage_2;
    public Transform hidden_stage_3;
    public Transform hidden_stage_4;

    public GameObject stage_changer;

    public bool start_true;

    //Only for Networked Loading
    public GameObject loading_screen;
    public LoadingFade loading;

    private bool bot_adjust;

    public GameObject EndCard;

    private void Start()
    {
        if (start_nixie != null)
        {
            stage_group = new GameObject[4];

            stage_group[0] = Stage1.transform.GetChild(0).gameObject;
            stage_group[1] = Stage2.transform.GetChild(0).gameObject;
            stage_group[2] = Stage3.transform.GetChild(0).gameObject;
            stage_group[3] = Stage4.transform.GetChild(0).gameObject;

            flash_group = new GameObject[4];

            flash_group[0] = flash1;
            flash_group[1] = flash2;
            flash_group[2] = flash3;
            flash_group[3] = flash4;
        }
    }

    void Update()
    {
        if (start_nixie != null)
        {
            if (camera_object.GetComponent<WorkshopDrag>().tutorial_complete)
            {
                if (Stage1.transform.GetChild(0).GetComponent<CheckCompletion>().complete_check &&
                    Stage2.transform.GetChild(0).GetComponent<CheckCompletion>().complete_check &&
                    Stage3.transform.GetChild(0).GetComponent<CheckCompletion>().complete_check &&
                    Stage4.transform.GetChild(0).GetComponent<CheckCompletion>().complete_check)
                {
                    start_true = true;
                }
                else
                {
                    start_true = false;
                }
            }
            else
            {
                start_nixie.SetActive(true);
            }
        }

        if (bot_adjust)
        {
            stage_changer.GetComponent<StageSelector>().change_scene = true;

            Stage1.SetActive(true);
            Stage1.transform.GetChild(0).GetComponent<CheckCompletion>().workshop = false;
            Stage1.transform.localScale = new Vector3(1, 1, 1);
            Stage1.transform.position = hidden_stage_1.position;
            Stage1.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

            if (Stage1.transform.GetChild(0).transform.childCount != 0)
            {
                Stage1.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                Stage1.transform.localScale = new Vector3(1, 1, 1);
            }

            Stage2.SetActive(true);
            Stage2.transform.GetChild(0).GetComponent<CheckCompletion>().workshop = false;
            Stage1.transform.localScale = new Vector3(1, 1, 1);
            Stage2.transform.position = hidden_stage_2.position;
            Stage2.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

            if (Stage2.transform.GetChild(0).transform.childCount != 0)
            {
                Stage2.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                Stage2.transform.localScale = new Vector3(1, 1, 1);
            }

            Stage3.SetActive(true);
            Stage3.transform.GetChild(0).GetComponent<CheckCompletion>().workshop = false;
            Stage1.transform.localScale = new Vector3(1, 1, 1);
            Stage3.transform.position = hidden_stage_3.position;
            Stage3.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

            if (Stage3.transform.GetChild(0).transform.childCount != 0)
            {
                Stage3.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                Stage3.transform.localScale = new Vector3(1, 1, 1);
            }

            Stage4.SetActive(true);
            Stage4.transform.GetChild(0).GetComponent<CheckCompletion>().workshop = false;
            Stage1.transform.localScale = new Vector3(1, 1, 1);
            Stage4.transform.position = hidden_stage_4.position;
            Stage4.transform.GetChild(0).gameObject.GetComponent<Collider2D>().enabled = false;

            if (Stage4.transform.GetChild(0).transform.childCount != 0)
            {
                Stage4.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                Stage4.transform.localScale = new Vector3(1, 1, 1);
            }

            stage_changer.GetComponent<StageSelector>().set_all_active();

            DontDestroyOnLoad(Stage1.gameObject);
            DontDestroyOnLoad(Stage2.gameObject);
            DontDestroyOnLoad(Stage3.gameObject);
            DontDestroyOnLoad(Stage4.gameObject);
        }
    }

    public void GoToBattleScene()
    {
        for (int nixie_count = 0; nixie_count < 4; nixie_count++)
        {
            if (stage_group[nixie_count].GetComponent<CheckCompletion>().complete_check != true)
            {
                flash_group[nixie_count].GetComponent<FlashScript>().flash = true;
            }
        }

        if (start_true)
        {
            bot_adjust = true;
            LoadWithScreen("LobbyScene");
        }
    }

    public void SkipToBattle()
    {
        bot_adjust = true;

        //LoadWithScreen("LobbyScene");
    }

    public void GoToWorkshop()
    {
        LoadWithScreen("Workshop");
    }

    public void GoToIntro()
    {
        StartCoroutine(LoadWithoutScreen("IntroScene"));
    }

    public void ShowEndScreen(bool win_condition)
    {
        Destroy(GameObject.Find("StageArea1"));
        Destroy(GameObject.Find("StageArea2"));
        Destroy(GameObject.Find("StageArea3"));
        Destroy(GameObject.Find("StageArea4"));

        if (win_condition)
        {
            CarryOver.GetComponent<StringCarryover>().carry_over_text = "YOU EMERGE VICTORIOUS!";
        }
        else
        {
            CarryOver.GetComponent<StringCarryover>().carry_over_text = "YOU HAVE BEEN DEFEATED!";
        }

        EndCard.SetActive(true);

        //DontDestroyOnLoad(CarryOver);
        //LoadWithScreen("EndScreen");
    }

    public void ReturnToWorkshop()
    {
        Destroy(GameObject.Find("StageArea1"));
        Destroy(GameObject.Find("StageArea2"));
        Destroy(GameObject.Find("StageArea3"));
        Destroy(GameObject.Find("StageArea4"));

        LoadWithScreen("Workshop");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadWithoutScreen(string load_destination)
    {
        loading_screen.SetActive(true);
        loading = loading_screen.GetComponent<LoadingFade>();
        loading.FadeInLoadScreen();

        yield return new WaitUntil(() => loading.fadeInComplete == true);
        SceneManager.LoadScene(load_destination);
    }

    private IEnumerator LoadWithScreen(string load_destination)
    {
        loading_screen.SetActive(true);
        loading.FadeInLoadScreen();

        yield return new WaitUntil(() => loading.fadeInComplete == true);
        PhotonNetwork.LoadLevel(load_destination);
    }
}