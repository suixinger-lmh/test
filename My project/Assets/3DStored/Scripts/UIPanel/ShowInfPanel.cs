using Custom.UIWidgets;
using FrameWork;
using Stored3D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShowInfPanel : LeftUIPanel
{
    //[Header("列表参数")]
    //public ScrollRect scrollRect;
    //public Transform itemParent;//父物体
    //public GameObject itemPrefab;//item预制体
    //public float _itemWidth;//选项宽度
    //public float _itemSpiteWidth;//展开图片宽度
    //public float _fixSpace = 10f;//展示区左右合适补偿距离(0就贴上左右边)
    //public float _levelSpace = 15f;//多级退格距离

    //float _itemViewWidth;
    //float _itemViewPosX;
    //float _itemSpritePosX;

    //不自己写，直接用插件

    public InputField findText;
    public Button findBtn;
    public Button back;

    public TreeView tree;
    ObservableList<TreeNode<TreeViewItem>> sceneNodes;
    UnityAction<TreeNode<TreeViewItem>> selectAction;
    UnityAction<TreeNode<TreeViewItem>> unselectAction;

    bool isInSearch = false;
    public void NodeSelected(TreeNode<TreeViewItem> node)
    {
        //Debug.Log(node.Item.Name + " selected");


        if (isInSearch)
        {
            node = FindOriginNode(node,sceneNodes);
        }


        if (selectAction != null)
        {
            selectAction(node);
        }
            

        //Debug.Log(node.Path.Count);
        //string xx = "";
        //foreach(var x in node.Path)
        //{
        //    xx += x.Item.Name;
        //    Debug.Log(xx);
        //}
    }



   





    // called when node deselected
    public void NodeUnselected(TreeNode<TreeViewItem> node)
    {
        //Debug.Log(node.Item.Name + " deselected");
    }


    public void InitViewData(ObservableList<TreeNode<TreeViewItem>> stored,UnityAction<TreeNode<TreeViewItem>> selectDo, UnityAction<TreeNode<TreeViewItem>> unselectDo)
    {
        sceneNodes = stored;
        tree.Nodes = sceneNodes;
        selectAction = selectDo;
        unselectAction = unselectDo;


        OpenUIPanel();
    }

    //只做刷新
    public void RefreshViewData(ObservableList<TreeNode<TreeViewItem>> stored)
    {
        sceneNodes = stored;
        tree.Nodes = sceneNodes;
    }
    public void RefreshViewData()
    {
        tree.Refresh();
    }


    public override void CloseUIPanel()
    {
        DoClear();



        base.CloseUIPanel();
    }


    void DoClear()
    {
        sceneNodes = null;
        tree.Clear();
        selectAction = null;
        unselectAction = null;

        isInSearch = false;

    }




    void SortNode()
    {

    }


    protected override void Start()
    {
        base.Start();

        tree.NodeSelected.AddListener(NodeSelected);
        tree.NodeDeselected.AddListener(NodeUnselected);
        tree.Start();

        //查找
        findBtn.onClick.AddListener(() => {
            string findstr = findText.text;
            if (!string.IsNullOrEmpty(findstr))
            {
                isInSearch = true;
                ObservableList<TreeNode<TreeViewItem>> findNodes = new ObservableList<TreeNode<TreeViewItem>>();

                foreach (var tt in sceneNodes)
                {
                    findNodes.AddRange(FindListStr(tt, findstr));
                }

                //Debug.Log(findNodes.Count);查到数量
                tree.Nodes = findNodes;
            }
        });

        back.onClick.AddListener(() => {
            isInSearch = false;
            findText.text = string.Empty;
            tree.Nodes = sceneNodes;
        });

    }


  





    TreeNode<TreeViewItem> FindOriginNode(TreeNode<TreeViewItem> node, ObservableList<TreeNode<TreeViewItem>> sourceList)
    {
        TreeNode<TreeViewItem> result = sourceList.Find(p => p.Item.Name == node.Item.Name);
        if (result != null)
            return result;

        foreach(var tt in sourceList)
        {
            if(tt.Nodes!=null && tt.Nodes.Count > 0)
            {
                result = FindOriginNode(node, (ObservableList<TreeNode<TreeViewItem>>)tt.Nodes);
                if (result != null)
                    return result;
            }
        }
        return null;
    }

    ObservableList<TreeNode<TreeViewItem>> FindListStr(TreeNode<TreeViewItem> listnode, string findstr)
    {
        ObservableList<TreeNode<TreeViewItem>> findNodes = new ObservableList<TreeNode<TreeViewItem>>();
        if (listnode.Item.Name.Contains(findstr))
        {
            TreeViewItem item = new TreeViewItem(listnode.Item.Name);
            TreeNode<TreeViewItem> newNode = new TreeNode<TreeViewItem>(item);
            findNodes.Add(newNode);
        }
           

        if (listnode.Nodes != null && listnode.Nodes.Count > 0)
        {
            foreach (var tt in listnode.Nodes)
            {
                findNodes.AddRange(FindListStr(tt, findstr));
                //findNodes.Add();
            }
        }
        return findNodes;

    }


    // Start is called before the first frame update
    void Start1()
    {
        tree.NodeSelected.AddListener(NodeSelected);
        tree.NodeDeselected.AddListener(NodeUnselected);
        tree.Start();
       // RecordStored record = DoSave();

        //滚动区域宽度
        //float scrollViewWidth = 205f;
        //float itemWidth = 200f;//选项宽度
        //float itemSPWidth = 20f;//展开图片宽度
        //float itemspaceWidth = 10f;//展开图和展示内容间距，以及
        //float itemlevelSpace = 15f;//多级退格间距

        ////计算内容：
        //float itemViewWidth = itemWidth - itemSPWidth - itemspaceWidth - i * itemlevelSpace;//选项 展示区宽度    根据退格距离变化
        //float itemSPPosX = itemSPWidth / 2 + i * itemlevelSpace;//选项 展开图 位置
        //float itemViewPosX = itemViewWidth / 2 + itemSPWidth + itemspaceWidth / 2 + i * itemlevelSpace;



    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
