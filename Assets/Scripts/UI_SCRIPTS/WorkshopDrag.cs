using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorkshopDrag : MonoBehaviour {

    // IMPORTANT! Major script! this is a big one, the workshop drag is the script responsible for building bots in the workshop. Attached to the main camera
    // in order to take advantage of the raycast (could have also put this somewhere else and linked the camera tbh). Also responsible for the tutorial slides
    // essentially, tutorial slides have a mini program in front that freezes everything else unless the tutorial is finished. Tutorial is a simple bit of code
    // that cycles through the slides on click. Actual workshop drag script uses a raycast to find valid parts in the part menus and drags and drops them onto
    // the appropriate slots. 

    // (CONFUSING AND IMPORTANT) In addition the workshop drag script also records how the robot was built. Basically, the workshop scene is NON-NETWORK but the
    // parts are NETWORKED. Networked prefabs require a network identity but do not function if the client is not connected to a network. Therefore, to circumvent this
    // the workshop part viewers actually includes part duplicates (includes name, description and stats)  but are non-networked parts (hereafter referred to as "showcase parts"). 
    // Workshop drag script then places the showcase parts on the stage and records on the stage (parent of the leg part slot) in the SpawnList script: the name, type and position of the placed part.
    // Then, when building the part, the PlayerSpawnScript in a NETWORKED scene goes through a library of NETWORKED parts (organized by part_type i.e. ARM, HEAD, TORSO, ETC) and finds the
    // corresponding part by type and name and spawns it in the appropriate position (position as in 1st child of 4th child of 2nd child in heiarchy etc, basically pinpointing a position on a family tree)

    public int tutorial_count;
    private GameObject[] tutorial;

    public GameObject stage_1;
    public GameObject stage_2;
    public GameObject stage_3;
    public GameObject stage_4;

    private int childCount1;
    private int childCount2;
    private int childCount3;
    private int childCount4;

    private int parent_count;

    public GameObject spawn_selector;
    private BarSelectParentLink selected_spawn;
    private GameObject selected_stage;

    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject tutorial3;
    public GameObject tutorial4;
    public GameObject tutorial5;
    public GameObject tutorial6;
    public GameObject tutorial7;
    public GameObject tutorial8;

    public bool tutorial_complete;

    public const string dragTag = "DraggableUIPart";
    public const string slotTag = "UIPartSlot";
    public const string botTag = "WorkshopBot";
    private string selectedPartType;
    private string placedSlotType;

    public bool dragging = false;

    private Vector2 originalPosition;
    private Vector3 mouseLocation;
    private Vector3 originalScale;
    public Transform selectedObject;
    public SpriteRenderer draggedImage;

    private string originalSortLayer;
    private int originalSortOrder;

    private int part_count;

    public GameObject headSlotObject;
    public GameObject torsoSlotObject;
    public GameObject armSlotObject;
    public GameObject legSlotObject;
    public GameObject topSlotObject;
    public GameObject armorSlotObject;

    public GameObject scrollBar;

    private bool initialFadeCompleted;
    private float alpha = 1;
    private Color fadeInColor = new Vector4(1, 1, 1, 1);
    private Color fullTransparent = new Vector4(1, 1, 1, 0);

    private Vector3 temp_position;

    // for testing purposes, create bot with these two parts
    [SerializeField] private GameObject L3Object;
    [SerializeField] private GameObject T7Object;

    void Start () {

        scrollBar.SetActive(false);

        // find the selected spawn (in this case actually stage) based on the bar select parent link (contains only
        // a single int representing which box is currently selected by the player
        selected_spawn = spawn_selector.GetComponent<BarSelectParentLink>();

        // clears the selected object (representing the bot part currently picked up by the player)
        selectedObject = null;

        // create an array of gameobjects by collating individual slides and activate only the first slide
        // and deactivate the rest. Set the tutorial counter to 0
        tutorial = new GameObject[8];

        tutorial_count = 0;

        tutorial1.SetActive(true);
        tutorial[0] = tutorial1;

        tutorial1.SetActive(false);
        tutorial[1] = tutorial2;

        tutorial1.SetActive(false);
        tutorial[2] = tutorial3;

        tutorial1.SetActive(false);
        tutorial[3] = tutorial4;

        tutorial1.SetActive(false);
        tutorial[4] = tutorial5;

        tutorial1.SetActive(false);
        tutorial[5] = tutorial6;

        tutorial1.SetActive(false);
        tutorial[6] = tutorial7;

        tutorial1.SetActive(false);
        tutorial[7] = tutorial8;

        tutorial[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RemovePart(FindObjectsOnClick(botTag, false).parent.transform);
        }

        // check for which stage is currently selected by the player. Stages do not need to be
        // activated/deactivated since the stage selector script does that for us (though it could be moved here to be simplified)
        // also, whichever stage is the selected stage will constantly call the fill_child_slots function which will continuously
        // find all slot_components on the stage and fill them with the corresponding part icon (more detail in the fill_child_slots function)
        if (selected_spawn.selected_box == 1)
        {
            selected_stage = stage_1;
            fill_child_slots(selected_stage);
        }
        else if (selected_spawn.selected_box == 2)
        {
            selected_stage = stage_2;
            fill_child_slots(selected_stage);
        }
        else if (selected_spawn.selected_box == 3)
        {
            selected_stage = stage_3;
            fill_child_slots(selected_stage);
        }
        else if (selected_spawn.selected_box == 4)
        {
            selected_stage = stage_4;
            fill_child_slots(selected_stage);
        }

        // before anything else, fade in the fade_in object by decreasing the alpha, setting the alpha component of the fade_in_color
        // to the alpha float variable then setting the fade_in_object's color to the intermediary fade_in_color. Once the alpha is less than
        // or equal to 0, set the fade_in_object to full transparent and set the initial fade to true (fade completed)

            // redundant safety code for testing in case the tutorial has been deactivated (can be removed later)
            if (tutorial_count == 0)
            {
                tutorial[0].SetActive(true);
            }
            else
            {
                tutorial[0].SetActive(false);
            }

            if (Input.GetMouseButtonDown(0))
            {
            // if the tutorial has not been completed then lmb will increase the tutorial count (and thus move to the next
            // slide) Once the tutorial is complete (tutorial_count == 8), tutorial complete = true
            if (!tutorial_complete)
            {
                tutorial_count++;

                for (int var = 0; var < 8; var++)
                {
                    if (var != tutorial_count)
                    {
                        tutorial[var].SetActive(false);
                    }
                    else if (var == tutorial_count)
                    {
                        tutorial[var].SetActive(true);
                    }
                }

                if (tutorial_count == 8)
                {
                    tutorial_complete = true;
                }
            }
            else
            {
                // upon pressing the lmb (once fade and tutorial complete), check if any colliders were beneath the point of 
                // mouse click. If the collider is attached to a valid UI part, then begin dragging
                // Additionally, record the original position and scale

                // if we are currently not dragging anything
                if (!dragging)
                {
                    // call the find objects on click for the drag tag, drop is false (since we are picking up a part) 
                    // and record the result in the selected_object gameobject
                    selectedObject = FindObjectsOnClick(dragTag, false);

                    // if a valid object is found
                    if (selectedObject != null)
                    {
                        // set dragging to true and record the selected object's part type
                        dragging = true;

                        selectedPartType = selectedObject.gameObject.GetComponent<PartStats>().part_type;

                        // record the original position and scale
                        originalPosition = selectedObject.position;
                        originalScale = selectedObject.localScale;

                        // disable the selected object's collider
                        selectedObject.GetComponent<BoxCollider2D>().enabled = false;

                        // get the selected object's sprite renderer
                        draggedImage = selectedObject.GetComponent<SpriteRenderer>();

                        // record the original sprite sort order
                        originalSortLayer = selectedObject.GetComponent<SpriteRenderer>().sortingLayerName;
                        originalSortOrder = selectedObject.GetComponent<SpriteRenderer>().sortingOrder;
                    }
                }
            }

            if (dragging)
            {
                // if we are dragging a part then move that part to the cursor, change the scale
                // to 3 times in the x and y to match the scale of the stage area. Additionally, change
                // the sorting layer to "Inspector" and change the sort order to 100 (on top of everything else)

                // (NOTE) currently, we move the dragged image to the mouse, which causes the defined center point
                // of the sprite to follow the cursor. The problem is that we may have clicked the image at a diferent location
                // than the center point, which causes a strange disconnect and doesn't feel like accurate dragging.
                // proposed solution is to record the offset between the selected_part's center and the cursor and carry
                // that offset through while dragging. This might be quite a bit of work though for very little payoff though it would
                // be nice and I should get to it at some point.

                mouseLocation = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

                selectedObject.position = new Vector3(mouseLocation[0], mouseLocation[1], 0);

                selectedObject.localScale = new Vector3(3, 3, 1);

                draggedImage.sortingLayerName = "Inspector";
                draggedImage.sortingOrder = 100;
            }

            if (Input.GetMouseButtonUp(0))
            {
                // if an object is currently selected on release
                if (selectedObject != null)
                {
                    // find colliders under the mouse with the slot tag
                    var placement = FindObjectsOnClick(slotTag, true);

                    // if there is a slot underneath the mouse
                    if (placement != null)
                    {
                        // record the slot's component type
                        placedSlotType = placement.gameObject.GetComponent<PartStats>().slot_type;

                        // if the slot's component type matches the component type i.e. the part
                        // fits in the slot
                        if (placedSlotType == selectedPartType)
                        {
                            // Calls the Function to remove parts that may currently be in the slot

                            RemovePart(placement.transform);

                            // instantiate the selected component as a child of the slot, then 
                            // adjust the component to be at the position and scale of the slot and
                            // if the selected component is not a "LEG" part then add the part stats to
                            // the standard stat block. Additionally, change the sort order of the 
                            // instantiated part back to the original sort order

                            var slot_component = Instantiate(selectedObject.gameObject, placement.position, placement.rotation);

                            slot_component.transform.parent = placement.transform;

                            parent_count = 0;
                            childCount1 = 0;
                            childCount2 = 0;
                            childCount3 = 0;
                            childCount4 = 0;

                            if (slot_component.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 0;
                            }
                            else if (slot_component.transform.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 1;
                                childCount1 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 2;
                                childCount1 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                childCount2 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 3;
                                childCount1 = slot_component.transform.parent.parent.parent.parent.parent.GetSiblingIndex();
                                childCount2 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                childCount3 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 4;
                                childCount1 = slot_component.transform.parent.parent.parent.parent.parent.parent.parent.GetSiblingIndex();
                                childCount2 = slot_component.transform.parent.parent.parent.parent.parent.GetSiblingIndex();
                                childCount3 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                childCount4 = slot_component.transform.parent.GetSiblingIndex();
                            }

                            selected_stage.GetComponent<SpawnList>().AddIndex(slot_component.GetComponent<PartStats>().part_name, slot_component.GetComponent<PartStats>().part_type, childCount1, childCount2, childCount3, childCount4, parent_count, slot_component);

                            // optional replace new instantiated component with component type name
                            // may be useful if locational damage is implemented
                            //slot_component.name = placed_slot_type;

                            slot_component.GetComponent<PartStats>().add_stats();

                            slot_component.GetComponent<PartStats>().list_index = (selected_stage.GetComponent<SpawnList>().name_list.Length - 1);

                            slot_component.transform.localScale = placement.transform.localScale;

                            if (slot_component.GetComponent<PartStats>().part_type == "ARM")
                            {
                                slot_component.transform.position = new Vector3(slot_component.transform.position.x, slot_component.transform.position.y, -slot_component.transform.position.y * 0.1f - 20);
                            }

                            slot_component.GetComponent<PartStats>().attached = true;
                            slot_component.gameObject.GetComponent<SpriteRenderer>().sortingOrder = originalSortOrder;

                            fill_child_slots(selected_stage);

                            // change the new component's tag

                            slot_component.tag = "WorkshopBot";

                            Destroy(slot_component.GetComponent<BoxCollider2D>());

                            slot_component.AddComponent<PolygonCollider2D>();

                            // return the selected component to its original position, scale and sort order

                            selectedObject.position = originalPosition;
                            selectedObject.localScale = originalScale;
                            selectedObject.GetComponent<SpriteRenderer>().sortingOrder = originalSortOrder;
                            selectedObject.GetComponent<SpriteRenderer>().sortingLayerName = originalSortLayer;
                        }
                        else
                        {
                            selectedObject.position = originalPosition;
                            selectedObject.localScale = originalScale;
                            selectedObject.GetComponent<SpriteRenderer>().sortingOrder = originalSortOrder;
                            selectedObject.GetComponent<SpriteRenderer>().sortingLayerName = originalSortLayer;
                        }
                    }
                    else
                    {
                        selectedObject.position = originalPosition;
                        selectedObject.localScale = originalScale;
                        selectedObject.GetComponent<SpriteRenderer>().sortingOrder = originalSortOrder;
                        selectedObject.GetComponent<SpriteRenderer>().sortingLayerName = originalSortLayer;
                    }

                    selectedObject.position = originalPosition;
                    selectedObject.localScale = originalScale;
                    selectedObject.GetComponent<SpriteRenderer>().sortingOrder = originalSortOrder;
                    selectedObject.GetComponent<SpriteRenderer>().sortingLayerName = originalSortLayer;

                    selectedObject.GetComponent<BoxCollider2D>().enabled = true;
                    selectedObject = null;

                    dragging = false;
                }
            }

            // TESTING ONLY - cheat fill bot up (not visually tho)
            if (Input.GetKeyDown(KeyCode.B))
            {
                GameObject[] stages = new GameObject[4] { stage_1, stage_2, stage_3, stage_4 };
                for(int i = 0; i < stages.Length; i++)
                {
                    var slot_component = Instantiate(L3Object.gameObject, Vector3.zero, Quaternion.identity);
                    var slot_component2 = Instantiate(T7Object.gameObject, Vector3.zero, Quaternion.identity);

                    stages[i].GetComponent<SpawnList>().AddIndex(slot_component.GetComponent<PartStats>().part_name, slot_component.GetComponent<PartStats>().part_type, childCount1, childCount2, childCount3, childCount4, parent_count, slot_component);
                    stages[i].GetComponent<SpawnList>().AddIndex(slot_component2.GetComponent<PartStats>().part_name, slot_component2.GetComponent<PartStats>().part_type, childCount1, childCount2, childCount3, childCount4, parent_count, slot_component2);

                    slot_component.transform.SetParent(stages[i].transform);
                    slot_component2.transform.SetParent(stages[i].transform);
                }
            }
        }
    }

    // initiate state removal and destruction of all components
    // previously attached to the slot

    public void RemovePart(Transform part_to_remove)
    {
        foreach (Transform slot_containment in part_to_remove)
        {
            if (slot_containment.GetComponent<PartStats>() != null)
            {
                if (slot_containment.GetComponent<PartStats>().part_type != "LEG")
                {
                    slot_containment.gameObject.GetComponent<PartStats>().remove_stats();
                    selected_stage.GetComponent<SpawnList>().RemoveIndex(slot_containment.gameObject.GetComponent<PartStats>().list_index);
                    remove_children(slot_containment.gameObject);
                }
                else
                {
                    selected_stage.GetComponent<SpawnList>().ClearIndex();
                }
                GameObject.Destroy(slot_containment.gameObject);
            }
            else
            {
                Destroy(slot_containment.gameObject);
            }
        }
    }

    // use raycast to find all colliders underneath the clicked position in world space
    // that have the tag given to the function

    private Transform FindObjectsOnClick(string find_tag, bool drop)
    {
        var click_position = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        //click_position = Input.mousePosition;

        RaycastHit2D ray_hit = (Physics2D.Raycast(click_position, Vector2.zero, Mathf.Infinity));

        if (ray_hit.collider != null)
        {
            if (ray_hit.collider.gameObject.tag == find_tag)
            {
                return ray_hit.collider.gameObject.transform;
            }
            else if (ray_hit.collider.gameObject.tag == "slot component" || ray_hit.collider.gameObject.tag == botTag)
            {
                if (drop)
                {
                    return ray_hit.collider.transform.parent.transform;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void remove_children(GameObject remove_object)
    {
        part_count = remove_object.transform.childCount;

        if (part_count != 0)
        {
            for (int var = 0; var < (part_count); var++)
            {
                if (remove_object.transform.GetChild(var).GetComponent<PartStats>() != null)
                {
                    if (remove_object.transform.GetChild(var).GetComponent<PartStats>().slot_component)
                    {
                        if (remove_object.transform.GetChild(var).childCount != 0)
                        {
                            if (remove_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<PartStats>() != null)
                            {
                                if (remove_object.transform.GetChild(var).GetChild(0).gameObject.transform.childCount != 0)
                                {
                                    remove_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<PartStats>().remove_stats();

                                    remove_children(remove_object.transform.GetChild(var).GetChild(0).gameObject);

                                    selected_stage.GetComponent<SpawnList>().RemoveIndex(remove_object.transform.GetChild(var).GetChild(0).GetComponent<PartStats>().list_index);

                                    part_count = remove_object.transform.childCount;
                                }
                                else
                                {
                                    remove_object.transform.GetChild(var).GetChild(0).gameObject.GetComponent<PartStats>().remove_stats();

                                    selected_stage.GetComponent<SpawnList>().RemoveIndex(remove_object.transform.GetChild(var).GetChild(0).GetComponent<PartStats>().list_index);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void fill_child_slots(GameObject part_to_fill)
    {
        part_count = part_to_fill.transform.childCount;

        if (part_count != 0)
        {
            for (int var = 0; var < (part_count); var++)
            {
                if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>() != null)
                {
                    if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_component)
                    {
                        if (part_to_fill.transform.GetChild(var).childCount != 0)
                        {
                            if (part_to_fill.transform.GetChild(var).GetChild(0).gameObject.transform.childCount != 0)
                            {
                                fill_child_slots(part_to_fill.transform.GetChild(var).GetChild(0).gameObject);
                                part_count = part_to_fill.transform.childCount;
                            }
                        }
                        else
                        {
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "LEG")
                            {
                                var leg_slot = Instantiate(legSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 25;
                                temp_position.z -= 100;
                                leg_slot.transform.position = temp_position;
                                leg_slot.transform.localScale = new Vector3(2, 2, 1);
                                leg_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "ARM")
                            {
                                var arm_slot = Instantiate(armSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.x -= 33;
                                temp_position.y += 17f;
                                temp_position.z -= 50;
                                arm_slot.transform.position = temp_position;
                                arm_slot.transform.localScale = new Vector3(1, 1, 1);
                                arm_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "TORSO")
                            {
                                var torso_slot = Instantiate(torsoSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 12;
                                temp_position.z -= 100;
                                torso_slot.transform.position = temp_position;
                                torso_slot.transform.localScale = new Vector3(1, 1, 1);
                                torso_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "HEAD")
                            {
                                var head_slot = Instantiate(headSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 14;
                                temp_position.z -= 100;
                                head_slot.transform.position = temp_position;
                                head_slot.transform.localScale = new Vector3(1, 1, 1);
                                head_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "ARMOR")
                            {
                                var armor_slot = Instantiate(armorSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.x += 14;
                                temp_position.z -= 100;
                                armor_slot.transform.position = temp_position;
                                armor_slot.transform.localScale = new Vector3(1, 1, 1);
                                armor_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "TOPLARGE")
                            {
                                var top_slot = Instantiate(topSlotObject, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 15;
                                temp_position.z -= 100;
                                top_slot.transform.position = temp_position;
                                top_slot.transform.localScale = new Vector3(1, 1, 1);
                                top_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                        }
                    }
                }
            }
        }
    }
}

