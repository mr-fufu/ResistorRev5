using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCompletion : MonoBehaviour
{
    public bool workshop;
    public int incomplete_count;
    public bool complete_check;

    public GameObject off_indicator;
    public GameObject on_indicator;

    public float flash_counter;

    // Start is called before the first frame update
    void Start()
    {
        complete_check = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (workshop)
        {
            check_complete(gameObject);

            if (complete_check)
            {
                off_indicator.SetActive(false);
                on_indicator.SetActive(true);
            }
            else
            {
                off_indicator.SetActive(true);
                on_indicator.SetActive(false);
            }
        }
    }

    void check_complete(GameObject check_object)
    {
        if (check_object.GetComponent<CheckCompletion>() != null)
        {
            incomplete_count = 0;
        }

        if (check_object.GetComponent<PartStats>() != null)
        {
            if (check_object.GetComponent<PartStats>().slotComponent)
            {
                int child_count = check_object.transform.childCount;
                int child_incomplete = 0;

                for (int check_var = 0; check_var < child_count; check_var++)
                {
                    if (check_object.transform.GetChild(check_var).GetComponent<PartStats>() != null)
                    {
                        int part_count = check_object.transform.GetChild(check_var).childCount;

                        for (int part_var = 0; part_var < part_count; part_var++)
                        {
                            if (check_object.transform.GetChild(check_var).GetChild(part_var).GetComponent<PartStats>() != null)
                            {
                                if (check_object.transform.GetChild(check_var).GetChild(part_var).GetComponent<PartStats>().slotComponent)
                                {
                                    check_complete(check_object.transform.GetChild(check_var).GetChild(part_var).gameObject);
                                }
                            }
                        }
                    }
                    else
                    {
                        child_incomplete++;
                    }
                }

                if (child_incomplete == child_count)
                {
                    incomplete_count++;
                }
            }
        }
    

    //void check_complete(GameObject check_object, int check_count)
    //{
    //    if (check_object.GetComponent<CheckCompletion>() != null)
    //    {
    //        incomplete_count = 0;
    //    }

    //    if (check_count != 0)
    //    {
    //        for (int check_var = 0; check_var < check_count; check_var++)
    //        {
    //            if (check_object.transform.GetChild(0).GetChild(check_var).GetComponent<PartStats>() != null)
    //            {
    //                if (check_object.transform.GetChild(0).GetChild(check_var).GetComponent<PartStats>().slot_component)
    //                {
    //                    if (check_object.transform.GetChild(0).GetChild(check_var).transform.childCount == 0)
    //                    {
    //                        incomplete_count++;
    //                    }
    //                    else if (check_object.transform.GetChild(0).GetChild(check_var).childCount > 0)
    //                    {
    //                        int child_obj_count = check_object.transform.GetChild(0).GetChild(check_var).childCount;
    //                        int child_incomplete = 0;

    //                        for (int child_check_var = 0; child_check_var < child_obj_count; child_check_var++)
    //                        {
    //                            if (check_object.transform.GetChild(0).GetChild(check_var).GetChild(child_check_var).gameObject.GetComponent<PartStats>() != null)
    //                            {
    //                                if (check_object.transform.GetChild(0).GetChild(check_var).GetChild(child_check_var).childCount != 0)
    //                                {
    //                                    check_complete(check_object.transform.GetChild(0).GetChild(check_var).gameObject, check_object.transform.GetChild(0).GetChild(check_var).GetChild(0).childCount);
    //                                    check_count = check_object.transform.GetChild(0).childCount;
    //                                }
    //                            }
    //                            else
    //                            {
    //                                child_incomplete++;
    //                            }
    //                        }
    //                        if(child_incomplete == child_obj_count)
    //                        {
    //                            incomplete_count++;
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                //incomplete_count++;
    //            }
    //        }
    //    }

        if (check_object.GetComponent<CheckCompletion>() != null)
        {
            if (incomplete_count == 0)
            {
                check_object.GetComponent<CheckCompletion>().complete_check = true;
            }
            else
            {
                check_object.GetComponent<CheckCompletion>().complete_check = false;
            }
        }
    }
}
