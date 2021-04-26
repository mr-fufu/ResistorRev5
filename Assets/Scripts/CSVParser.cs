using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[ExecuteInEditMode]
public class CSVParser : MonoBehaviour
{
    public List<Part> parts = new List<Part>();
    
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = { '\"' };

    // Do to "ExecuteInEditMode" above, this is called by the Editor, not on start
#if UNITY_EDITOR
    
    [ContextMenu("UPDATE PREFABS FROM CSV")]
    private void Start()
    {
        LoadStatData();
        //WARNING: this updates the prefabs
        foreach(var part in parts)
        {
            part.UpdatePrefab();
        }
    }
#endif
    
    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public void LoadStatData()
    {
        parts.Clear();

        List<Dictionary<string, object>> data = Read("Stats");
        for (int i = 0; i < data.Count; i++)
        {
            string name = data[i]["PART NAME"].ToString();
            string manu = data[i]["MANUFACTURER"].ToString();
            string desc = data[i]["DESC"].ToString();
            string type = data[i]["TYPE"].ToString();
            string id = data[i]["ID"].ToString();

            if(name.Equals("PLACEHOLDER"))
                continue;

            try
            {
                int plate = Convert.ToInt32(data[i]["PLATE"]);
                int logic = Convert.ToInt32(data[i]["LOGIC"]);
                int range = Convert.ToInt32(data[i]["RANGE"]);
                int armor = Convert.ToInt32(data[i]["ARMOR"]);
                int speed = Convert.ToInt32(data[i]["SPEED"]);
                int fuel = Convert.ToInt32(data[i]["FUEL"]);
                int power = Convert.ToInt32(data[i]["POWER"]);
                int slots = Convert.ToInt32(data[i]["SLOTS"]);
                int cost = Convert.ToInt32(data[i]["COST"]);
                
                parts.Add(new Part(name, manu, desc, type, id,
                    plate, logic, range, armor, speed, fuel, power, slots, cost));
            }
            catch (Exception e)
            {
                Debug.LogError("Sam: one of your stats is NOT an int in the csv. " +
                                  "Please make sure they all are and there are NO empty strings either, use 0 for the stats.");
                //throw;
            }
        }
    }

}