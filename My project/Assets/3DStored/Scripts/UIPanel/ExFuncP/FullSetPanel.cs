using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//复制物体依赖 unity的Instantiate克隆，注意能够被克隆的属性，有些不能够自动克隆下来
public class FullSetPanel : ExFuncPanelBase
{
    [Header("填充")]
    //public Button btn_fullSet;

    public Button btn_saveCreateObjs;//保存按钮
    public InputField spaceX;
    public InputField spaceY;

    public Button createX;
    public Button createY;


    //间隔距离生成一个 x z


    //间隔距离生成多个 x z

    //间隔距离生成满 x z xz

    GameObject shelves;
    BoxCollider areaBox;
    Transform tempCreateParent;
    Transform facShelvesParent;

    List<GameObject> createObjsTemp = new List<GameObject>();

    public override void InitExFunc(ExFuncParam param)
    {
        createX.onClick.RemoveAllListeners();
        createX.onClick.AddListener(() => {
            float spx;
            if(float.TryParse(spaceX.text,out spx))
            {
                FullSet(areaBox,shelves,spx,-1);
            }
        });

        createY.onClick.RemoveAllListeners();
        createY.onClick.AddListener(() =>
        {
            float spx;
            if (float.TryParse(spaceY.text, out spx))
            {
                FullSet(areaBox, shelves, -1, spx);
            }
        });

        btn_saveCreateObjs.interactable = false;
        btn_saveCreateObjs.onClick.RemoveAllListeners();
        btn_saveCreateObjs.onClick.AddListener(() => {
            
            for(int i = 0; i < createObjsTemp.Count; i++)
            {
                createObjsTemp[i].transform.SetParent(facShelvesParent);
            }
            createObjsTemp.Clear();
        });


        createObjsTemp.Clear();
        Debug.Log("填充功能");

        areaBox = param._area.GetComponent<BoxCollider>();
        shelves = param._shelves;
        tempCreateParent = param.TempShelvesParent;
        facShelvesParent = param.FacShelvesParent;
      //  FullSet(param._area.GetComponent<BoxCollider>(), param._shelves, 0, 1);
        base.InitExFunc(param);
    }

    public override void ExitExPanel()
    {
        for(int i = 0; i < createObjsTemp.Count; i++)
        {
            Destroy(createObjsTemp[i]);
        }
        createObjsTemp.Clear();

        base.ExitExPanel();
    }



    private void Update()
    {
        if(createObjsTemp!=null && createObjsTemp.Count > 0)
        {
            btn_saveCreateObjs.interactable = true;
        }
    }





    //填充
    void FullSet(BoxCollider areaBox, GameObject shelvesObj, float spaceX, float spaceZ)
    {
        //获取区域box中心点
        //中心点位置+父物体移动位置 = 中心点世界坐标
        Vector3 centerPos = areaBox.bounds.center;
        //Vector3 centerPos = areaBox.center +areaBox.transform.position;
        //centerPos += areaBox.transform.parent.parent.position;

        //获取box x方向上的最大最小值
        float Xmin = centerPos.x - areaBox.bounds.size.x / 2;
        float Xmax = centerPos.x + areaBox.bounds.size.x / 2;

        //获取box z方向上的最大最小值
        float Zmin = centerPos.z - areaBox.bounds.size.z / 2;
        float Zmax = centerPos.z + areaBox.bounds.size.z / 2;

        //获取货架的世界坐标
        //shelvesObj.transform.position;

        //获取货架的box X,Z方向的长度
        float Xsize = shelvesObj.GetComponent<BoxCollider>().bounds.size.x;
        float Zsize = shelvesObj.GetComponent<BoxCollider>().bounds.size.z;

        if (spaceX >= 0)
        {
            //X正方向铺满
            float xLength = Xmax - shelvesObj.transform.position.x - Xsize / 2;
            int xCount = Mathf.FloorToInt(xLength / (spaceX + Xsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position + new Vector3((Xsize + spaceX) * (i + 1), 0, 0));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
            //X负方向铺满
            xLength = shelvesObj.transform.position.x - Xmin - Xsize / 2;
            xCount = Mathf.FloorToInt(xLength / (spaceX + Xsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position - new Vector3((Xsize + spaceX) * (i + 1), 0, 0));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
        }

        
        if(spaceZ >= 0)
        {
            //Z正方向铺满
            float xLength = Zmax - shelvesObj.transform.position.z - Zsize / 2;
            int xCount = Mathf.FloorToInt(xLength / (spaceZ + Zsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position + new Vector3(0, 0, (Zsize + spaceZ) * (i + 1)));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
            //z负方向铺满
            xLength = shelvesObj.transform.position.z - Zmin - Zsize / 2;
            xCount = Mathf.FloorToInt(xLength / (spaceZ + Zsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position - new Vector3(0, 0, (Zsize + spaceZ) * (i + 1)));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
        }
      

    }


    void CreateObjIns(Vector3 insPos)
    {
        bool hasOne = false;
        foreach(var x in createObjsTemp)
        {
            if(x.GetComponent<BoxCollider>().bounds.Contains(insPos))
            //if (x.transform.position == insPos)
                hasOne = true;
        }

        //不存在
        if (!hasOne)
        {
            StoredItemShelves sheItem = shelves.GetComponent<StoredItemShelves>();
            GameObject tempobj = sheItem.GetCloneObj();
            
            //位置
            tempobj.transform.SetParent(tempCreateParent);
            tempobj.transform.position = insPos;

            //GameObject tempobj = Instantiate(shelves, tempCreateParent);
            ////tempobj.name = shelves.name;
            //tempobj.transform.position = insPos;
            ////初始化
            //tempobj.GetComponent<DragItem>().InitDragFunc(shelves.GetComponent<DragItem>());

            createObjsTemp.Add(tempobj);
        }
      
    }





    void CreateObj(float targetValue, float nowValue, float objwidth, float space, GameObject obj)
    {
        float length;
        int count;
        if (targetValue > nowValue)
        {
            length = (targetValue - nowValue) - objwidth / 2;
            count = Mathf.FloorToInt(length / (space + objwidth));
        }
        else
        {
            length = (nowValue - targetValue) - objwidth / 2;
            count = Mathf.FloorToInt(length / (space + objwidth));
        }

        for (int i = 0; i < count; i++)
        {
            GameObject tempobj = Instantiate(obj, obj.transform.parent);
            tempobj.transform.position = obj.transform.position + new Vector3();
        }

    }

}
