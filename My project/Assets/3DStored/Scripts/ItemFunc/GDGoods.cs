using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//货物不能不同
//GD挂载脚本，复制货物的增删改查，信息数据
public class GDGoods : MonoBehaviour
{
    
   

    //货架属性固定
    Vector3 centerPos;//中心点
    Vector3 contentSize;//容积
    Vector3 originPos;//摆放起始点

    //货物属性(需要根据货物变动)
    [SerializeField]
    Goods nowUseGoods;//当前货物
    int MaxCount;//容纳最大数量
    float SpaceX;//间距
    [SerializeField]
    int nowCount;//当前数量

    List<string> goodsname;

    public string LineName;
    public Goods GetGoods() { return nowUseGoods; }
    public int GetNowCount() { return nowCount; }
    public List<string> GetNames() { return goodsname; }
    
    public void InitSelf()
    {
        //固定参数获取
        centerPos = GetComponent<BoxCollider>().center + transform.position;// +shelvesTrans.position
        contentSize = GetComponent<BoxCollider>().size;
        //摆放起始位置(居中左对齐)
        originPos = centerPos - contentSize / 2;//x向左对齐
        originPos.z = centerPos.z;//z向居中


        //可更新
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
            return "空";

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
        if (MaxCount == 1)//只有一个时放在中心
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

    //增加一个
    public void CreateAdd(Goods newGoods, GameObject prefabObj)
    {
        CreateAddN(newGoods, prefabObj, nowCount + 1);
    }

    public void CreateAddN(Goods newGoods,GameObject prefabObj,int n = 1)
    {
        //货物不同
        if (!IsSameGoods(newGoods))
        {
            //清空原来的
            ClearAll();
        }
        UpdateInf(newGoods, prefabObj.GetComponent<BoxCollider>().size);
        if (nowCount < n)//增加
        {
            while (nowCount < MaxCount && nowCount < n)
            {
                CreateGoods(nowCount, prefabObj);//生成
            }
        }
        else//减少
        {
            while (nowCount > n && nowCount>0)
            {
                DeleteGoods();//生成
            }
        }
       
    }


    //填满
    public void CreateAll(Goods newGoods,GameObject prefabObj)
    {
        CreateAddN(newGoods, prefabObj, MaxCount);
        ////货物不同
        //if (!IsSameGoods(newGoods))
        //{
        //    //清空原来的
        //    ClearAll();
        //}
        //UpdateInf(newGoods, prefabObj.GetComponent<BoxCollider>().size);
        ////可以增加

        //while (nowCount < MaxCount)
        //{
        //    CreateGoods(nowCount, prefabObj);//生成
        //}
    }


    //刷新货物属性(需要货物标识和货物尺寸)
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
        nowCount = transform.childCount;//挂点下货物数量
        SpaceX = 0;
        MaxCount = Mathf.FloorToInt(contentSize.x / objSize.x);
        if (MaxCount > 1)
            SpaceX = (contentSize.x - MaxCount * objSize.x) / (MaxCount - 1);
        else
            MaxCount = 1;//物体比货架长，只能放一个TODO:超出货架的货物不能放置
    }






  


    bool IsSameGoods(Goods newGoods)
    {
        if (nowUseGoods == null)//按相同处理
            return true;
        if (newGoods.abRes.ABPath == nowUseGoods.abRes.ABPath && newGoods.abRes.AssetName == nowUseGoods.abRes.AssetName)
            return true;

        return false;
    }



    void LayoutXmin(Vector3 contentCenterPos, Vector3 contentSize, Vector3 objSize,GameObject nowObjPrefab)
    {
        if (contentSize.x < objSize.x)
        {
            Debug.Log("尺寸不够");
            return;
        }


         //每层货架中心点
         // Vector3 contentCenterPos = boxList[i].center + boxList[i].transform.position;// +shelvesTrans.position
         //起始点 x，y移动到最低位置
        Vector3 originPos = contentCenterPos - contentSize / 2;
        originPos.z = contentCenterPos.z;

        //x * objSize.x + (x- 1)* spaceX = contentsize.x
        //    x* objSize.x + x* spacex  - spacex =
        //int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));
        //int xcount = Mathf.FloorToInt( (contentSize.x + spaceX) / (objSize.x + spaceX) );



        if (MaxCount == 1)//只有一个时放在中心
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
