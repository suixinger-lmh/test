using Custom.UIWidgets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Stored3D
{
    public static class CommonHelper
    {
        /// <summary>
        /// 库位显示悬浮高度(linerender点位高度)
        /// </summary>
        public static float AreaLineFloatHeight = 1f;
        public const float AreaLineFloatHeight_Drawing = 0.1f;//库位绘制高度
        public static float GroundFLOORHEIGHT = 0;//地面位置

        public const float GRIDHEIGHT = -0.01f;//网格位置（在0位置时，和模型高度重合，会造成闪面）

        public static string Root = "[3DStroed]";

        public static string Factory = "[Factory]";
        public static string Floor = "Floor";
        public static string TempArea = "[TempArea]";
        public static string TempShelves = "[TempShelves]";


        #region 物体绑定系统组件

        //TODO:模型存在多个box，且位置高度不同
        public static bool IsSuitFactory(GameObject obj,Factory data)
        {
            //最外层存在box作为仓库地面
            if (obj.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("仓库模型转换失败！原因：最外层物体不存在地面BoxCollider！");
                Debug.LogError("模型资源异常！Res:" + data.abRes.ABPath + "/" + data.abRes.AssetName);
                return false;
            }
            return true;
        }
        public static StoredItemFactory BindFactory_(GameObject fac, Factory dataSource)
        {
            //模型转仓储对象
            StoredItemFactory _factoryItem = fac.AddComponent<StoredItemFactory>();
            _factoryItem.BindComponent(dataSource);//绑定组件
            return _factoryItem;
        }
        public static StoredItemArea BindArea_(GameObject area, Area data)
        {
            //模型转仓储对象
            StoredItemArea _areaItem = area.AddComponent<StoredItemArea>();
            _areaItem.BindComponent(data);//绑定组件
            return _areaItem;
        }






        public static bool IsSuitShelves(GameObject obj, Shelves data)
        {
            switch (data.Usage)
            {
                case "货架":
                    return IsSuitShelves_HJ(obj,data);
            }

            return true;
        }

        //货架资源满足条件：
        static bool IsSuitShelves_HJ(GameObject obj, Shelves data)
        {
            //1.最外层存在box作为货架碰撞
            if (obj.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("货架模型转换失败！原因：最外层物体不存在BoxCollider！");
                return false;
            }
            //2.子节点下存在GD节点，且节点有容积box
            Transform gd = obj.transform.Find("GD");
            if (gd == null)
            {
                Debug.LogError("货架模型转换失败！原因：货架子物体下缺少GD节点！");
                return false;
            }
            if (gd.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("货架模型转换失败！原因：GD节点无容积BoxCollider！");
                return false;
            }
            return true;
        }
        public static StoredItemShelves BindShelves_(GameObject she, Shelves dataSource)
        {
            //模型转仓储对象
            StoredItemShelves _shelvesItem = she.AddComponent<StoredItemShelves>();
            _shelvesItem.BindComponent(dataSource);//绑定组件
            return _shelvesItem;
        }



        #endregion



        #region Get

        /// <summary>
        /// 获取场景放置仓库的根物体
        /// </summary>
        /// <returns></returns>
        public static Transform GetFactoryRoot()
        {
            return GameObject.Find(string.Format("{0}/[EditorScene]/{1}", Root, Factory)).transform;
        }

        /// <summary>
        /// 获取仓库挂载库位的父物体
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Transform GetFactoryAreaParent(GameObject factory)
        {
            Transform areaTemp = factory.transform.Find(TempArea);
            if (areaTemp == null)
            {
                areaTemp = new GameObject(TempArea).transform;
                areaTemp.SetParent(factory.transform);
            }
            return areaTemp;
        }

        /// <summary>
        /// 获取区域挂载货架的父物体
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Transform GetFactoryShelvesParent(GameObject area)
        {
            Transform objParent = area.transform.Find(TempShelves);
            if (objParent == null)
            {
                objParent = new GameObject(TempShelves).transform;
                objParent.SetParent(area.transform);
            }
            return objParent;
        }


     

        public static Bounds GetObjBounds(GameObject obj)
        {
            return MeshTool.OnlyGetBounds(obj.transform);
        }

        /// <summary>
        /// 获取和物体的合适距离
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetObjSuitDistance(GameObject obj)
        {
            //根据物体尺寸获取合适的距离，这里+1是为了给过小的物体增加距离(TODO:动态调整此值)
            return GetObjBounds(obj).size.magnitude / 2 + 1;
        }


        #endregion


        /// <summary>
        /// 在centerPos的位置上重新绘制指定大小的线框
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x">x尺寸</param>
        /// <param name="y">z尺寸</param>
        /// <param name="centerPos">中心位置</param>
        public static void CreateAreaBoundsByLength(LineRenderer line,float x,float y,Vector3 centerPos)
        {
            line.transform.position = Vector3.zero;

            Vector3 startPos = centerPos + new Vector3(-x/2,0,y/2);
            line.positionCount = 5;
            line.SetPosition(0, startPos);
            line.SetPosition(1, startPos + new Vector3(x, 0, 0));
            line.SetPosition(2, startPos + new Vector3(x, 0, -y));
            line.SetPosition(3, startPos + new Vector3(0, 0, -y));
            line.SetPosition(4, startPos);
        }


        /// <summary>
        /// 为默认仓库(默认仓库为平面，修改scale和Collider的尺寸)尺寸修改
        /// </summary>
        /// <param name="defaultFac">默认仓库对象</param>
        /// <param name="facX"></param>
        /// <param name="facZ"></param>
        public static void CreateDefaultFactorySize(GameObject defaultFac, float facX, float facZ)
        {
            Transform floorObj = defaultFac.transform.GetChild(0);
            floorObj.localScale = new Vector3(facX * 0.1f, 1, facZ * 0.1f);
            defaultFac.GetComponent<BoxCollider>().size = new Vector3(facX, 0, facZ);
        }


        #region 下载

        public static IEnumerator GetText(string path, Action<string> at = null,Action<string> errorAc = null)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (errorAc != null)
                    errorAc(www.error);
                //Debug.LogError(www.error);
            }
            else
            {
                // Show results as text
#if UNITY_EDITOR
                Debug.Log(www.downloadHandler.text);
#endif
                if (at != null)
                    at(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }



        #endregion




        #region 碰撞体区域判定

        public static bool IsFactory(RaycastHit hit)
        {
            StoredItemBase tempBase = hit.transform.GetComponent<StoredItemBase>();
            if (tempBase != null && tempBase._type == StoredItemBase.StoredItemType.Factory)
                return true;
            return false;

            //if (hit.transform.parent.name == Factory)
            //    return true;
            //return false;
        }
       

        #endregion


        #region 起始点终止点计算

        //获取矩形的长宽 【按照y高度不变】
        /// <summary>
        /// 根据起始位置和终止位置，计算矩形长宽(默认y值一致)
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="endP"></param>
        /// <returns></returns>
        public static Vector2 DrawInfGet_RectInf(Vector3 startP,Vector3 endP)
        {
            float length = Mathf.Abs(endP.x - startP.x);
            float width = Mathf.Abs(endP.z - startP.z);
            return new Vector2(length,width);
        }

        /// <summary>
        /// 获取半径
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="endP"></param>
        /// <returns></returns>
        public static float DrawInfGet_CircleInf(Vector3 startP, Vector3 endP)
        {
            return Vector3.Distance(startP,endP)/2;

        }

        #endregion




        #region 数据

        public static string Vector3toString(Vector3 pos)
        {
            return string.Format("{0}/{1}/{2}", pos.x, pos.y, pos.z);
        }
        public static Vector3 String2Vector3(string str)
        {
            string[] tempstr = str.Split('/');
            float x = float.Parse(tempstr[0]);
            float y = float.Parse(tempstr[1]);
            float z = float.Parse(tempstr[2]);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 将x在(t1,t2)区间映射到(s1,s2)区间【注意超出区间的部分】
        /// </summary>
        /// <param name="x"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static float Remap(float x,float t1,float t2,float s1,float s2)
        {
            return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
        }


        #endregion


        #region 物体高度

        //情形：1，模型最低位置为起始    2，模型以某个物体位置为起始  
        //获取参照最低点的物体(由bounds获取，如果有比期望位置还低的物体需要剔除)

        //注意物体锚点差异影响//
        //根据期望最低位置物体，计算bounds的最低位置(世界坐标)    center.y - size.y/2
        //根据当前位置(世界坐标)和最低位置(世界坐标)  计算出高度差
        //修改物体新位置(世界坐标) = 原位置-高度差



        /// <summary>
        /// 物体世界坐标和自身地面位置的高度偏移量(避免锚点对物体高度影响)
        /// </summary>
        /// <param name="positon">物体当前世界坐标</param>
        /// <param name="groundObj">物体在当前世界坐标下的地面参照物体</param>
        /// <returns></returns>
        public static float GetObjSelfHeightValue(Vector3 positon, GameObject groundObj)
        {
            //计算ground位置
            float groundHeightWorld = GetObjGroundHeight(groundObj);
            //物体位置高度差
            float moveSizeY = positon.y - groundHeightWorld;
            //新坐标
            return moveSizeY;
        }
        /// <summary>
        /// 物体世界坐标和自身地面位置的高度偏移量(避免锚点对物体高度影响)
        /// </summary>
        /// <param name="positon">物体当前世界坐标</param>
        /// <param name="groundBox">物体在当前世界坐标下的地面参照物体</param>
        /// <returns></returns>
        public static float GetObjSelfHeightValue(Vector3 positon, BoxCollider groundBox)
        {
            //计算ground位置
            float groundHeightWorld = GetObjGroundHeight(groundBox);
            //物体位置高度差
            float moveSizeY = positon.y - groundHeightWorld;
            //新坐标
            return moveSizeY;
        }


        /// <summary>
        /// 将地面参照移动到指定高度后物体的新位置(世界坐标)
        /// </summary>
        /// <param name="position">物体当前位置(世界坐标)</param>
        /// <param name="groundBox">物体的地面参照</param>
        /// <param name="groundHeight">新的高度位置(世界坐标)</param>
        /// <returns>新位置(世界坐标)</returns>
        public static Vector3 GetObjPositionByGroundHeight(Vector3 position,GameObject groundObj,float groundHeight)
        {
            //获取当前地面物体的高度
            float groundObjHeightWorld = GetObjGroundHeight(groundObj);

            //地面移动到指定高度的偏移
            float groundMoveY = groundHeight - groundObjHeightWorld;

            //新高度
            float newPositionY = position.y + groundMoveY;

            return new Vector3(position.x, newPositionY, position.z);
        }

        /// <summary>
        /// 将地面参照移动到指定高度后物体的新位置(世界坐标)
        /// </summary>
        /// <param name="position">物体当前位置(世界坐标)</param>
        /// <param name="groundBox">物体的地面参照</param>
        /// <param name="groundHeight">新的高度位置(世界坐标)</param>
        /// <returns>新位置(世界坐标)</returns>
        public static Vector3 GetObjPositionByGroundHeight(Vector3 position, BoxCollider groundBox, float groundHeight)
        {
            //获取当前地面物体的世界高度
            float groundObjHeightWorld = GetObjGroundHeight(groundBox);

            //地面移动到指定高度的偏移
            float groundMoveY = groundHeight - groundObjHeightWorld;

            //新高度
            float newPositionY = position.y + groundMoveY;

            return new Vector3(position.x, newPositionY, position.z);
        }

        /// <summary>
        /// 获取物体地面高度(默认为底部位置)
        /// </summary>
        /// <param name="groundObj">物体</param>
        /// <param name="type">-1：box的底部位置；0：box的中心位置；1：box的顶部位置</param>
        /// <returns></returns>
        public static float GetObjGroundHeight(GameObject groundObj,int type = -1)
        {
            //获取当前地面物体的高度
            Bounds bounds = GetObjBounds(groundObj);

            switch (type)
            {
                case -1:
                    return bounds.center.y - bounds.size.y / 2;
                case 0:
                    return bounds.center.y;
                case 1:
                    return bounds.center.y + bounds.size.y / 2;
            }
            return bounds.center.y;
        }
        /// <summary>
        /// 获取物体地面高度(默认为底部位置)
        /// </summary>
        /// <param name="groundBox">物体collider</param>
        /// <param name="type">-1：box的底部位置；0：box的中心位置；1：box的顶部位置</param>
        /// <returns></returns>
        public static float GetObjGroundHeight(BoxCollider groundBox, int type = -1)
        {
            //获取当前地面物体的高度
            Bounds bounds = groundBox.bounds;

            switch (type)
            {
                case -1:
                    return bounds.center.y - bounds.size.y / 2;
                case 0:
                    return bounds.center.y;
                case 1:
                    return bounds.center.y + bounds.size.y / 2;
            }
            return bounds.center.y;
        }



        //遍历模型mesh上的点找最低点；模型需要开启可读
        [Obsolete("过时，用bounds的方式获取最低点")]
        public static Vector3 FindLowPositionLoop(Transform obj)
        {
            Transform[] allTrans = obj.GetComponentsInChildren<Transform>();
            Vector3 min = FindLowPosition(allTrans[0]);
            //找出每个组件最小位置
            for (int i = 0; i < allTrans.Length; i++)
            {
                Vector3 tempVector;
                tempVector = FindLowPosition(allTrans[i]);
                if (min.y > tempVector.y)
                    min = tempVector;
            }
            return min;
        }
        static Vector3 FindLowPosition(Transform obj)
        {
            if (obj.GetComponent<MeshFilter>() == null || !obj.GetComponent<MeshFilter>().mesh.isReadable)
                return Vector3.zero;
            Mesh objMesh = obj.GetComponent<MeshFilter>().mesh;
            Vector3[] objVertor = objMesh.vertices;
            Vector3 minVector = obj.TransformPoint(objVertor[0]);//模型点位的世界坐标位置
            Vector3 tempVector;
            for (int i = 0; i < objVertor.Length; i++)
            {
                tempVector = obj.TransformPoint(objVertor[i]);
                if (minVector.y > tempVector.y)
                    minVector = tempVector;
            }
            return minVector;
        }


        #endregion

    }
}

