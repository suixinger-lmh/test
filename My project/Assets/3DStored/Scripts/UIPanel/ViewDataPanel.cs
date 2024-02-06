using Custom.UIWidgets;
using FrameWork;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ViewDataPanel : CenterPanel
{
    public Button CloseBtn;

    public Text inf;
    protected override void Start()
    {
        Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.ViewData, OpenPanelByData);

        base.Start();

        CloseBtn.onClick.AddListener(CloseUIPanel);
    }


    void OpenPanelByData(CoreEvent ce)
    {
        TreeNode<TreeViewItem> item = (TreeNode<TreeViewItem>)ce.EventParam;
        StoredTreeViewItem storedTVI = item.Item as StoredTreeViewItem;

        string type = string.Empty;

        switch (item.Path.Count)
        {
            case 0:
                type = "�ֿ�";
                break;
            case 1:
                type = "��λ";
                break;
            case 2:
                type = "����";
                break;
        }
        inf.text = string.Empty;
        inf.text += "���ͣ�" + type +"\r\n";
        inf.text += "���ƣ�" + storedTVI.Name + "\r\n";
        inf.text += "ID��" + storedTVI.ID + "\r\n";

        OpenUIPanel();
    }


}
