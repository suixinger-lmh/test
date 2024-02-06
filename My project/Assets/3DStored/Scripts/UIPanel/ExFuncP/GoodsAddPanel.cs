using FrameWork;
using Stored3D;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoodsAddPanel : ExFuncPanelBase
{
    public Text content;
    public Text nowgoodsName;
    public Text count;
    public Text goodsName;
    public Text goodsSize;

    public Dropdown levelDropdown;

    public Button openGoodsBtn;

    public Button createOne;
    public Button createAll;
    public Button removeOne;
    public Button reomveAll;

    bool isgoodsopen = false;

    protected override void Start()
    {
        base.Start();

        //货物选中接收事件
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.GoodsSelect, SelectGoodsEvent);


        openGoodsBtn.onClick.AddListener(() => {
           
            if (!isgoodsopen)
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.CreateViewPanelOpen, "货物选择面板"));
            else
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "货物选择面板"));
            isgoodsopen = !isgoodsopen;
        });

        removeOne.onClick.AddListener(() => {
            if (nowSelectGD == null)
                return;
            for (int i = 0; i < nowSelectGD.Count; i++)
            {
                nowSelectGD[i].DeleteGoods();
            }
        });
        reomveAll.onClick.AddListener(() =>
        {
            if (nowSelectGD == null)
                return;
            for (int i = 0; i < nowSelectGD.Count; i++)
            {
                nowSelectGD[i].ClearAll();
            }
        });
        createOne.onClick.AddListener(() => {
            if (nowObjPrefab == null)
                return;
            if (nowSelectGD == null)
                return;
            for (int i = 0; i < nowSelectGD.Count; i++)
            {
                nowSelectGD[i].CreateAdd(nowGoods, nowObjPrefab);
            }
            RefreshInf(nowSelectGD);
            //if (nowObjPrefab != null)
            //{
            //    GameObject obj = Instantiate(nowObjPrefab);


            //    Vector3 pos = box.center + shelvesTrans.position;//容积中心点
            //    float floorHeight = pos.y - box.size.y / 2;//地面高度(货架板子)
            //    //物体锚点到最低位置距离
            //    float minheight = CommonHelper.FindLowPositionLoop(obj.transform).y;
            //    float objHeight = obj.transform.position.y - minheight;

            //    //布局
            //    LayoutXZmin(pos, box.size,obj.GetComponent<BoxCollider>().size);

            //    //在中心点生成
            //    obj.transform.position = new Vector3(pos.x, floorHeight + objHeight, pos.z);
            //    obj.transform.SetParent(box.transform);
            //}
        });





        createAll.onClick.AddListener(() => {
            if (nowObjPrefab == null) 
                return;
            if (nowSelectGD == null)
                return;

            for(int i = 0; i < nowSelectGD.Count; i++)
            {
                nowSelectGD[i].CreateAll(nowGoods,nowObjPrefab);
            }


            RefreshInf(nowSelectGD);
        });



        levelDropdown.onValueChanged.AddListener((index) =>
        {
            levelLayer = index;
            nowSelectGD = GetGDByLayer(levelLayer);
            RefreshInf(nowSelectGD);
            //Debug.Log(index);
        });


    }


    //刷新层信息
    void RefreshInf(List<GDGoods> list)
    {
        if(list == null)
        {
            nowgoodsName.text = "空";
            count.text = "";
        }
        else
        {
            if(list.Count == 1)
            {
                nowgoodsName.text = list[0].GetGoodsName();
                count.text = list[0].GetContentStr();
            }
            else
            {
                nowgoodsName.text = "/";
                count.text = "";
            } 
        }
    }


    List<GDGoods> GetGDByLayer(int layer)
    {
        if (layer < 0) 
            return null;

        if (layer == 0)
            return GDList;
        else
            return new List<GDGoods> { GDList[layer - 1] };
    }




    Goods nowGoods;
    GameObject nowObjPrefab;
    void SelectGoodsEvent(CoreEvent ce)
    {
        nowGoods = (Goods)ce.EventParam;

        goodsName.text = nowGoods.abRes.LabelName;
        goodsSize.text = "等待加载...";
        nowObjPrefab = null;
        //加载对应ab包
        //加载完成后卸载ab包并生成实例
        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Goods/", nowGoods.abRes.ABPath, nowGoods.abRes.AssetName, (go) =>
        {
            nowObjPrefab = go;
            BoxCollider box = go.GetComponent<BoxCollider>();
            //货物信息
            goodsSize.text = string.Format("长:{0:f2} 宽:{1:f2} 高:{2:f2} ", box.size.x, box.size.z, box.size.y);
        });
    }

    //超出长度，文字高亮




    List<GDGoods> nowSelectGD;
    Transform shelvesTrans;//当前货架
    List<GDGoods> GDList = new List<GDGoods>();//货架挂点GD
    ShelvesLevel hasLevel;//货架是否分层
    Vector3 boxContentSize;//货架容积

    int levelLayer = -1;//0层为全部
   
    public override void InitExFunc(ExFuncParam param)
    {
        //分层下拉
        levelDropdown.ClearOptions();
        List<Dropdown.OptionData> oplist = new List<Dropdown.OptionData>();
        oplist.Add(new Dropdown.OptionData("所有层"));
      
        //是否分层【多层和单层通过中心点的数量区分】
        GDList.Clear();
        shelvesTrans = param._shelves.transform;
        hasLevel = shelvesTrans.GetComponent<ShelvesLevel>();
        if (hasLevel != null && hasLevel.hasLevel)//分层
        {            
            //记录每一层的挂点物体
            for(int i = 0; i < hasLevel.level; i++)
            {
                oplist.Add(new Dropdown.OptionData(string.Format("第{0}层", i + 1)));
                GDList.Add(AddGD(shelvesTrans.GetChild(i).Find("GD")));
            }
        }
        else//未分层
        {
            hasLevel = null;//置空
            GDList.Add(AddGD(shelvesTrans.Find("GD")));
        }

        //下拉菜单
        levelDropdown.AddOptions(oplist);
        levelLayer = 0;//当前选择层
        nowSelectGD = GetGDByLayer(levelLayer);//当前选择挂点组
        RefreshInf(nowSelectGD);

        //展示信息
        boxContentSize = GDList[0].GetGDContentSize();
        content.text = string.Format("长:{0} 宽:{1} 高:{2} ", boxContentSize.x, boxContentSize.z, boxContentSize.y);
        nowgoodsName.text = "空";
        count.text = "";


        base.InitExFunc(param);
    }

    public override void ExitExPanel()
    {
        //货物选择信息
        nowObjPrefab = null;
        nowGoods = null;
        goodsName.text = string.Empty;
        goodsSize.text = string.Empty;

        //层信息
        levelLayer = -1;
        nowSelectGD = null;

        //关闭
        if(isgoodsopen)
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "货物选择面板"));
        isgoodsopen = false;
       

        base.ExitExPanel();

    }




    GDGoods AddGD(Transform gd)
    {
        GDGoods temp = gd.GetComponent<GDGoods>();
        if (temp == null)
            temp = gd.AddComponent<GDGoods>();

        temp.InitSelf();
        return temp;
    }



    #region 货物布局摆放

    //从x最小，z最小处开始摆放
    void LayoutXZmin(Vector3 contentCenterPos,Vector3 contentSize,Vector3 objSize)
    {
        //xz最小处，原点
        Vector3 originPos = contentCenterPos - contentSize / 2;

        Vector3 pos1 = originPos + objSize / 2;
        pos1.y = originPos.y;

        GameObject obj = Instantiate(nowObjPrefab);
        obj.transform.position = pos1;
        obj.transform.SetParent(GDList[0].transform);


        //计算行数
        float spaceZ = 0.05f;
        int lineCount = Mathf.FloorToInt(  contentSize.z / (objSize.z + spaceZ));
        Debug.Log(lineCount);
     
        //计算数量
        float spaceX = 0.05f;
        int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));

        Debug.Log(xcount);
    }

    //x方向上铺满
    void LayoutXmin(List<BoxCollider> boxList, Vector3 contentSize, Vector3 objSize)
    {
        if (contentSize.x < objSize.x)
        {
            Debug.Log("尺寸不够");
            return;
        }

        //计算容纳最大数量 和 间距
        int maxCount = Mathf.FloorToInt(contentSize.x / objSize.x);
        float spaceX = (contentSize.x - maxCount * objSize.x) / (maxCount - 1);


        for (int i = 0; i < boxList.Count; i++)
        {
            //每层货架中心点
            Vector3 contentCenterPos = boxList[i].center  + boxList[i].transform.position;// +shelvesTrans.position
            //起始点 x，y移动到最低位置
            Vector3 originPos = contentCenterPos - contentSize / 2;
            originPos.z = contentCenterPos.z;

            //x * objSize.x + (x- 1)* spaceX = contentsize.x
            //    x* objSize.x + x* spacex  - spacex =
            //int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));
            //int xcount = Mathf.FloorToInt( (contentSize.x + spaceX) / (objSize.x + spaceX) );

           

            if (maxCount == 1)//只有一个时放在中心
            {
                GameObject obj = Instantiate(nowObjPrefab);
                obj.transform.position = contentCenterPos - new Vector3(0, contentSize.y / 2, 0);
                obj.transform.SetParent(boxList[i].transform);
            }
            else
            {
                for (int m = 0; m < maxCount; m++)
                {
                    Vector3 pos1 = originPos + new Vector3(objSize.x / 2 + m * (objSize.x + spaceX), 0, 0);

                    GameObject obj = Instantiate(nowObjPrefab);
                    obj.transform.position = pos1;
                    obj.transform.SetParent(boxList[i].transform);

                }

            }
        }
    }
    #endregion
}
