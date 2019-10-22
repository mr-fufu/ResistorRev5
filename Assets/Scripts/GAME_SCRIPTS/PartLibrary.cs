using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartLibrary : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;
    public GameObject object5;
    public GameObject object6;
    public GameObject object7;
    public GameObject object8;
    public GameObject object9;
    public GameObject object10;
    public GameObject object11;
    public GameObject object12;
    public GameObject object13;
    public GameObject object14;
    public GameObject object15;
    public GameObject object16;
    public GameObject object17;
    public GameObject object18;
    public GameObject object19;
    public GameObject object20;
    public GameObject object21;
    public GameObject object22;
    public GameObject object23;
    public GameObject object24;
    public GameObject object25;
    public GameObject object26;
    public GameObject object27;
    public GameObject object28;
    public GameObject object29;
    public GameObject object30;

    public GameObject[] part_library;

    private GameObject[] part_container;

    private int new_index;

    // Start is called before the first frame update
    void Start()
    {
        part_library = new GameObject[30];

        part_library[0] = object1;
        part_library[1] = object2;
        part_library[2] = object3;
        part_library[3] = object4;
        part_library[4] = object5;
        part_library[5] = object6;
        part_library[6] = object7;
        part_library[7] = object8;
        part_library[8] = object9;
        part_library[9] = object10;
        part_library[10] = object11;
        part_library[11] = object12;
        part_library[12] = object13;
        part_library[13] = object14;
        part_library[14] = object15;
        part_library[15] = object16;
        part_library[16] = object17;
        part_library[17] = object18;
        part_library[18] = object19;
        part_library[19] = object20;
        part_library[20] = object21;
        part_library[21] = object22;
        part_library[22] = object23;
        part_library[23] = object24;
        part_library[24] = object25;
        part_library[25] = object26;
        part_library[26] = object27;
        part_library[27] = object28;
        part_library[28] = object29;
        part_library[29] = object30;

        for (int var_count = 0; var_count < 30; var_count++)
        {
            if (part_library[var_count] == null)
            {
                new_index = var_count;
                break;
            }
        }

        part_container = new GameObject[new_index];

        for (int index = 0; index < new_index; index++)
        {
            part_container[index] = part_library[index];
        }

        part_library = part_container;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
