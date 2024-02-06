using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAuto
{
    
}

public class FactoryAuto
{
    public Info info;

    public List<AreaAuto> areaList;
    //public Factory model;//仓库绑定模型
}

public class AreaAuto
{
    public Info info;
    public Size size;//可不填，按照默认，或内部大小设置
    public string type;//该区域的类型【货架，筒仓，堆放】

    public int shelvesCount;//货架数量
    public int rowCount;//有几排

    public List<ShelvesAuto> shelvesList;//货架信息
}


public class ShelvesAuto
{
    public Info info;

    public Size size;

    public int level;//货架层数

    public List<GoodsAuto> goodsList;//每层对应的货物信息
}


public class GoodsAuto
{
    public Info info;
}













public class Size
{
    public float length;
    public float width;
    public float height;
}
public class Info
{
    public string id;//唯一标识
    public string name;//显示名称
}