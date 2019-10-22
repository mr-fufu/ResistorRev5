using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnList : MonoBehaviour
{
    // (IMPORTANT!) This script is a fair bit more complicated than it looks. Essentially, the script
    // contains a number of lists that together provide all the required information on how 
    // the contained robot was built

    // The spawn list is populated as the bot is built in the workshop. Thus, the first element
    // in each array corresponds to the first part placed (always the leg) and the second to the second
    // part placed and so on. The name and part type lists are fairly self explanatory
    // but the child_count arrays and the parent_count list is a more complex.

    // Example Bot

    //                 [LEG PART (COLOSSUS)]
    //                           ||
    //                      {torso slot}
    //                           ||
    //                 [TORSO PART(LIGHT CORE)]
    //                 ||                    ||
    //             {arm slot}           {front slot}
    //                 ||                    ||
    //       [ARM PART(FLAMETHROWER)] [FRONT PART(ARMOR)]

    // Here, the diagram represents a bot built in the workshop scene heirarchy
    // the leg part is the root with the child slot as its child and the torso part a child of the slot
    // the arm slot and front slot here are both children of the torso but the arm slot is the 0th child
    // and the front slot the 1st child. The arm and front parts are then children of the respective slots

    // In this example, the 0th element of the name list is COLOSSUS, and the 0th element of the part type
    // list is LEG. The second element is thus LIGHT CORE and TORSO and so on. Child count lists are more
    // tricky. For say the arm part, the child 1 is 0, the child 2 is 0. For the armor, child 1 is 0
    // but child 2 is 1. To find the child count values, we look at the path from the root to the part
    // described by the index. From the root, child count 1 refers to which child the path continues on.
    // For child_count_2, the arm and front part diverge. From the torso, the arm slot is the 0th child of
    // the torso while the front slot is the 1st child.

    // Parent Count refers to how many elements separate the indexed part from the root Leg part. In this
    // case the arm has a parent_count of 2 since it is separated by the arm slot and the torso slot. The 
    // torso part is separated from the leg part by the torso slot alone and has a parent_count of 1. Finally,
    // for the leg part, the parent_count is 0.

    public string[] name_list;
    public string[] part_type_list;

    public int[] child_count_1_list;
    public int[] child_count_2_list;
    public int[] child_count_3_list;
    public int[] child_count_4_list;

    public int[] parent_count_list;

    public GameObject[] part_object_list;

    private string[] name_add;
    private string[] part_type_add;

    private int[] child_count_1_add;
    private int[] child_count_2_add;
    private int[] child_count_3_add;
    private int[] child_count_4_add;

    private int[] parent_count_add;

    private GameObject[] part_object_add;

    void Start()
    {
        // initialize arrays of size 0 (size will increase as parts are added)

        name_list = new string[0];
        part_type_list = new string[0];

        child_count_1_list =  new int[0];
        child_count_2_list = new int[0];
        child_count_3_list = new int[0];
        child_count_4_list = new int[0];

        parent_count_list = new int[0];
    }

    void Update()
    {
        
    }

    // Main function for adding an element to all arrays. Increase the size of all arrays by 1
    // then adds the name, part type, child counts and parent count to the appropriate arrays at the 
    // new index (representing the new part). 

    public void AddIndex (string part_name_to_add, string part_type_to_add, int child_count_1_to_add, int child_count_2_to_add, int child_count_3_to_add, int child_count_4_to_add, int parent_count_to_add, GameObject object_to_add)
    {
        // create a new array for each list (add list) that is 1 larger than the length
        // of the current lists
        name_add = new string[name_list.Length + 1];
        part_type_add = new string[name_list.Length + 1];
        child_count_1_add = new int[name_list.Length + 1];
        child_count_2_add = new int[name_list.Length + 1];
        child_count_3_add = new int[name_list.Length + 1];
        child_count_4_add = new int[name_list.Length + 1];
        parent_count_add = new int[name_list.Length + 1];
        part_object_add = new GameObject[name_list.Length + 1];

        // populate all elements of add arrays with current list elements (all elements
        // are then full except for the newly added array index
        for (int index = 0; index < (name_list.Length); index++)
        {
            name_add[index] = name_list[index];
            part_type_add[index] = part_type_list[index];
            child_count_1_add[index] = child_count_1_list[index];
            child_count_2_add[index] = child_count_2_list[index];
            child_count_3_add[index] = child_count_3_list[index];
            child_count_4_add[index] = child_count_4_list[index];
            parent_count_add[index] = parent_count_list[index];
            part_object_add[index] = part_object_list[index];
        }

        // add the newly added part name, part type, child count and parent count to the
        // newly created index
        name_add[name_list.Length] = part_name_to_add;
        part_type_add[name_list.Length] = part_type_to_add;
        child_count_1_add[name_list.Length] = child_count_1_to_add;
        child_count_2_add[name_list.Length] = child_count_2_to_add;
        child_count_3_add[name_list.Length] = child_count_3_to_add;
        child_count_4_add[name_list.Length] = child_count_4_to_add;
        parent_count_add[name_list.Length] = parent_count_to_add;
        part_object_add[name_list.Length] = object_to_add;

        // set the list arrays equal to the add arrays
        name_list = name_add;
        part_type_list = part_type_add;
        child_count_1_list = child_count_1_add;
        child_count_2_list = child_count_2_add;
        child_count_3_list = child_count_3_add;
        child_count_4_list = child_count_4_add;
        parent_count_list = parent_count_add;
        part_object_list = part_object_add;
    }

    // Remove index functions removes all elements of a specific index (such as the 3rd part or 4th part) and
    // move all indicies afterwards up by 1
    public void RemoveIndex(int remove_index)
    {
        // Create a new set of arrays referred to as "add arrays". Length of new arrays is equal
        // to the current number of elements minus one.
        name_add = new string[name_list.Length - 1];
        part_type_add = new string[name_list.Length - 1];
        child_count_1_add = new int[name_list.Length - 1];
        child_count_2_add = new int[name_list.Length - 1];
        child_count_3_add = new int[name_list.Length - 1];
        child_count_4_add = new int[name_list.Length - 1];
        parent_count_add = new int[name_list.Length - 1];
        part_object_add = new GameObject[name_list.Length - 1];

        // For all indicies before the index to be removed copy the current elements
        // into the add arrays
        for (int index = 0; index < remove_index; index++)
        {
            name_add[index] = name_list[index];
            part_type_add[index] = part_type_list[index];
            child_count_1_add[index] = child_count_1_list[index];
            child_count_2_add[index] = child_count_2_list[index];
            child_count_3_add[index] = child_count_3_list[index];
            child_count_4_add[index] = child_count_4_list[index];
            parent_count_add[index] = parent_count_list[index];
            part_object_add[index] = part_object_list[index];
        }

        // For all indicies after the index to be removed copy the current elements
        // into the add arrays but move them up by one index (i.e. copy the name[index2] into
        // name[index2 - 1]
        for (int index2 = (remove_index + 1); index2 < (name_list.Length); index2++)
        {
            name_add[index2 -1] = name_list[index2];
            part_type_add[index2 - 1] = part_type_list[index2];
            child_count_1_add[index2 - 1] = child_count_1_list[index2];
            child_count_2_add[index2 - 1] = child_count_2_list[index2];
            child_count_3_add[index2 - 1] = child_count_3_list[index2];
            child_count_4_add[index2 - 1] = child_count_4_list[index2];
            parent_count_add[index2 - 1] = parent_count_list[index2];
            part_object_add[index2 - 1] = part_object_list[index2];

            part_object_add[index2 - 1].GetComponent<PartStats>().list_index -= 1;
        }

        // set the current arrays to the add arrays
        name_list = name_add;
        part_type_list = part_type_add;
        child_count_1_list = child_count_1_add;
        child_count_2_list = child_count_2_add;
        child_count_3_list = child_count_3_add;
        child_count_4_list = child_count_4_add;
        parent_count_list = parent_count_add;
        part_object_list = part_object_add;
    }

    // clear all list arrays
    public void ClearIndex()
    {
        name_list = new string[0];
        part_type_list = new string[0];
        child_count_1_list = new int[0];
        child_count_2_list = new int[0];
        child_count_3_list = new int[0];
        child_count_4_list = new int[0];
        parent_count_list = new int[0];
        part_object_list = new GameObject[0];
    }
}
