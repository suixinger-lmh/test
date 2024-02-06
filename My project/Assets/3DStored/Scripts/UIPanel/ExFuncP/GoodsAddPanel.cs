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

        //����ѡ�н����¼�
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.GoodsSelect, SelectGoodsEvent);


        openGoodsBtn.onClick.AddListener(() => {
           
            if (!isgoodsopen)
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.CreateViewPanelOpen, "����ѡ�����"));
            else
                Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "����ѡ�����"));
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


            //    Vector3 pos = box.center + shelvesTrans.position;//�ݻ����ĵ�
            //    float floorHeight = pos.y - box.size.y / 2;//����߶�(���ܰ���)
            //    //����ê�㵽���λ�þ���
            //    float minheight = CommonHelper.FindLowPositionLoop(obj.transform).y;
            //    float objHeight = obj.transform.position.y - minheight;

            //    //����
            //    LayoutXZmin(pos, box.size,obj.GetComponent<BoxCollider>().size);

            //    //�����ĵ�����
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


    //ˢ�²���Ϣ
    void RefreshInf(List<GDGoods> list)
    {
        if(list == null)
        {
            nowgoodsName.text = "��";
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
        goodsSize.text = "�ȴ�����...";
        nowObjPrefab = null;
        //���ض�Ӧab��
        //������ɺ�ж��ab��������ʵ��
        Manager3DStored.Instance.GetStoredComponent<AssetBundleManager>().DoLoadOnce("/AssetBundle/Goods/", nowGoods.abRes.ABPath, nowGoods.abRes.AssetName, (go) =>
        {
            nowObjPrefab = go;
            BoxCollider box = go.GetComponent<BoxCollider>();
            //������Ϣ
            goodsSize.text = string.Format("��:{0:f2} ��:{1:f2} ��:{2:f2} ", box.size.x, box.size.z, box.size.y);
        });
    }

    //�������ȣ����ָ���




    List<GDGoods> nowSelectGD;
    Transform shelvesTrans;//��ǰ����
    List<GDGoods> GDList = new List<GDGoods>();//���ܹҵ�GD
    ShelvesLevel hasLevel;//�����Ƿ�ֲ�
    Vector3 boxContentSize;//�����ݻ�

    int levelLayer = -1;//0��Ϊȫ��
   
    public override void InitExFunc(ExFuncParam param)
    {
        //�ֲ�����
        levelDropdown.ClearOptions();
        List<Dropdown.OptionData> oplist = new List<Dropdown.OptionData>();
        oplist.Add(new Dropdown.OptionData("���в�"));
      
        //�Ƿ�ֲ㡾���͵���ͨ�����ĵ���������֡�
        GDList.Clear();
        shelvesTrans = param._shelves.transform;
        hasLevel = shelvesTrans.GetComponent<ShelvesLevel>();
        if (hasLevel != null && hasLevel.hasLevel)//�ֲ�
        {            
            //��¼ÿһ��Ĺҵ�����
            for(int i = 0; i < hasLevel.level; i++)
            {
                oplist.Add(new Dropdown.OptionData(string.Format("��{0}��", i + 1)));
                GDList.Add(AddGD(shelvesTrans.GetChild(i).Find("GD")));
            }
        }
        else//δ�ֲ�
        {
            hasLevel = null;//�ÿ�
            GDList.Add(AddGD(shelvesTrans.Find("GD")));
        }

        //�����˵�
        levelDropdown.AddOptions(oplist);
        levelLayer = 0;//��ǰѡ���
        nowSelectGD = GetGDByLayer(levelLayer);//��ǰѡ��ҵ���
        RefreshInf(nowSelectGD);

        //չʾ��Ϣ
        boxContentSize = GDList[0].GetGDContentSize();
        content.text = string.Format("��:{0} ��:{1} ��:{2} ", boxContentSize.x, boxContentSize.z, boxContentSize.y);
        nowgoodsName.text = "��";
        count.text = "";


        base.InitExFunc(param);
    }

    public override void ExitExPanel()
    {
        //����ѡ����Ϣ
        nowObjPrefab = null;
        nowGoods = null;
        goodsName.text = string.Empty;
        goodsSize.text = string.Empty;

        //����Ϣ
        levelLayer = -1;
        nowSelectGD = null;

        //�ر�
        if(isgoodsopen)
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent(CoreEventId.CreateViewPanelClose, "����ѡ�����"));
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



    #region ���ﲼ�ְڷ�

    //��x��С��z��С����ʼ�ڷ�
    void LayoutXZmin(Vector3 contentCenterPos,Vector3 contentSize,Vector3 objSize)
    {
        //xz��С����ԭ��
        Vector3 originPos = contentCenterPos - contentSize / 2;

        Vector3 pos1 = originPos + objSize / 2;
        pos1.y = originPos.y;

        GameObject obj = Instantiate(nowObjPrefab);
        obj.transform.position = pos1;
        obj.transform.SetParent(GDList[0].transform);


        //��������
        float spaceZ = 0.05f;
        int lineCount = Mathf.FloorToInt(  contentSize.z / (objSize.z + spaceZ));
        Debug.Log(lineCount);
     
        //��������
        float spaceX = 0.05f;
        int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));

        Debug.Log(xcount);
    }

    //x����������
    void LayoutXmin(List<BoxCollider> boxList, Vector3 contentSize, Vector3 objSize)
    {
        if (contentSize.x < objSize.x)
        {
            Debug.Log("�ߴ粻��");
            return;
        }

        //��������������� �� ���
        int maxCount = Mathf.FloorToInt(contentSize.x / objSize.x);
        float spaceX = (contentSize.x - maxCount * objSize.x) / (maxCount - 1);


        for (int i = 0; i < boxList.Count; i++)
        {
            //ÿ��������ĵ�
            Vector3 contentCenterPos = boxList[i].center  + boxList[i].transform.position;// +shelvesTrans.position
            //��ʼ�� x��y�ƶ������λ��
            Vector3 originPos = contentCenterPos - contentSize / 2;
            originPos.z = contentCenterPos.z;

            //x * objSize.x + (x- 1)* spaceX = contentsize.x
            //    x* objSize.x + x* spacex  - spacex =
            //int xcount = Mathf.FloorToInt(contentSize.x / (objSize.x + spaceX));
            //int xcount = Mathf.FloorToInt( (contentSize.x + spaceX) / (objSize.x + spaceX) );

           

            if (maxCount == 1)//ֻ��һ��ʱ��������
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
