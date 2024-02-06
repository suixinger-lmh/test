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
    //public Factory model;//�ֿ��ģ��
}

public class AreaAuto
{
    public Info info;
    public Size size;//�ɲ������Ĭ�ϣ����ڲ���С����
    public string type;//����������͡����ܣ�Ͳ�֣��ѷš�

    public int shelvesCount;//��������
    public int rowCount;//�м���

    public List<ShelvesAuto> shelvesList;//������Ϣ
}


public class ShelvesAuto
{
    public Info info;

    public Size size;

    public int level;//���ܲ���

    public List<GoodsAuto> goodsList;//ÿ���Ӧ�Ļ�����Ϣ
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
    public string id;//Ψһ��ʶ
    public string name;//��ʾ����
}