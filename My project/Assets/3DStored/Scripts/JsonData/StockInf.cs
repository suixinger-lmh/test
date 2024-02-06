using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockInf
{
    public string STOCKORGID;//id
    public string STOCKORGNAME;//name

    public List<Shelf> WAREHOUSES;//组织信息
}

[System.Serializable]
public class Shelf
{
    public string code;//编码
    public string id;//id
    public string name;//名称
    public List<Shelf> shelf;

    public string pID;//上级id
}


//仓库：ID
//库位：ID+货架模型+货架层数+间距
//货架:ID

[System.Serializable]
public class SaveStockInf_Fac
{
    public string ID;
    //public Factory modelData;
    public List<SaveStockInf_Area> areaList;
}
[System.Serializable]
public class SaveStockInf_Area
{
    public string ID;
    public Shelves modelData;
    public int level;
    public string x;
    public string y;
    public List<SaveStockInf_She> sheList;
}
[System.Serializable]
public class SaveStockInf_She
{
    public string ID;
}