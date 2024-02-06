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
    //�ֿ���Ϣ
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
                //��ȡ����
                GetRecoredData(id, (data) => {
                    //��¼��ṹ
                    SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(data);
                    Debug.Log(rs);
                    Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData,rs);
                }, (errormsg) => {
                    Debug.LogError("���ؼ�¼��Ϣʧ�ܣ�"+errormsg);
                });
            }
            else
            {
                //ֱ�Ӵ�
                //��������
                Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData);
            }
        });

        editBtn.onClick.AddListener(() =>
        {

            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            string id = storedTreeViewItem.ID;
            if (IsRecoredDataExist(id))
            {
                //��ȡ����
                GetRecoredData(id, (data) =>
                {
                    //��¼��ṹ
                    SaveStockInf_Fac rs = JsonMapper.ToObject<SaveStockInf_Fac>(data);
                    Debug.Log(rs);
                    Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData, rs,true);
                }, (errormsg) =>
                {
                    Debug.LogError("���ؼ�¼��Ϣʧ�ܣ�" + errormsg);
                });
            }
            else
            {
                //ֱ�Ӵ�
                //��������
                Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().QuickCreate(nowSelectData,null,true);
            }

            //��������
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
        //�õ��ֿ����ṹ
        ObservableList<TreeNode<TreeViewItem>> facListTemp = (ObservableList<TreeNode<TreeViewItem>>)ce.EventParam;

        //��������Ϊ�ر����
        if(facListTemp == null)
        {
            CloseUIPanel();
            return;
        }


        if(facListTemp != nowSelectData)
        {
            //��¼�µ����ṹ
            nowSelectData = facListTemp;

            //����ɵ�չʾ������Ϣ
            Manager3DStored.Instance.GetStoredComponent<UIPanelComponent>().GetUIPanel<QuickAutoCreatePanel>().CloseTreeQuick();

            //������Ϣ
            StoredTreeViewItem storedTreeViewItem = nowSelectData[0].Item as StoredTreeViewItem;
            nameText.text = storedTreeViewItem.Name;
            //idText.text = storedTreeViewItem.ID;
            idText.text = "��λ������" + nowSelectData[0].Nodes.Count;
            //�����TODO

            //�жϼ�¼��Ϣ�Ƿ����
            UpdataData(storedTreeViewItem.ID);

            OpenUIPanel();
        }
    }




    //��ǰ�ֿ��Ƿ���ڼ�¼������
    bool IsRecoredDataExist(string id)
    {
        //����streamingassets���Ƿ����id.json�ļ�
        string dirPath = PathExtensions.StreamingAssetsPath() + "/RecoredDir/" + id + ".json";
        //Debug.Log(dirPath);
        return File.Exists(dirPath);
    }

    void GetRecoredData(string id,UnityAction<string> acDo,UnityAction<string> errorDo)
    {
        //��ȡ������
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
