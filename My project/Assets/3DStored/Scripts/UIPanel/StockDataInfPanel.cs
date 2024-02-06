using Custom.UIWidgets;
using FrameWork;
using LitJson;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StockDataInfPanel : RightUIPanel
{
    //仓库信息
    public Text nameText;
    public Text idText;

    public Text infText;
    public Button createBtn;

    public Button editBtn;

    public Button deleteBtn;
    public GameObject isShowDataBtn;

    //TreeNode<TreeViewItem>


    protected override void Start()
    {
        base.Start();

        if (Manager3DStored.Instance != null)
        {
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.OpenStockInf, OpenStockInf);
        }

        createBtn.onClick.AddListener(() =>
        {
            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            string id = storedTreeViewItem.ID;
            if (IsRecoredDataExist(id))
            {
                //读取数据
                GetRecoredData(id, (data) => {
                    //记录表结构
                    SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(data);
                    Debug.Log(rs);
                    Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData,rs);
                }, (errormsg) => {
                    Debug.LogError("加载记录信息失败！"+errormsg);
                });
            }
            else
            {
                //直接打开
                //快速生成
                Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData);
            }
        });

        editBtn.onClick.AddListener(() =>
        {

            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            string id = storedTreeViewItem.ID;
            if (IsRecoredDataExist(id))
            {
                //读取数据
                GetRecoredData(id, (data) =>
                {
                    //记录表结构
                    SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(data);
                    Debug.Log(rs);
                    Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData, rs,true);
                }, (errormsg) =>
                {
                    Debug.LogError("加载记录信息失败！" + errormsg);
                });
            }
            else
            {
                //直接打开
                //快速生成
                Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData,null,true);
            }

            //快速生成
            //Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().DoCheckData(nowSelectData,true);
        });


        deleteBtn.onClick.AddListener(() => {
            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            DeleteData(storedTreeViewItem.ID);
        });

    }


    ObservableList<TreeNode<TreeViewItem>> nowSelectData;
    void OpenStockInf(CoreEvent ce)
    {
        //拿到仓库树结构
        ObservableList<TreeNode<TreeViewItem>> facListTemp = (ObservableList<TreeNode<TreeViewItem>>)ce.EventParam;

        //无数据则为关闭面板
        if(facListTemp == null)
        {
            CloseUIPanel();
            return;
        }


        if(facListTemp != nowSelectData)
        {
            //记录新的树结构
            nowSelectData = facListTemp;

            //清理旧的展示生成信息
            Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().CloseTreeQuick();

            //基本信息
            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            nameText.text = storedTreeViewItem.Name;
            //idText.text = storedTreeViewItem.ID;
            idText.text = "库位数量：" + nowSelectData[0].Nodes.Count;
            //出入库TODO

            //判断记录信息是否存在
            UpdataData(storedTreeViewItem.ID);

            OpenUIPanel();
        }
    }




    //当前仓库是否存在记录表数据
    bool IsRecoredDataExist(string id)
    {
        //查找streamingassets下是否存在id.json文件
        string dirPath = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
        //Debug.Log(dirPath);
        return File.Exists(dirPath);
    }

    void GetRecoredData(string id,UnityAction<string> acDo,UnityAction<string> errorDo)
    {
        //获取表数据
        string path = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
        StartCoroutine(CommonHelper.GetText(path, (str) =>
        {
            acDo(str);

        }, (error) =>
        {
            errorDo(error);
        }));
    }

    void DeleteData(string id)
    {
        string dirPath = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
        File.Delete(dirPath);
        UpdataData(id);
    }


    void UpdataData(string id)
    {
        if (IsRecoredDataExist(id))
        {
            isShowDataBtn.SetActive(true);
        }
        else
        {
            isShowDataBtn.SetActive(false);
        }
    }


}
