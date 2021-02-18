using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlatingValues : MonoBehaviour {

    public Slider bar_value;
    public Plating plating_value;

	void Start () {
        plating_value = transform.parent.gameObject.GetComponent<Plating>();
    }
	
	void Update () {
		bar_value.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 3, gameObject.transform.position.z);
        bar_value.maxValue = plating_value.max_plating;
        bar_value.value = plating_value.current_plating;
    }
}
