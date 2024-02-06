using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ﲻ�ܲ�ͬ
//GD���ؽű������ƻ������ɾ�Ĳ飬��Ϣ����
public class GDGoods : MonoBehaviour
{
    
   

    //�������Թ̶�
    Vector3 centerPos;//���ĵ�
    Vector3 contentSize;//�ݻ�
    Vector3 originPos;//�ڷ���ʼ��

    //��������(��Ҫ���ݻ���䶯)
    [SerializeField]
    Goods nowUseGoods;//��ǰ����
    int MaxCount;//�����������
    float SpaceX;//���
    [SerializeField]
    int nowCount;//��ǰ����

    List<string> goodsname;

    public string LineName;
    public Goods GetGoods() { return nowUseGoods; }
    public int GetNowCount() { return nowCount; }
    public List<string> GetNames() { return goodsname; }
    
    public void InitSelf()
    {
        //�̶�������ȡ
        centerPos = GetComponent<BoxCollider>().center + transform.position;// +shelvesTrans.position
        contentSize = GetComponent<BoxCollider>().size;
        //�ڷ���ʼλ��(���������)
        originPos = centerPos - contentSize / 2;//x�������
        originPos.z = centerPos.z;//z�����


        //�ɸ���
        if (nowUseGoods != null && transform.childCount>0)
        {
            UpdateInf(nowUseGoods, transform.GetChild(0).GetComponent<BoxCollider>().size);
        }
        else
        {
            nowUseGoods = null;
            nowCount = MaxCount = 0;
            SpaceX = 0;
        }
    }

    public Vector3 GetGDContentSize() { return contentSize; }
    public string GetContentStr()
    {
        if (nowUseGoods == null)
            return "-";
        return nowCount + "/" + MaxCount;
    }
    

    public string GetGoodsName()
    {
        if (nowUseGoods == null)
            return "��";

        return nowUseGoods.abRes.LabelName;
    }



    public void ClearAll()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
            i--;
        }
        nowCount = 0;
    }


    public void DeleteGoods()
    {
        if (nowCount > 0)
        {
            DestroyImmediate(transform.GetChild(nowCount-1).gameObject);
            nowCount--;
        }
    }
    void CreateGoods(int index,GameObject objPrefab)
    {
        if (MaxCount == 1)//ֻ��һ��ʱ��������
        {
            InstantiateOneObj(objPrefab, centerPos - new Vector3(0, contentSize.y / 2, 0));
        }
        else
        {
            Vector3 objSize = objPrefab.GetComponent<BoxCollider>().size;
            Vector3 pos1 = originPos + new Vector3(objSize.x / 2 + index * (objSize.x + SpaceX), 0, 0);
            InstantiateOneObj(objPrefab, pos1);
            //for (int m = 0; m < MaxCount; m++)
            //{
            //    InstantiateOneObj(nowObjPrefab, pos1);
            //    //GameObject obj = Instantiate(nowObjPrefab);
            //    //obj.transform.position = pos1;
            //    //obj.transform.SetParent(transform);
            //}
        }
    }

    //����һ��
    public void CreateAdd(Goods newGoods, GameObject prefabObj)
    {
        CreateAddN(newGoods, prefabObj, nowCount + 1);
    }

    public void CreateAddN(Goods newGoods,GameObject prefabObj,int n = 1)
    {
        //���ﲻͬ
        if (!IsSameGoods(newGoods))
        {
            //���ԭ����
            ClearAll();
        }
        UpdateInf(newGoods, prefabObj.GetComponent<BoxCollider>().size);
        if (nowCount < n)//����
        {
            while (nowCount < MaxCount && nowCount < n)
            {
                CreateGoods(nowCount, prefabObj);//����
            }
        }
        else//����
        {
            while (nowCount > n && nowCount>0)
            {
                DeleteGoods();//����
            }
        }
       
    }


    //����
    public void CreateAll(Goods newGoods,GameObject prefabObj)
    {
        CreateAddN(newGoods, prefabObj, MaxCount);
        ////���ﲻͬ
        //if (!IsSameGoods(newGoods))
        //{
        //    //���ԭ����
        //    ClearAll();
        //}
        //UpdateInf(newGoods, prefabObj.GetComponent<BoxCollider>().size);
        ////��������

        //while (nowCount < MaxCount)
        //{
        //    CreateGoods(nowCount, prefabObj);//����
        //}
    }


    //ˢ�»�������(��Ҫ�����ʶ�ͻ���ߴ�)
    void UpdateInf(Goods goods,Vector3 objSize)
    {
        //
        if (goods == null)
        {
            nowUseGoods = null;
            nowCount = MaxCount = 0;
            SpaceX = 0;
            return;
        }

        nowUseGoods = goods;
        nowCount = transform.childCount;//�ҵ��»�������
        SpaceX = 0;
        MaxCount = Mathf.FloorToInt(contentSize.x / objSize.x);
        if (MaxCount > 1)
            SpaceX = (contentSize.x - MaxCount * objSize.x) / (MaxCount - 1);
        else
            MaxCount = 1;//����Ȼ��ܳ���ֻ�ܷ�һ��TODO:�������ܵĻ��ﲻ�ܷ���
    }






  


    bool IsSameGoods(Goods newGoods)
    {
        if (nowUseGoods == null)//����ͬ����
            return true;
        if (newGoods.abRes.ABPath == nowUseGoods.abRes.ABPath && newGoods.abRes.AssetName == nowUseGoods.abRes.AssetName)
            return true;

        return false;
    }



    void LayoutXmin(Vector3 contentCenterPos, Vector3 contentSize, Vector3 objSize,GameObject nowObjPrefab)
    {
        if (contentSize.x < objSize.x)
        {
            Debug.Log("�ߴ粻��");
            return;
        }


         //ÿ��������ĵ�
         // Vector3 contentCenterPos = boxList[i].center + boxList[i].transform.position;// +shelvesTrans.position
         //��ʼ�� x��y�ƶ������λ��
        Vector3 originPos = contentCenterPos - contentSize / 2;
        originPos.z = contentCenterPos.z;

        //x * objSize.x + (x- 1)* spaceX = contentsize.x
        //    x* objSize.x + x* spacex  - spacex =
        //int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));
        //int xcount = Mathf.FloorToInt( (contentSize.x + spaceX) / (objSize.x + spaceX) );



        if (MaxCount == 1)//ֻ��һ��ʱ��������
        {
            InstantiateOneObj(nowObjPrefab, contentCenterPos - new Vector3(0, contentSize.y / 2, 0));
            //GameObject obj = Instantiate(nowObjPrefab);
            //obj.transform.position = contentCenterPos - new Vector3(0, contentSize.y / 2, 0);
            //obj.transform.SetParent(transform);
        }
        else
        {
            for (int m = 0; m < MaxCount; m++)
            {
                Vector3 pos1 = originPos + new Vector3(objSize.x / 2 + m * (objSize.x + SpaceX), 0, 0);

                InstantiateOneObj(nowObjPrefab,pos1);
                //GameObject obj = Instantiate(nowObjPrefab);
                //obj.transform.position = pos1;
                //obj.transform.SetParent(transform);
            }
        }
    }



    void InstantiateOneObj(GameObject prefab,Vector3 pos)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.position = pos;
        obj.transform.SetParent(transform);
        nowCount++;
    }
}
