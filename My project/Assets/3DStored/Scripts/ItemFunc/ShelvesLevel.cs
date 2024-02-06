using Stored3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: �費��Ҫ���зֳ������������룬�зֳ������� ���ܹ��ٷֲ�
public class ShelvesLevel : MonoBehaviour
{
    public static string CombineName = "combine";

    //��¼ԭ����
    public GameObject shelvesObj;

    public GameObject newShelves;

    public int level = 1;


    //����ԭʼbox��Ϣ
    public Vector3 boxCenter;
    public Vector3 boxSize;
    public float moveHeight;

    public bool hasLevel = false;//�Ƿ��Ѿ��ֲ�






    //��¼δ�ֲ�ʱ����Ϣ��ֻ�ڵ�һ��ִ��
    //public void SaveOrigin(GameObject obj)
    //{
    //    shelvesObj = obj;

    //    //��¼ԭbox��Ϣ
    //    BoxCollider objBox = shelvesObj.GetComponent<BoxCollider>();
    //    boxCenter = objBox.center;
    //    boxSize = objBox.size;
    //    moveHeight = objBox.bounds.size.y; //�߶�//�ѵ��߶Ȱ�����ײ��߶�����
    //}
    public void SaveOrigin(GameObject obj)
    {
        shelvesObj = obj;

        //��¼ԭbox��Ϣ
        BoxCollider objBox = shelvesObj.GetComponent<BoxCollider>();
        boxCenter = objBox.center;
        boxSize = objBox.size;
        moveHeight = objBox.bounds.size.y; //�߶�//�ѵ��߶Ȱ�����ײ��߶�����
    }


    //�������״̬
    public void BackState()
    {
        //�ӷֲ�״̬��ԭ
        if (hasLevel)
        {
            //�ҵ�combine
            GameObject combine = shelvesObj.transform.Find(CombineName).gameObject;
            if (combine != null)
            {
                ClearOtherCombineObj(combine);
                //��combine�µ����廹ԭ�����ڵ�
                for (int i = 0, t = combine.transform.childCount; i < t; i++)
                {
                    combine.transform.GetChild(0).SetParent(shelvesObj.transform);
                }
                //ɾ��combine����
                DestroyImmediate(combine);
                //box��ԭ
                shelvesObj.GetComponent<BoxCollider>().center = boxCenter;
                shelvesObj.GetComponent<BoxCollider>().size = boxSize;
                level = 1;
                //�ֲ���ȡ��
                hasLevel = false;
            }
        }
    }


    //public void BackBoxInf()
    //{
    //    //box��ԭ
    //    shelvesObj.GetComponent<BoxCollider>().size = boxSize;
    //    shelvesObj.GetComponent<BoxCollider>().center = boxCenter;
        

    //    level = 1;
    //}


    [Obsolete("ֱ�Ӹ��ƶ���ÿ�����������Ҫ�����,������")]
    public void SetLevel(int le)
    {
        //ɾ��ָ������

        if (newShelves != null)
        {
            DestroyImmediate(newShelves);
        }

        //�����µĸ�����
        GameObject newParent = new GameObject(shelvesObj.name);
        newParent.transform.SetParent(shelvesObj.transform.parent);
      
        //���帴�����
        for (int i = 0; i < le; i++)
        {
            Instantiate(shelvesObj, new Vector3(0, moveHeight * i, 0), Quaternion.Euler(0, 0, 0), newParent.transform);
        }

        //���︸����λ�����ƶ������box�Ϳ��Բ�����ƫ��
        newParent.transform.position = shelvesObj.transform.position;
        newParent.transform.rotation = shelvesObj.transform.rotation;

        //box�������
        BoxCollider newBox = newParent.AddComponent<BoxCollider>();
        newBox.size = new Vector3(boxSize.x, boxSize.y * le, boxSize.z);
        newBox.center = new Vector3(boxCenter.x, boxCenter.y + (le-1)*(moveHeight/2), boxCenter.z);
        //box�ܵ������������Ӱ�죬
        //newBox.center += shelvesObj.transform.position;

        //����������
        newShelves = newParent;
        
    }

    //�µĸ��Ʒ�ʽ��ֻ����ģ�����壬���������������ͬһ�׹������
    public void SetLevel_new(int le)
    {
        GameObject combineP;
        if (hasLevel)//�Ѿ��ֲ㣬����combine���󼴿�
        {
            combineP = shelvesObj.transform.Find(CombineName).gameObject;
            ClearOtherCombineObj(combineP);
        }
        else
        {
            //�ڸ��ڵ����½�combine���������ڵ��µ����������������combine�ڵ���
            combineP = new GameObject(CombineName);
            combineP.transform.SetParent(shelvesObj.transform);
            //��������
            combineP.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            for (int i = 0, t = shelvesObj.transform.childCount; i < t; i++)
            {
                shelvesObj.transform.GetChild(0).SetParent(combineP.transform);
            }
           
        }

        //�������
        for (int i = 1; i < le; i++)
        {
            //��������
            Instantiate(combineP, combineP.transform.position+new Vector3(0,moveHeight*i,0), combineP.transform.rotation, shelvesObj.transform);
        }
        //box�������
        BoxCollider newBox = shelvesObj.GetComponent<BoxCollider>();
        newBox.size = new Vector3(boxSize.x, boxSize.y * le, boxSize.z);
        newBox.center = new Vector3(boxCenter.x, boxCenter.y + (le - 1) * (moveHeight / 2), boxCenter.z);

        level = le;
        //ģ�ͱ����ˢ�¸���Ч��
        shelvesObj.GetComponent<DragItem>().ReInitObj();
        hasLevel = true;
    }




    /// <summary>
    /// ��ո��ڵ�������combine�Ŀ�¡(����combine)
    /// </summary>
    void ClearOtherCombineObj(GameObject combine = null)
    {
        if (!hasLevel) return;//δ�ּ�������combine

        if (combine == null)
            combine = shelvesObj.transform.Find(CombineName).gameObject;
        //ɾ�������combine//��ʱɾ��
        //for(int i = 0,t = shelvesObj.transform.childCount; i <t; i++)
        //{
        //    if (shelvesObj.transform.GetChild(i) != combine)
        //    {
        //        Destroy(shelvesObj.transform.GetChild(i).gameObject);
        //    }
        //}
        //����ɾ��
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
