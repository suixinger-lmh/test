using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//������������ unity��Instantiate��¡��ע���ܹ�����¡�����ԣ���Щ���ܹ��Զ���¡����
public class FullSetPanel : ExFuncPanelBase
{
    [Header("���")]
    //public Button btn_fullSet;

    public Button btn_saveCreateObjs;//���水ť
    public InputField spaceX;
    public InputField spaceY;

    public Button createX;
    public Button createY;


    //�����������һ�� x z


    //����������ɶ�� x z

    //������������� x z xz

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
        Debug.Log("��书��");

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





    //���
    void FullSet(BoxCollider areaBox, GameObject shelvesObj, float spaceX, float spaceZ)
    {
        //��ȡ����box���ĵ�
        //���ĵ�λ��+�������ƶ�λ�� = ���ĵ���������
        Vector3 centerPos = areaBox.bounds.center;
        //Vector3 centerPos = areaBox.center +areaBox.transform.position;
        //centerPos += areaBox.transform.parent.parent.position;

        //��ȡbox x�����ϵ������Сֵ
        float Xmin = centerPos.x - areaBox.bounds.size.x / 2;
        float Xmax = centerPos.x + areaBox.bounds.size.x / 2;

        //��ȡbox z�����ϵ������Сֵ
        float Zmin = centerPos.z - areaBox.bounds.size.z / 2;
        float Zmax = centerPos.z + areaBox.bounds.size.z / 2;

        //��ȡ���ܵ���������
        //shelvesObj.transform.position;

        //��ȡ���ܵ�box X,Z����ĳ���
        float Xsize = shelvesObj.GetComponent<BoxCollider>().bounds.size.x;
        float Zsize = shelvesObj.GetComponent<BoxCollider>().bounds.size.z;

        if (spaceX >= 0)
        {
            //X����������
            float xLength = Xmax - shelvesObj.transform.position.x - Xsize / 2;
            int xCount = Mathf.FloorToInt(xLength / (spaceX + Xsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position + new Vector3((Xsize + spaceX) * (i + 1), 0, 0));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
            //X����������
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
            //Z����������
            float xLength = Zmax - shelvesObj.transform.position.z - Zsize / 2;
            int xCount = Mathf.FloorToInt(xLength / (spaceZ + Zsize));
            for (int i = 0; i < xCount; i++)
            {
                CreateObjIns(shelvesObj.transform.position + new Vector3(0, 0, (Zsize + spaceZ) * (i + 1)));
                //GameObject tempobj = Instantiate(shelvesObj, shelvesObj.transform.parent);
                //tempobj.transform.position = ;
                //createObjsTemp.Add(tempobj);
            }
            //z����������
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

        //������
        if (!hasOne)
        {
            StoredItemShelves sheItem = shelves.GetComponent<StoredItemShelves>();
            GameObject tempobj = sheItem.GetCloneObj();
            
            //λ��
            tempobj.transform.SetParent(tempCreateParent);
            tempobj.transform.position = insPos;

            //GameObject tempobj = Instantiate(shelves, tempCreateParent);
            ////tempobj.name = shelves.name;
            //tempobj.transform.position = insPos;
            ////��ʼ��
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
