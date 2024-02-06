using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: 需不需要对切分出的物体做隔离，切分出的物体 不能够再分层
public class ShelvesLevel : MonoBehaviour
{
    public static string CombineName = "combine";

    //记录原物体
    public GameObject shelvesObj;

    public GameObject newShelves;

    public int level = 1;


    //保留原始box信息
    public Vector3 boxCenter;
    public Vector3 boxSize;
    public float moveHeight;

    public bool hasLevel = false;//是否已经分层






    //记录未分层时的信息，只在第一次执行
    //public void SaveOrigin(GameObject obj)
    //{
    //    shelvesObj = obj;

    //    //记录原box信息
    //    BoxCollider objBox = shelvesObj.GetComponent<BoxCollider>();
    //    boxCenter = objBox.center;
    //    boxSize = objBox.size;
    //    moveHeight = objBox.bounds.size.y; //高度//堆叠高度按照碰撞体高度来算
    //}
    public void SaveOrigin(GameObject obj)
    {
        shelvesObj = obj;

        //记录原box信息
        BoxCollider objBox = shelvesObj.GetComponent<BoxCollider>();
        boxCenter = objBox.center;
        boxSize = objBox.size;
        moveHeight = objBox.bounds.size.y; //高度//堆叠高度按照碰撞体高度来算
    }


    //返回最初状态
    public void BackState()
    {
        //从分层状态还原
        if (hasLevel)
        {
            //找到combine
            GameObject combine = shelvesObj.transform.Find(CombineName).gameObject;
            if (combine != null)
            {
                ClearOtherCombineObj(combine);
                //将combine下的物体还原到父节点
                for (int i = 0, t = combine.transform.childCount; i < t; i++)
                {
                    combine.transform.GetChild(0).SetParent(shelvesObj.transform);
                }
                //删除combine自身
                DestroyImmediate(combine);
                //box还原
                shelvesObj.GetComponent<BoxCollider>().center = boxCenter;
                shelvesObj.GetComponent<BoxCollider>().size = boxSize;
                level = 1;
                //分层标记取消
                hasLevel = false;
            }
        }
    }


    //public void BackBoxInf()
    //{
    //    //box还原
    //    shelvesObj.GetComponent<BoxCollider>().size = boxSize;
    //    shelvesObj.GetComponent<BoxCollider>().center = boxCenter;
        

    //    level = 1;
    //}


    [Obsolete("直接复制对象每个都会带不需要的组件,不合适")]
    public void SetLevel(int le)
    {
        //删除指定个数

        if (newShelves != null)
        {
            DestroyImmediate(newShelves);
        }

        //生成新的父物体
        GameObject newParent = new GameObject(shelvesObj.name);
        newParent.transform.SetParent(shelvesObj.transform.parent);
      
        //物体复制组合
        for (int i = 0; i < le; i++)
        {
            Instantiate(shelvesObj, new Vector3(0, moveHeight * i, 0), Quaternion.Euler(0, 0, 0), newParent.transform);
        }

        //这里父物体位置做移动，后边box就可以不增加偏移
        newParent.transform.position = shelvesObj.transform.position;
        newParent.transform.rotation = shelvesObj.transform.rotation;

        //box重新添加
        BoxCollider newBox = newParent.AddComponent<BoxCollider>();
        newBox.size = new Vector3(boxSize.x, boxSize.y * le, boxSize.z);
        newBox.center = new Vector3(boxCenter.x, boxCenter.y + (le-1)*(moveHeight/2), boxCenter.z);
        //box受到物体自身参数影响，
        //newBox.center += shelvesObj.transform.position;

        //组件重新添加
        newShelves = newParent;
        
    }

    //新的复制方式，只复制模型物体，不带组件，还是用同一套功能组件
    public void SetLevel_new(int le)
    {
        GameObject combineP;
        if (hasLevel)//已经分层，复制combine对象即可
        {
            combineP = shelvesObj.transform.Find(CombineName).gameObject;
            ClearOtherCombineObj(combineP);
        }
        else
        {
            //在父节点下新建combine，并将父节点下的所有物体对象置于combine节点下
            combineP = new GameObject(CombineName);
            combineP.transform.SetParent(shelvesObj.transform);
            //本地坐标
            combineP.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            for (int i = 0, t = shelvesObj.transform.childCount; i < t; i++)
            {
                shelvesObj.transform.GetChild(0).SetParent(combineP.transform);
            }
           
        }

        //复制组合
        for (int i = 1; i < le; i++)
        {
            //世界坐标
            Instantiate(combineP, combineP.transform.position+new Vector3(0,moveHeight*i,0), combineP.transform.rotation, shelvesObj.transform);
        }
        //box重新添加
        BoxCollider newBox = shelvesObj.GetComponent<BoxCollider>();
        newBox.size = new Vector3(boxSize.x, boxSize.y * le, boxSize.z);
        newBox.center = new Vector3(boxCenter.x, boxCenter.y + (le - 1) * (moveHeight / 2), boxCenter.z);

        level = le;
        //模型变更后刷新高亮效果
        shelvesObj.GetComponent<DragItem>().ReInitObj();
        hasLevel = true;
    }




    /// <summary>
    /// 清空父节点下所有combine的克隆(保留combine)
    /// </summary>
    void ClearOtherCombineObj(GameObject combine = null)
    {
        if (!hasLevel) return;//未分级不存在combine

        if (combine == null)
            combine = shelvesObj.transform.Find(CombineName).gameObject;
        //删除多余的combine//延时删除
        //for(int i = 0,t = shelvesObj.transform.childCount; i <t; i++)
        //{
        //    if (shelvesObj.transform.GetChild(i) != combine)
        //    {
        //        Destroy(shelvesObj.transform.GetChild(i).gameObject);
        //    }
        //}
        //立即删除
        for (int i = 0; i < shelvesObj.transform.childCount; i++)
        {
            if (shelvesObj.transform.GetChild(i) != combine.transform)
            {
                DestroyImmediate(shelvesObj.transform.GetChild(i).gameObject);
                i--;
            }
        }
    }






}
