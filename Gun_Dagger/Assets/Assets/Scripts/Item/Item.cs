using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Stat,
    Passive,
    Event,
    Mix
}

public class Item
{
    int index;
    public string ItemName;
    public byte tier;
    public bool IsReturn;
    public ItemType type;
    public string shortDescription;
    public string longDescription;
    public int[] value;
    public Player GetObj;
    public delegate void Execute();
    public delegate void EventProcess(bool p);
    public Execute GetExecute;
    public EventProcess GetEvent;

    public Item(int id, string name, bool ReturnRisk, int _type, byte t, string s_desc,string l_desc,int[] v)
    {
        index = id;
        ItemName = name;
        IsReturn = ReturnRisk;
        type = (ItemType)_type;
        tier = t;
        shortDescription = s_desc;
        longDescription = l_desc;
        value = v;
    }
}