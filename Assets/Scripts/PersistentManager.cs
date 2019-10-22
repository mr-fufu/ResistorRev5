using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour {

	public static PersistentManager single_instance { get; private set; }

    public int Value;

    private void Awake()
    {
        if (single_instance == null)
        {
            single_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
