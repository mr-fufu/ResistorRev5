using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatingValues : MonoBehaviour {

    public Slider bar_value;
    public Plating plating_value;
    private bool enemy_check;

	// Use this for initialization
	void Start () {
        plating_value = transform.parent.gameObject.GetComponent<Plating>();
        enemy_check = transform.parent.gameObject.GetComponent<StandardStatBlock>().ENEMY;
    }
	
	// Update is called once per frame
	void Update () {
        if (enemy_check == false)
        {
            bar_value.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 3, gameObject.transform.position.z);
        }
        else if (enemy_check == true)
        {
            bar_value.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 3, gameObject.transform.position.z);
        }

        bar_value.maxValue = plating_value.max_plating;
        bar_value.value = plating_value.current_plating;
    }
}
