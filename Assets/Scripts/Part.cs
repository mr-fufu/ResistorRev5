using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public string manufacturer;
    public string description;
    public string type;
    public string id;
    
    public int plate;
    public int logic;
    public int range;
    public int armor;
    public int speed;
    public int fuel;
    public int power;
    public int slots;
    public int cost;
    
    public Part(string name, string manu, string desc, string type, string id,
                int plate, int logic, int range, int armor, int speed, int fuel, int power, int slots, int cost)
    {
        //part general info
        this.name = name;
        this.manufacturer = manu;
        this.description = desc;
        this.type = type;
        this.id = id;
        
        //part's stats
        this.plate = plate;
        this.logic = logic;
        this.range = range;
        this.armor = armor;
        this.speed = speed;
        this.fuel = fuel;
        this.power = power;
        this.slots = slots;
        this.cost = cost;
    }

    public void UpdatePrefab()
    {
        GameObject prefab = GetAssociatedPrefab();
        if (prefab == null)
            return;
        
        PartStats stats = prefab.GetComponent<PartStats>();

        stats.partType = type;
        stats.PLATE = plate;
        stats.LOGIC = logic;
        stats.RANGE = range;
        stats.ARMOR = armor;
        stats.SPEED = speed;
        stats.FUEL = fuel;
        stats.POWER = power;
        stats.COST = cost;
        
        //TODO: stats.slots
    }

    //find prefab in editor, before runtime please
    private GameObject GetAssociatedPrefab()
    {
        string prefabPath = "Assets/Resources/PartPrefabs/" + type + "Prefabs/" + id + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object)) as GameObject;

        if (prefab == null)
        {
            Debug.LogError("Sam: Error finding prefab: " + name + ". Check Part.cs and csv file. Path: " + prefabPath);
        }

        return prefab;
    }
}