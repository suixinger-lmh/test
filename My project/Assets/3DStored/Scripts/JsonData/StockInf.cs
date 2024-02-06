using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StockInf
{
    public string STOCKORGID;//id
    public string STOCKORGNAME;//name

    public List<Shelf> WAREHOUSES;//��֯��Ϣ
}

[System.Serializable]
public class Shelf
{
    public string code;//����
    public string id;//id
    public string name;//����
    public List<Shelf> shelf;

    public string pID;//�ϼ�id
}


//�ֿ⣺ID
//��λ��ID+����ģ��+���ܲ���+���
//����:ID

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