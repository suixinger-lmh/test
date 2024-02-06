using UnityEngine;
using System.Collections;
namespace FrameWork
{
    public class CoreEvent
    {
        // m_iEventId标记 接受的对象，m_iEventCode标记事件的内容
        protected CoreEventId m_iEventId = 0;
        protected object m_EventParam = null;

        public CoreEvent(CoreEventId eventId)
        {
            m_iEventId = eventId;

        }

        public CoreEvent(CoreEventId eventId, object eventParam)
        {
            m_iEventId = eventId;
            m_EventParam = eventParam;
        }

        public CoreEventId EventID
        {
            get { return m_iEventId; }
        }
        public object EventParam
        {
            get { return m_EventParam; }
            set { m_EventParam = value; }
        }
    }
    /// <summary>
    /// 自定义消息事件
    /// </summary>
    public enum CoreEventId
    {
        SystemNULL = 0,
        /// <summary>
        /// 操作引导
        /// </summary>
        OperationGuide = 1,
        /// <summary>
        /// 流程初始化完成
        /// </summary>
        ProcessInited = 2,
        /// <summary>
        /// 框架初始化完成
        /// </summary>
        FrameInited = 3,

        /// <summary>
        /// task变化
        /// </summary>
        TaskChange = 5,
        /// <summary>
        /// 向UI面板发送消息
        /// </summary>
        CallUIPanel = 6,
        /// <summary>
        /// 向操作发出结束消息
        /// </summary>
        CallUIOperationFinished = 7,


        /// <summary>
        /// 调用评分系统消息
        /// </summary>
        CallEvalutionPanel = 8,


        /// <summary>
        /// 向操作发出UI面板错误消息
        /// </summary>
        CallUIOperationError = 9,


        /// <summary>
        /// 操作完成事件
        /// </summary>
        OnOperationFinished = 10,

        /// <summary>
        /// 操作完成事件
        /// </summary>
        LanguageChange = 11,
        /// <summary>
        /// 当前的UI立即完成逻辑
        /// </summary>
        FinishUIPanelImmediately = 12,
        /// <summary>
        /// 回退当前的UI逻辑
        /// </summary>
        RevertUIPanel = 13,


        /// <summary>
        /// 检测碰撞
        /// </summary>
        BoxColliderEvent = 15,

        RepairOperationGuide = 16,

        /// <summary>
        /// 显示所有的ToolLabe
        /// </summary>
        ShowAllToolLabel = 51,
        /// <summary>
        /// ToolLabel响应模式改变消息。
        /// </summary>
        ToolLabelTypeChange = 52,
        /// <summary>
        /// PlaneControl射中物体消息。
        /// </summary>
        PlaneControlTarget = 53,

        /// <summary>
        /// PlaneControl射中物体消息。
        /// </summary>
        TimeLinePointPause = 54,


    


        /// <summary>
        /// 开始下载
        /// </summary>
        DownloadStartEventArgs = 10000,
        /// <summary>
        /// 下载进度
        /// </summary>
        DownloadUpdateEventArgs = 10001,
        /// <summary>
        /// 成功下载
        /// </summary>
        DownloadSuccessEventArgs = 10002,
        /// <summary>
        /// 下载失败
        /// </summary>
        DownloadFailureEventArgs = 10003,
        /// <summary>
        /// 资源开始加载
        /// </summary>
        ResourceUpdateStartEventArgs = 20000,
        /// <summary>
        /// 资源加载进度
        /// </summary>
        ResourceUpdateChangedEventArgs = 20001,
        /// <summary>
        /// 资源加载成功
        /// </summary>
        ResourceUpdateSuccessEventArgs = 20002,
        /// <summary>
        /// 资源加载失败
        /// </summary>
        ResourceUpdateFailureEventArgs = 20003,
        /// <summary>
        /// 行为树控制器通知事件
        /// </summary>
        BTCPStartEvent = 30000,

        CollisionEvent = 13000,
        TriggerEvent = 12000,
        GrabbedObj = 11000,
        JinDu = 14000,



        /// <summary>
        /// 列表面板通知
        /// </summary>
        ViewListCall = 60891,

        /// <summary>
        /// 取消选择
        /// </summary>
        CancelSelect = 60892,

        SelectGetObj = 60893,

        //拖拽组件事件
        DragItemCall = 60894,


        //分层
        ShelvesEdit_LevelSet = 60895,
        ShelvesEdit_full = 60896,
        //关闭
        ShelvesEdit_Close = 60897,
        //多功能编辑面板打开
        ShelvesEdit_Open = 60898,


        //面板打开
        CreateViewPanelOpen = 60899,
        //面板关闭
        CreateViewPanelClose = 608910,

        GoodsSelect = 608911,

        //标签
        ToolLabel_Create = 608912,
        ToolLabel_View = 608913,
        ToolLabel_Delete = 608915,

        ViewData = 608916,

        ToolLabel_Mode = 608917,
        ToolLabel_Mode_World = 608918,

        CreateViewPanelOBJEventChange = 608919,

        EditorPanel = 608920,


        RefreshTreeData = 608921,

        OpenStockInf = 608922,

        CameraLookAt = 608923,
    }

}