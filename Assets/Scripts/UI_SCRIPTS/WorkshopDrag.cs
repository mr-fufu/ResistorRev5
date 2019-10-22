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

    private int child_count_1;
    private int child_count_2;
    private int child_count_3;
    private int child_count_4;

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

    public const string drag_tag = "DraggableUIPart";
    public const string slot_tag = "UIPartSlot";
    private string selected_part_type;
    private string placed_slot_type;

    public bool dragging = false;

    private Vector2 original_position;
    private Vector3 mouse_location;
    private Vector3 original_scale;
    public Transform selected_object;
    public SpriteRenderer dragged_image;

    private GameObject hit_object;
    private bool occupied;
    private string original_sort_layer;
    private int original_sort_order;

    private int part_count;

    public GameObject head_slot_object;
    public GameObject torso_slot_object;
    public GameObject arm_slot_object;
    public GameObject leg_slot_object;
    public GameObject top_slot_object;
    public GameObject armor_slot_object;
    public GameObject fade_in_object;

    private bool initial_fade_completed;
    private float alpha = 1;
    private Color fade_in_color = new Vector4(1, 1, 1, 1);
    private Color full_transparent = new Vector4(1, 1, 1, 0);

    private Vector2 temp_position;

    // Use this for initialization
    void Start () {

        // find the selected spawn (in this case actually stage) based on the bar select parent link (contains only
        // a single int representing which box is currently selected by the player
        selected_spawn = spawn_selector.GetComponent<BarSelectParentLink>();

        // clears the selected object (representing the bot part currently picked up by the player)
        selected_object = null;

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

        // activate the fade_in object, basically a solid black shape that becomes transparent over time
        fade_in_object.SetActive(true);

        // placeholder color component that holds the color (RGB) of the fade_in object it begins with so that
        // we can alter only the alpha without changing the actual color (RGB) since we cannot change the alpha alone
        // of a color component attached to a gameobject but conversely ARE allowed to modify a single color component (color.a)
        // of an arbitrary color component we define. Its weird.
        fade_in_color = fade_in_object.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (!initial_fade_completed)
        {
            if (alpha >= 0)
            {
                fade_in_color.a = alpha;
                alpha -= 0.006f;
                fade_in_object.GetComponent<SpriteRenderer>().color = fade_in_color;
            }
            else
            {
                fade_in_object.GetComponent<SpriteRenderer>().color = full_transparent;
                initial_fade_completed = true;
                fade_in_color = new Vector4(0, 0, 0, 0);
            }
        }

        else{

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
                        selected_object = FindObjectsOnClick(drag_tag, false);

                        // if a valid object is found
                        if (selected_object != null)
                        {
                            // set dragging to true and record the selected object's part type
                            dragging = true;

                            selected_part_type = selected_object.gameObject.GetComponent<PartStats>().part_type;

                            // record the original position and scale
                            original_position = selected_object.position;
                            original_scale = selected_object.localScale;

                            // disable the selected object's collider
                            selected_object.GetComponent<Collider2D>().enabled = false;

                            // get the selected object's sprite renderer
                            dragged_image = selected_object.GetComponent<SpriteRenderer>();

                            // record the original sprite sort order
                            original_sort_layer = selected_object.GetComponent<SpriteRenderer>().sortingLayerName;
                            original_sort_order = selected_object.GetComponent<SpriteRenderer>().sortingOrder;
                        }
                    }
                }

            }

            if (dragging)
            {
                // if we are dragging a part then move that part to the cursor, change the scale
                // to 3 times in the x and y to match the scale of the stage area. Additionally, change
                // the sorting layer to "Overlay" and change the sort order to 100 (on top of everything else)

                // (NOTE) currently, we move the dragged image to the mouse, which causes the defined center point
                // of the sprite to follow the cursor. The problem is that we may have clicked the image at a diferent location
                // than the center point, which causes a strange disconnect and doesn't feel like accurate dragging.
                // proposed solution is to record the offset between the selected_part's center and the cursor and carry
                // that offset through while dragging. This might be quite a bit of work though for very little payoff though it would
                // be nice and I should get to it at some point.

                mouse_location = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

                selected_object.position = new Vector3(mouse_location[0], mouse_location[1], 0);

                selected_object.localScale = new Vector3(3, 3, 1);

                dragged_image.sortingLayerName = "Overlay";
                dragged_image.sortingOrder = 100;
            }

            if (Input.GetMouseButtonUp(0))
            {
                // if an object is currently selected on release
                if (selected_object != null)
                {
                    // find colliders under the mouse with the slot tag
                    var placement = FindObjectsOnClick(slot_tag, true);

                    // if there is a slot underneath the mouse
                    if (placement != null)
                    {
                        // record the slot's component type
                        placed_slot_type = placement.gameObject.GetComponent<PartStats>().slot_type;

                        // if the slot's component type matches the component type i.e. the part
                        // fits in the slot
                        if (placed_slot_type == selected_part_type)
                        {
                            // initiate state removal and destruction of all components
                            // previously attached to the slot
                            foreach (Transform slot_containment in placement.transform)
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

                            // instantiate the selected component as a child of the slot, then 
                            // adjust the component to be at the position and scale of the slot and
                            // if the selected component is not a "LEG" part then add the part stats to
                            // the standard stat block. Additionally, change the sort order of the 
                            // instantiated part back to the original sort order

                            var slot_component = Instantiate(selected_object.gameObject, placement.position, placement.rotation);

                            slot_component.transform.parent = placement.transform;

                            parent_count = 0;
                            child_count_1 = 0;
                            child_count_2 = 0;
                            child_count_3 = 0;
                            child_count_4 = 0;

                            if (slot_component.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 0;
                            }
                            else if (slot_component.transform.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 1;
                                child_count_1 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 2;
                                child_count_1 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                child_count_2 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 3;
                                child_count_1 = slot_component.transform.parent.parent.parent.parent.parent.GetSiblingIndex();
                                child_count_2 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                child_count_3 = slot_component.transform.parent.GetSiblingIndex();
                            }
                            else if (slot_component.transform.parent.parent.parent.parent.parent.parent.parent.parent.GetComponent<PartStats>().part_type == "LEG")
                            {
                                parent_count = 4;
                                child_count_1 = slot_component.transform.parent.parent.parent.parent.parent.parent.parent.GetSiblingIndex();
                                child_count_2 = slot_component.transform.parent.parent.parent.parent.parent.GetSiblingIndex();
                                child_count_3 = slot_component.transform.parent.parent.parent.GetSiblingIndex();
                                child_count_4 = slot_component.transform.parent.GetSiblingIndex();
                            }

                            selected_stage.GetComponent<SpawnList>().AddIndex(slot_component.GetComponent<PartStats>().part_name, slot_component.GetComponent<PartStats>().part_type, child_count_1, child_count_2, child_count_3, child_count_4, parent_count, slot_component);

                            // optional replace new instantiated component with component type name
                            // may be useful if locational damage is implemented
                            //slot_component.name = placed_slot_type;

                            slot_component.GetComponent<PartStats>().add_stats();

                            slot_component.GetComponent<PartStats>().list_index = (selected_stage.GetComponent<SpawnList>().name_list.Length - 1);

                            slot_component.transform.localScale = placement.transform.localScale;

                            if (slot_component.GetComponent<PartStats>().part_type == "ARM")
                            {
                                slot_component.transform.position = new Vector3(slot_component.transform.position.x, slot_component.transform.position.y, -slot_component.transform.position.y * 0.1f - 10);
                            }

                            slot_component.GetComponent<PartStats>().attached = true;
                            slot_component.gameObject.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;

                            fill_child_slots(selected_stage);

                            // return the selected component to its original position, scale and sort order

                            selected_object.position = original_position;
                            selected_object.localScale = original_scale;
                            selected_object.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;
                            selected_object.GetComponent<SpriteRenderer>().sortingLayerName = original_sort_layer;
                        }
                        else
                        {
                            selected_object.position = original_position;
                            selected_object.localScale = original_scale;
                            selected_object.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;
                            selected_object.GetComponent<SpriteRenderer>().sortingLayerName = original_sort_layer;
                        }
                    }
                    else
                    {
                        selected_object.position = original_position;
                        selected_object.localScale = original_scale;
                        selected_object.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;
                        selected_object.GetComponent<SpriteRenderer>().sortingLayerName = original_sort_layer;
                    }

                    selected_object.position = original_position;
                    selected_object.localScale = original_scale;
                    selected_object.GetComponent<SpriteRenderer>().sortingOrder = original_sort_order;
                    selected_object.GetComponent<SpriteRenderer>().sortingLayerName = original_sort_layer;

                    selected_object.GetComponent<Collider2D>().enabled = true;
                    selected_object = null;

                    dragging = false;
                }
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
            else if (ray_hit.collider.gameObject.tag == "slot component")
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
                                var leg_slot = Instantiate(leg_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 25;
                                leg_slot.transform.position = temp_position;
                                leg_slot.transform.localScale = new Vector3(2, 2, 1);
                                leg_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "ARM")
                            {
                                var arm_slot = Instantiate(arm_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.x -= 33;
                                temp_position.y += 17f;
                                arm_slot.transform.position = temp_position;
                                arm_slot.transform.localScale = new Vector3(1, 1, 1);
                                arm_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "TORSO")
                            {
                                var torso_slot = Instantiate(torso_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 12;
                                torso_slot.transform.position = temp_position;
                                torso_slot.transform.localScale = new Vector3(1, 1, 1);
                                torso_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "HEAD")
                            {
                                var head_slot = Instantiate(head_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 14;
                                head_slot.transform.position = temp_position;
                                head_slot.transform.localScale = new Vector3(1, 1, 1);
                                head_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "ARMOR")
                            {
                                var armor_slot = Instantiate(armor_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.x += 14;
                                armor_slot.transform.position = temp_position;
                                armor_slot.transform.localScale = new Vector3(1, 1, 1);
                                armor_slot.transform.parent = part_to_fill.transform.GetChild(var);
                            }
                            if (part_to_fill.transform.GetChild(var).GetComponent<PartStats>().slot_type == "TOPLARGE")
                            {
                                var top_slot = Instantiate(top_slot_object, part_to_fill.transform.GetChild(var).position, part_to_fill.transform.GetChild(var).rotation);
                                temp_position = part_to_fill.transform.GetChild(var).position;
                                temp_position.y += 15;
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

