using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordStored
{
    //��¼��Ϣ
    public List<FactoryRecordInf> factoryRecordInfs;
    
}


//�ֿ���Ϣ
public class FactoryRecordInf
{
    public string Name;//�ֿ�����
    public Factory Datasource;//����Դ

    public string Position;//λ����Ϣ

    public string ID;

    //������Ϣ
    public List<AreaRecordInf> AreaRecordInfs;
}

public class AreaRecordInf
{
    public string Name;//��������
    public List<string> PointPositions;//linerender���е�λ
    public string Position;//λ����Ϣ

    public Area Datasource;
    public string ID;
    //������Ϣ
    public List<ShelvesRecordInf> ShelvesRecordInfs;
}

public class ShelvesRecordInf
{
    public string Name;//��������
    public string Position;//λ��

    public Shelves Datasource;//����Դ
    public LevelRecord LevleRecord;
    public string ID;
    public List<GoodsRecordInf> GoodsRecordInfs;
}

public class LevelRecord
{
    public int Level;//����

}

public class GoodsRecordInf
{
    public List<string> Names;
    public string Name;
    public Goods Datasource;
    public int count;
    //public 
}