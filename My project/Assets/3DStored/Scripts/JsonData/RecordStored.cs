using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordStored
{
    //记录信息
    public List<FactoryRecordInf> factoryRecordInfs;
    
}


//仓库信息
public class FactoryRecordInf
{
    public string Name;//仓库名称
    public Factory Datasource;//数据源

    public string Position;//位置信息

    public string ID;

    //区域信息
    public List<AreaRecordInf> AreaRecordInfs;
}

public class AreaRecordInf
{
    public string Name;//区域名称
    public List<string> PointPositions;//linerender所有点位
    public string Position;//位置信息

    public Area Datasource;
    public string ID;
    //货架信息
    public List<ShelvesRecordInf> ShelvesRecordInfs;
}

public class ShelvesRecordInf
{
    public string Name;//货架名称
    public string Position;//位置

    public Shelves Datasource;//数据源
    public LevelRecord LevleRecord;
    public string ID;
    public List<GoodsRecordInf> GoodsRecordInfs;
}

public class LevelRecord
{
    public int Level;//层数

}

public class GoodsRecordInf
{
    public List<string> Names;
    public string Name;
    public Goods Datasource;
    public int count;
    //public 
}