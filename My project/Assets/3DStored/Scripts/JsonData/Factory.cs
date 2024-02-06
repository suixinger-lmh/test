using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stored3D
{
    [System.Serializable]
    public class StoredABItem
    {
        public string LabelName;//显示名称
        public string ABPath;//ab包名称
        public string AssetName;//ab包内资源名称
        public string ImagePath;//缩略图地址
    }

    [System.Serializable]
    public class ABItemBase 
    {
        public StoredABItem abRes = new StoredABItem();

        public Texture GetThumbnail() {
            if (Manager3DStored.Instance != null)
                return Manager3DStored.Instance.imageCache[abRes.ImagePath];
            return null;
        }
    }




    //仓库ab资源格式：
    //1.需要地面collider
    ////1.存在一个collider(射线检测移动操作)
    ////2.存在一层地面(放置操作)
    [System.Serializable]
    public class Factory: ABItemBase
    {
       // public StoredABItem abRes = new StoredABItem();


        public string Scale;

    }
    [System.Serializable]
    public class Area
    {
        public string AreaShape;//区域形状
        public string Usage;//库位用途【货架，筒仓，堆放】

        public void SetShape(DrawTool.DrawStyle style)
        {
            switch (style)
            {
                case DrawTool.DrawStyle.rect:
                    AreaShape = "矩形";
                    break;
                case DrawTool.DrawStyle.line:
                    AreaShape = "其他图形";
                    break;
                case DrawTool.DrawStyle.cicle:
                    AreaShape = "圆形";
                    break;

            }
        }
        public void SetUsage(DrawTool.DrawStyle style)
        {
            switch (style)
            {
                case DrawTool.DrawStyle.rect:
                    Usage = "矩形";
                    break;
                case DrawTool.DrawStyle.line:
                    Usage = "其他图形";
                    break;
                case DrawTool.DrawStyle.cicle:
                    Usage = "圆形";
                    break;

            }
        }
    }

    //货架item格式：
    //1.一个空物体作为父节点，用于挂载相关组件
    //2.boxcollider挂载在父节点上，其下不要有collider（将collider整合到父节点一个box上）

    //3.货架需要有 容量boxcollider  GD挂点(物体下新增cube来拖动容量大小，转为GD挂点，容量信息包含在挂点上)
    [System.Serializable]
    public class Shelves : ABItemBase
    {
       // public StoredABItem abRes = new StoredABItem();

        public string Usage;//三种用途：杂物，货架，筒仓 【不同用途有不同功能】



    }


    [System.Serializable]
    public class Goods : ABItemBase
    {
        //public StoredABItem abRes = new StoredABItem();
    }



}
