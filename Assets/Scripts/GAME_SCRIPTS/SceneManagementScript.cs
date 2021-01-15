using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public GameObject loading_screen;
    public LoadingFade loading;

    private bool set_load;
    private string load_location;
    private bool bot_adjust;
    private bool currently_loading;

    public GameObject EndCard;

    //private AsyncOperation sceneAsync;

    private void Start()
    {
        //var load_screen = (GameObject)Instantiate(loading_screen, new Vector3(0, 0, 0), Quaternion.identity);
        loading = loading_screen.GetComponent<LoadingFade>();
        loading_screen.SetActive(false);

        //valueTxt.text = PersistentManager.single_instance.Value.ToString();
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

        if (set_load)
        {
            //if (loading.loaded_in)
            //{
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

                if (!currently_loading)
                {
                    StartCoroutine(LoadAsync(load_location));

                    currently_loading = true;
                }
           // }
        }
    }

    public void GoToBattleScene()
    {
        /*
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainBattleScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;
        sceneAsync = asyncLoad;

        //SceneManager.LoadScene("MainBattleScene");

        SceneManager.MoveGameObjectToScene(Stage1, SceneManager.GetSceneByName("MainBattleScene"));

        SceneManager.UnloadSceneAsync("Workshop");
        */

        for (int nixie_count = 0; nixie_count < 4; nixie_count++)
        {
            //Debug.Log(nixie_count);
            //Debug.Log(stage_group[nixie_count].GetComponent<CheckCompletion>().complete_check);
            //Debug.Log(flash_group[nixie_count]);

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
        /*
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainBattleScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;
        sceneAsync = asyncLoad;

        //SceneManager.LoadScene("MainBattleScene");

        SceneManager.MoveGameObjectToScene(Stage1, SceneManager.GetSceneByName("MainBattleScene"));

        SceneManager.UnloadSceneAsync("Workshop");
        */

        bot_adjust = true;

        LoadWithScreen("LobbyScene");
    }

    public void GoToWorkshop()
    {
        LoadWithScreen("Workshop");
    }

    public void GoToIntro()
    {
        LoadWithScreen("IntroScene");
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

    public void LoadWithScreen(string load_destination)
    {
        loading_screen.SetActive(true);
        loading.fade_in = true;

        set_load = true;
        load_location = load_destination;
    }

    IEnumerator LoadAsync(string load_destination)
    {
        bool load_completion = false;

        //if (loading.loaded_in)

        AsyncOperation load_op = SceneManager.LoadSceneAsync(load_destination);

        while (!load_op.isDone)
        {
            if (load_op.progress >= 0.89f)
            {
                if (!load_completion)
                {
                    Debug.Log("load Completed");

                    loading.fade_in = false;
                    loading.load_complete = true;

                    DontDestroyOnLoad(loading_screen);

                    load_completion = true;
                }
            }

            yield return null;
        }
    }
}
