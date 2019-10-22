using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartInspector : MonoBehaviour
{
    // The Part Inspector handles most of the UI interactions in the workshop. Primarily, for its namesake, it
    // registers the stats, name, and description of a part in the workshop. Otherwise, the inspector will display the
    // overall stats of the bot currently being built.

    public GameObject scene_manager;

    public Color positive_color;
    public Color negative_color;
    public Color neutral_color;
    public Color selected_color;

    public Color initial_name_color;
    public Color initial_text_color;

    public Gradient value_color;

    public Color part_name_color;
    public Color part_text_color;

    public GameObject PLATE_symbol;
    public GameObject ARMOR_symbol;
    public GameObject RANGE_symbol;
    public GameObject FUEL_symbol;
    public GameObject SPEED_symbol;
    public GameObject POWER_symbol;
    public GameObject LOGIC_symbol;
    public GameObject COST_symbol;

    public float bot_PLATE_value;
    public float bot_ARMOR_value;
    public float bot_RANGE_value;
    public float bot_FUEL_value;
    public float bot_SPEED_value;
    public float bot_POWER_value;
    public float bot_LOGIC_value;
    public float bot_COST_value;

    public Text PLATE_value;
    public Text RANGE_value;
    public Text ARMOR_value;
    public Text FUEL_value;
    public Text SPEED_value;
    public Text POWER_value;
    public Text LOGIC_value;
    public Text COST_value;

    public Text part_name;
    public Text part_description;

    public GameObject nixie_select;
    public GameObject select_stage_1;
    public GameObject select_stage_2;
    public GameObject select_stage_3;
    public GameObject select_stage_4;

    public GameObject start_button;

    private int PLATE;
    private int RANGE;
    private int ARMOR;
    private int FUEL;
    private int SPEED;
    private int POWER;
    private int LOGIC;
    private int COST;

    private bool button_mouse_over;
    private bool credits_showing;

    private GameObject selected_stage;
    private int selected_value;

    public float max_PLATE;
    public float max_RANGE;
    public float max_ARMOR;
    public float max_FUEL;
    public float max_SPEED;
    public float max_POWER;
    public float max_LOGIC;
    public float max_COST;

    private GradientColorKey[] color_key;

    private bool dragging;
    private GameObject selected_object;
    private bool show_info;

    public string holdover_name;
    public string holdover_description;

    public string displayed_name;
    public string displayed_description;

    public GameObject select_frame;

    void Start()
    {

    }

    void Update()
    {
        if (GetComponent<WorkshopDrag>().tutorial_complete)
        {
            selected_value = nixie_select.GetComponent<BarSelectParentLink>().selected_box;
            if (selected_value == 1)
            {
                selected_stage = select_stage_1;
            }
            else if (selected_value == 2)
            {
                selected_stage = select_stage_2;
            }
            else if (selected_value == 3)
            {
                selected_stage = select_stage_3;
            }
            else if (selected_value == 4)
            {
                selected_stage = select_stage_4;
            }

            var mouse_position = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D ray_hit = (Physics2D.Raycast(mouse_position, Vector2.zero, Mathf.Infinity));

            dragging = GetComponent<WorkshopDrag>().dragging;

            if (ray_hit.collider != null)
            {
                if (ray_hit.collider.gameObject == start_button)
                {
                    //if (scene_manager.GetComponent<SceneManagementScript>().start_true)
                    start_button.GetComponent<FlickerScript>().flicker = true;
                }

                if (ray_hit.collider.gameObject.tag == "DraggableUIPart")
                {
                    button_mouse_over = true;
                    PLATE = ray_hit.collider.gameObject.GetComponent<PartStats>().PLATE;
                    RANGE = ray_hit.collider.gameObject.GetComponent<PartStats>().RANGE;
                    ARMOR = ray_hit.collider.gameObject.GetComponent<PartStats>().ARMOR;
                    FUEL = ray_hit.collider.gameObject.GetComponent<PartStats>().FUEL;
                    SPEED = ray_hit.collider.gameObject.GetComponent<PartStats>().SPEED;
                    POWER = ray_hit.collider.gameObject.GetComponent<PartStats>().POWER;
                    LOGIC = ray_hit.collider.gameObject.GetComponent<PartStats>().LOGIC;
                    COST = ray_hit.collider.gameObject.GetComponent<PartStats>().COST;

                    displayed_name = ray_hit.collider.gameObject.GetComponent<PartStats>().part_name;
                    displayed_description = ray_hit.collider.gameObject.GetComponent<PartStats>().part_description;

                    select_frame.SetActive(true);
                    select_frame.transform.position = new Vector2(ray_hit.collider.gameObject.transform.parent.transform.position.x - 1, ray_hit.collider.gameObject.transform.parent.transform.position.y - 2);
                }
            }
            else if (dragging)
            {
                button_mouse_over = true;
                selected_object = GetComponent<WorkshopDrag>().selected_object.gameObject;

                PLATE = selected_object.GetComponent<PartStats>().PLATE;
                RANGE = selected_object.GetComponent<PartStats>().RANGE;
                ARMOR = selected_object.GetComponent<PartStats>().ARMOR;
                FUEL = selected_object.GetComponent<PartStats>().FUEL;
                SPEED = selected_object.GetComponent<PartStats>().SPEED;
                POWER = selected_object.GetComponent<PartStats>().POWER;
                LOGIC = selected_object.GetComponent<PartStats>().LOGIC;
                COST = selected_object.GetComponent<PartStats>().COST;

                displayed_name = selected_object.GetComponent<PartStats>().part_name;
                displayed_description = selected_object.GetComponent<PartStats>().part_description;

                select_frame.SetActive(true);
                select_frame.transform.position = new Vector2(selected_object.transform.parent.transform.position.x - 1, selected_object.transform.parent.transform.position.y - 2);
            }
            else
            {
                select_frame.SetActive(false);
                button_mouse_over = false;
            }

            if (displayed_name != holdover_name)
            {
                part_name.GetComponent<TypeWriter>().output_string = "";
                part_description.GetComponent<TypeWriter>().output_string = "";

                part_name.GetComponent<TypeWriter>().clear_character();
                part_description.GetComponent<TypeWriter>().clear_character();

                part_name.text = "";
                part_description.text = "";

                part_name.GetComponent<TypeWriter>().string_count = 0;
                part_description.GetComponent<TypeWriter>().string_count = 0;

                part_name.GetComponent<TypeWriter>().input_string = displayed_name;
                part_description.GetComponent<TypeWriter>().input_string = displayed_description;

                part_name.GetComponent<TypeWriter>().finish_typing = false;
                part_description.GetComponent<TypeWriter>().finish_typing = false;

                part_name.GetComponent<TypeWriter>().start_typing = true;

                holdover_name = displayed_name;
                holdover_description = displayed_description;
            }

            if (!part_description.GetComponent<TypeWriter>().start_typing && part_name.GetComponent<TypeWriter>().finish_typing)
            {
                part_description.GetComponent<TypeWriter>().start_typing = true;
            }

            if (button_mouse_over)
            {
                part_name.color = part_name_color;
                part_description.color = part_text_color;

                // PLATE
                //---------------------------------------------------------

                if (PLATE > 0)
                {
                    PLATE_value.text = "+" + PLATE;
                    PLATE_value.color = positive_color;
                    PLATE_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (PLATE < 0)
                {
                    PLATE_value.text = "-" + PLATE;
                    PLATE_value.color = negative_color;
                    PLATE_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (PLATE == 0)
                {
                    PLATE_value.text = "0";
                    PLATE_value.color = neutral_color;
                    PLATE_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // ARMOR
                //---------------------------------------------------------

                if (ARMOR > 0)
                {
                    ARMOR_value.text = "+" + ARMOR;
                    ARMOR_value.color = positive_color;
                    ARMOR_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (ARMOR < 0)
                {
                    ARMOR_value.text = "-" + ARMOR;
                    ARMOR_value.color = negative_color;
                    ARMOR_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (ARMOR == 0)
                {
                    ARMOR_value.text = "0";
                    ARMOR_value.color = neutral_color;
                    ARMOR_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // SPEED
                //---------------------------------------------------------

                if (SPEED > 0)
                {
                    SPEED_value.text = "+" + SPEED;
                    SPEED_value.color = positive_color;
                    SPEED_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (SPEED < 0)
                {
                    SPEED_value.text = "-" + SPEED;
                    SPEED_value.color = negative_color;
                    SPEED_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (SPEED == 0)
                {
                    SPEED_value.text = "0";
                    SPEED_value.color = neutral_color;
                    SPEED_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // POWER
                //---------------------------------------------------------

                if (POWER > 0)
                {
                    POWER_value.text = "+" + POWER;
                    POWER_value.color = positive_color;
                    POWER_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (POWER < 0)
                {
                    POWER_value.text = "-" + POWER;
                    POWER_value.color = negative_color;
                    POWER_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (POWER == 0)
                {
                    POWER_value.text = "0";
                    POWER_value.color = neutral_color;
                    POWER_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // FUEL
                //---------------------------------------------------------

                if (FUEL > 0)
                {
                    FUEL_value.text = "+" + FUEL;
                    FUEL_value.color = positive_color;
                    FUEL_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (FUEL < 0)
                {
                    FUEL_value.text = "-" + FUEL;
                    FUEL_value.color = negative_color;
                    FUEL_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (FUEL == 0)
                {
                    FUEL_value.text = "0";
                    FUEL_value.color = neutral_color;
                    FUEL_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // RANGE
                //---------------------------------------------------------

                if (RANGE > 0)
                {
                    RANGE_value.text = "+" + RANGE;
                    RANGE_value.color = positive_color;
                    RANGE_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (RANGE < 0)
                {
                    RANGE_value.text = "-" + RANGE;
                    RANGE_value.color = negative_color;
                    RANGE_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (RANGE == 0)
                {
                    RANGE_value.text = "0";
                    RANGE_value.color = neutral_color;
                    RANGE_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // LOGIC
                //---------------------------------------------------------

                if (LOGIC > 0)
                {
                    LOGIC_value.text = "+" + LOGIC;
                    LOGIC_value.color = positive_color;
                    LOGIC_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (LOGIC < 0)
                {
                    LOGIC_value.text = "-" + LOGIC;
                    LOGIC_value.color = negative_color;
                    LOGIC_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (LOGIC == 0)
                {
                    LOGIC_value.text = "0";
                    LOGIC_value.color = neutral_color;
                    LOGIC_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
                // COST
                //---------------------------------------------------------

                if (COST > 0)
                {
                    COST_value.text = "+" + COST;
                    COST_value.color = positive_color;
                    COST_symbol.GetComponent<SpriteRenderer>().color = positive_color;
                }
                else if (COST < 0)
                {
                    COST_value.text = "-" + COST;
                    COST_value.color = negative_color;
                    COST_symbol.GetComponent<SpriteRenderer>().color = negative_color;
                }
                else if (COST == 0)
                {
                    COST_value.text = "0";
                    COST_value.color = neutral_color;
                    COST_symbol.GetComponent<SpriteRenderer>().color = neutral_color;
                }
            }

            if (button_mouse_over == false)
            {
                part_name.GetComponent<TypeWriter>().start_typing = false;
                part_name.GetComponent<TypeWriter>().finish_typing = false;

                part_description.GetComponent<TypeWriter>().start_typing = false;
                part_description.GetComponent<TypeWriter>().finish_typing = false;

                part_name.GetComponent<TypeWriter>().input_string = "";
                part_description.GetComponent<TypeWriter>().input_string = "";

                part_name.text = "Part Inspector";
                part_description.text = " Bot Stats Displayed                          Hover Over Part Menu to Inspect";

                displayed_name = part_name.text;
                displayed_description = part_description.text;

                holdover_name = part_name.text;
                holdover_description = part_description.text;

                part_name.color = initial_name_color;
                part_description.color = initial_text_color;

                if (selected_stage.transform.GetChild(0).transform.childCount != 0 && selected_stage.transform.GetChild(0).GetChild(0).GetComponent<PartStats>() != null)
                {
                    bot_PLATE_value = (selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().PLATE);
                    PLATE_value.text = " " + bot_PLATE_value;
                    PLATE_value.color = value_color.Evaluate(bot_PLATE_value / max_PLATE);
                    PLATE_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_PLATE_value / max_PLATE);

                    bot_ARMOR_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().ARMOR;
                    ARMOR_value.text = " " + bot_ARMOR_value;
                    ARMOR_value.color = value_color.Evaluate(bot_ARMOR_value / max_ARMOR);
                    ARMOR_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_ARMOR_value / max_ARMOR);

                    bot_FUEL_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().FUEL;
                    FUEL_value.text = " " + bot_FUEL_value;
                    FUEL_value.color = value_color.Evaluate(bot_FUEL_value / max_FUEL);
                    FUEL_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_FUEL_value / max_FUEL);

                    bot_RANGE_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().RANGE;
                    RANGE_value.text = " " + bot_RANGE_value;
                    RANGE_value.color = value_color.Evaluate(bot_RANGE_value / max_RANGE);
                    RANGE_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_RANGE_value / max_RANGE);

                    bot_SPEED_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().SPEED;
                    SPEED_value.text = " " + bot_SPEED_value;
                    SPEED_value.color = value_color.Evaluate(bot_SPEED_value / max_SPEED);
                    SPEED_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_SPEED_value / max_SPEED);

                    bot_LOGIC_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().LOGIC;
                    LOGIC_value.text = " " + bot_LOGIC_value;
                    LOGIC_value.color = value_color.Evaluate(bot_LOGIC_value / max_LOGIC);
                    LOGIC_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_LOGIC_value / max_LOGIC);

                    bot_POWER_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().POWER;
                    POWER_value.text = " " + bot_POWER_value;
                    POWER_value.color = value_color.Evaluate(bot_POWER_value / max_POWER);
                    POWER_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_POWER_value / max_POWER);

                    bot_COST_value = selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().COST;
                    COST_value.text = " " + bot_COST_value;
                    COST_value.color = value_color.Evaluate(bot_COST_value / max_COST);
                    COST_symbol.GetComponent<SpriteRenderer>().color = value_color.Evaluate(bot_COST_value / max_COST);

                    ARMOR_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().ARMOR;
                    POWER_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().POWER;
                    FUEL_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().FUEL;
                    RANGE_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().RANGE;
                    SPEED_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().SPEED;
                    LOGIC_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().LOGIC;
                    COST_value.text = " " + selected_stage.transform.GetChild(0).transform.GetChild(0).GetComponent<StandardStatBlock>().COST;
                }
                else
                {
                    PLATE_value.text = "0";
                    PLATE_value.color = selected_color;
                    PLATE_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    ARMOR_value.text = "0";
                    ARMOR_value.color = selected_color;
                    ARMOR_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    POWER_value.text = "0";
                    POWER_value.color = selected_color;
                    POWER_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    FUEL_value.text = "0";
                    FUEL_value.color = selected_color;
                    FUEL_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    RANGE_value.text = "0";
                    RANGE_value.color = selected_color;
                    RANGE_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    SPEED_value.text = "0";
                    SPEED_value.color = selected_color;
                    SPEED_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    LOGIC_value.text = "0";
                    LOGIC_value.color = selected_color;
                    LOGIC_symbol.GetComponent<SpriteRenderer>().color = selected_color;

                    COST_value.text = "0";
                    COST_value.color = selected_color;
                    COST_symbol.GetComponent<SpriteRenderer>().color = selected_color;
                }
            }
        }
        else
        {
            part_name.text = "Part Inspector";
            part_description.text = " Bot Stats Displayed                          Hover Over Part Menu to Inspect";

            select_frame.SetActive(false);
        }

        
    }
}

