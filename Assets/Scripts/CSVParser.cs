using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class CSVParser : MonoBehaviour
{
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = { '\"' };

#if UNITY_EDITOR
    // Due to "ExecuteInEditMode" above, this is called by the Editor, not on start
    [ContextMenu("UPDATE PREFABS FROM CSV")]
    private void Start()
    {
        // doesn't work well with workshop :shrug:
        if (EditorSceneManager.GetActiveScene().name.Equals("Parts Matrix"))
        {
            LoadStatData();
        }
        UpdatePartsInScene();
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

#if UNITY_EDITOR
    public void LoadStatData()
    {
        List<Dictionary<string, object>> data = Read("Stats");
        for (int i = 0; i < data.Count; i++)
        {
            string partName = data[i]["PART NAME"].ToString();
            string manu = data[i]["MANUFACTURER"].ToString();
            string desc = data[i]["DESC"].ToString();
            string type = data[i]["TYPE"].ToString();
            string id = data[i]["ID"].ToString();
            bool isMelee = data[i]["WEAPON TYPE"].ToString().Equals("MELEE");

            if(partName.Equals("PLACEHOLDER"))
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
                
                GameObject prefab = GetAssociatedPrefab("PartPrefabs/" + type + "Prefabs/" + id + ".prefab");
                GameObject viewerPrefab = GetAssociatedPrefab("ViewerPrefabs/Viewer" + type + "/" + id + ".prefab");
                UpdatePartsObject(prefab, partName, manu, desc, type, id, isMelee,
                    plate, logic, range, armor, speed, fuel, power, slots, cost);
                UpdatePartsObject(viewerPrefab, partName, manu, desc, type, id,isMelee, 
                    plate, logic, range, armor, speed, fuel, power, slots, cost);
            }
            catch (Exception e)
            {
                Debug.LogError("Sam: one of your stats is NOT an int in the csv. " +
                                  "Please make sure they all are and there are NO empty strings either, use 0 for the stats.");
                //throw;
            }
        }
    }
    
    private void UpdatePartsObject(GameObject gameobj, string partName, string manu, string desc, string type, string id,
        bool isMelee, int plate, int logic, int range,int armor, int speed, int fuel, int power, int slots, int cost)
    {
        PartStats stats = gameobj.GetComponent<PartStats>();

        stats.partName = partName;
        stats.partDescription = desc;
        stats.partType = type;
        stats.meleeComponent = isMelee;
        
        stats.PLATE = plate;
        stats.LOGIC = logic;
        stats.RANGE = range;
        stats.ARMOR = armor;
        stats.SPEED = speed;
        stats.FUEL = fuel;
        stats.POWER = power;
        stats.COST = cost;
        
        //TODO: stats.slots, manufacturer
    }
    
    //find prefab in editor (before runtime) please
    private GameObject GetAssociatedPrefab(string fileName)
    {
        string prefabPath = "Assets/Resources/" + fileName;
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(UnityEngine.Object)) as GameObject;

        if (prefab == null)
        {
            Debug.LogError("Sam: Error finding prefab: " + name + ". Check csv file. Path: " + prefabPath);
        }

        return prefab;
    }

    private void UpdatePartsInScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();
        
        foreach (var obj in rootObjects)
        {
            var partStatsList = obj.GetComponentsInChildren<PartStats>(true);
            foreach (var partStats in partStatsList)
            {
                if (partStats.gameObject.name.Contains("Attachment"))
                {
                    partStats.slotComponent = true;
                    partStats.slotType = partStats.name.Replace("_AttachmentSlot", "");
                }
                
                if (PrefabUtility.IsPartOfPrefabThatCanBeAppliedTo(partStats.gameObject))
                {
                    //automated action => cant undo & no confirmation pop ups
                    PrefabUtility.ApplyPrefabInstance(partStats.gameObject, InteractionMode.AutomatedAction);
                }
            }
        }
    }
#endif
}