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
        /// ��λ��ʾ�����߶�(linerender��λ�߶�)
        /// </summary>
        public static float AreaLineFloatHeight = 1f;
        public const float AreaLineFloatHeight_Drawing = 0.1f;//��λ���Ƹ߶�
        public static float GroundFLOORHEIGHT = 0;//����λ��

        public const float GRIDHEIGHT = -0.01f;//����λ�ã���0λ��ʱ����ģ�͸߶��غϣ���������棩

        public static string Root = "[3DStroed]";

        public static string Factory = "[Factory]";
        public static string Floor = "Floor";
        public static string TempArea = "[TempArea]";
        public static string TempShelves = "[TempShelves]";


        #region �����ϵͳ���

        //TODO:ģ�ʹ��ڶ��box����λ�ø߶Ȳ�ͬ
        public static bool IsSuitFactory(GameObject obj,Factory data)
        {
            //��������box��Ϊ�ֿ����
            if (obj.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("�ֿ�ģ��ת��ʧ�ܣ�ԭ����������岻���ڵ���BoxCollider��");
                Debug.LogError("ģ����Դ�쳣��Res:" + data.abRes.ABPath + "/" + data.abRes.AssetName);
                return false;
            }
            return true;
        }
        public static StoredItemFactory BindFactory_(GameObject fac, Factory dataSource)
        {
            //ģ��ת�ִ�����
            StoredItemFactory _factoryItem = fac.AddComponent<StoredItemFactory>();
            _factoryItem.BindComponent(dataSource);//�����
            return _factoryItem;
        }
        public static StoredItemArea BindArea_(GameObject area, Area data)
        {
            //ģ��ת�ִ�����
            StoredItemArea _areaItem = area.AddComponent<StoredItemArea>();
            _areaItem.BindComponent(data);//�����
            return _areaItem;
        }






        public static bool IsSuitShelves(GameObject obj, Shelves data)
        {
            switch (data.Usage)
            {
                case "����":
                    return IsSuitShelves_HJ(obj,data);
            }

            return true;
        }

        //������Դ����������
        static bool IsSuitShelves_HJ(GameObject obj, Shelves data)
        {
            //1.��������box��Ϊ������ײ
            if (obj.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("����ģ��ת��ʧ�ܣ�ԭ����������岻����BoxCollider��");
                return false;
            }
            //2.�ӽڵ��´���GD�ڵ㣬�ҽڵ����ݻ�box
            Transform gd = obj.transform.Find("GD");
            if (gd == null)
            {
                Debug.LogError("����ģ��ת��ʧ�ܣ�ԭ�򣺻�����������ȱ��GD�ڵ㣡");
                return false;
            }
            if (gd.GetComponent<BoxCollider>() == null)
            {
                Debug.LogError("����ģ��ת��ʧ�ܣ�ԭ��GD�ڵ����ݻ�BoxCollider��");
                return false;
            }
            return true;
        }
        public static StoredItemShelves BindShelves_(GameObject she, Shelves dataSource)
        {
            //ģ��ת�ִ�����
            StoredItemShelves _shelvesItem = she.AddComponent<StoredItemShelves>();
            _shelvesItem.BindComponent(dataSource);//�����
            return _shelvesItem;
        }



        #endregion



        #region Get

        /// <summary>
        /// ��ȡ�������òֿ�ĸ�����
        /// </summary>
        /// <returns></returns>
        public static Transform GetFactoryRoot()
        {
            return GameObject.Find(string.Format("{0}/[EditorScene]/{1}", Root, Factory)).transform;
        }

        /// <summary>
        /// ��ȡ�ֿ���ؿ�λ�ĸ�����
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
        /// ��ȡ������ػ��ܵĸ�����
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
        /// ��ȡ������ĺ��ʾ���
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetObjSuitDistance(GameObject obj)
        {
            //��������ߴ��ȡ���ʵľ��룬����+1��Ϊ�˸���С���������Ӿ���(TODO:��̬������ֵ)
            return GetObjBounds(obj).size.magnitude / 2 + 1;
        }


        #endregion


        /// <summary>
        /// ��centerPos��λ�������»���ָ����С���߿�
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x">x�ߴ�</param>
        /// <param name="y">z�ߴ�</param>
        /// <param name="centerPos">����λ��</param>
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
        /// ΪĬ�ϲֿ�(Ĭ�ϲֿ�Ϊƽ�棬�޸�scale��Collider�ĳߴ�)�ߴ��޸�
        /// </summary>
        /// <param name="defaultFac">Ĭ�ϲֿ����</param>
        /// <param name="facX"></param>
        /// <param name="facZ"></param>
        public static void CreateDefaultFactorySize(GameObject defaultFac, float facX, float facZ)
        {
            Transform floorObj = defaultFac.transform.GetChild(0);
            floorObj.localScale = new Vector3(facX * 0.1f, 1, facZ * 0.1f);
            defaultFac.GetComponent<BoxCollider>().size = new Vector3(facX, 0, facZ);
        }


        #region ����

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




        #region ��ײ�������ж�

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


        #region ��ʼ����ֹ�����

        //��ȡ���εĳ��� ������y�߶Ȳ��䡿
        /// <summary>
        /// ������ʼλ�ú���ֹλ�ã�������γ���(Ĭ��yֵһ��)
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
        /// ��ȡ�뾶
        /// </summary>
        /// <param name="startP"></param>
        /// <param name="endP"></param>
        /// <returns></returns>
        public static float DrawInfGet_CircleInf(Vector3 startP, Vector3 endP)
        {
            return Vector3.Distance(startP,endP)/2;

        }

        #endregion




        #region ����

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
        /// ��x��(t1,t2)����ӳ�䵽(s1,s2)���䡾ע�ⳬ������Ĳ��֡�
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


        #region ����߶�

        //���Σ�1��ģ�����λ��Ϊ��ʼ    2��ģ����ĳ������λ��Ϊ��ʼ  
        //��ȡ������͵������(��bounds��ȡ������б�����λ�û��͵�������Ҫ�޳�)

        //ע������ê�����Ӱ��//
        //�����������λ�����壬����bounds�����λ��(��������)    center.y - size.y/2
        //���ݵ�ǰλ��(��������)�����λ��(��������)  ������߶Ȳ�
        //�޸�������λ��(��������) = ԭλ��-�߶Ȳ�



        /// <summary>
        /// ��������������������λ�õĸ߶�ƫ����(����ê�������߶�Ӱ��)
        /// </summary>
        /// <param name="positon">���嵱ǰ��������</param>
        /// <param name="groundObj">�����ڵ�ǰ���������µĵ����������</param>
        /// <returns></returns>
        public static float GetObjSelfHeightValue(Vector3 positon, GameObject groundObj)
        {
            //����groundλ��
            float groundHeightWorld = GetObjGroundHeight(groundObj);
            //����λ�ø߶Ȳ�
            float moveSizeY = positon.y - groundHeightWorld;
            //������
            return moveSizeY;
        }
        /// <summary>
        /// ��������������������λ�õĸ߶�ƫ����(����ê�������߶�Ӱ��)
        /// </summary>
        /// <param name="positon">���嵱ǰ��������</param>
        /// <param name="groundBox">�����ڵ�ǰ���������µĵ����������</param>
        /// <returns></returns>
        public static float GetObjSelfHeightValue(Vector3 positon, BoxCollider groundBox)
        {
            //����groundλ��
            float groundHeightWorld = GetObjGroundHeight(groundBox);
            //����λ�ø߶Ȳ�
            float moveSizeY = positon.y - groundHeightWorld;
            //������
            return moveSizeY;
        }


        /// <summary>
        /// ����������ƶ���ָ���߶Ⱥ��������λ��(��������)
        /// </summary>
        /// <param name="position">���嵱ǰλ��(��������)</param>
        /// <param name="groundBox">����ĵ������</param>
        /// <param name="groundHeight">�µĸ߶�λ��(��������)</param>
        /// <returns>��λ��(��������)</returns>
        public static Vector3 GetObjPositionByGroundHeight(Vector3 position,GameObject groundObj,float groundHeight)
        {
            //��ȡ��ǰ��������ĸ߶�
            float groundObjHeightWorld = GetObjGroundHeight(groundObj);

            //�����ƶ���ָ���߶ȵ�ƫ��
            float groundMoveY = groundHeight - groundObjHeightWorld;

            //�¸߶�
            float newPositionY = position.y + groundMoveY;

            return new Vector3(position.x, newPositionY, position.z);
        }

        /// <summary>
        /// ����������ƶ���ָ���߶Ⱥ��������λ��(��������)
        /// </summary>
        /// <param name="position">���嵱ǰλ��(��������)</param>
        /// <param name="groundBox">����ĵ������</param>
        /// <param name="groundHeight">�µĸ߶�λ��(��������)</param>
        /// <returns>��λ��(��������)</returns>
        public static Vector3 GetObjPositionByGroundHeight(Vector3 position, BoxCollider groundBox, float groundHeight)
        {
            //��ȡ��ǰ�������������߶�
            float groundObjHeightWorld = GetObjGroundHeight(groundBox);

            //�����ƶ���ָ���߶ȵ�ƫ��
            float groundMoveY = groundHeight - groundObjHeightWorld;

            //�¸߶�
            float newPositionY = position.y + groundMoveY;

            return new Vector3(position.x, newPositionY, position.z);
        }

        /// <summary>
        /// ��ȡ�������߶�(Ĭ��Ϊ�ײ�λ��)
        /// </summary>
        /// <param name="groundObj">����</param>
        /// <param name="type">-1��box�ĵײ�λ�ã�0��box������λ�ã�1��box�Ķ���λ��</param>
        /// <returns></returns>
        public static float GetObjGroundHeight(GameObject groundObj,int type = -1)
        {
            //��ȡ��ǰ��������ĸ߶�
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
        /// ��ȡ�������߶�(Ĭ��Ϊ�ײ�λ��)
        /// </summary>
        /// <param name="groundBox">����collider</param>
        /// <param name="type">-1��box�ĵײ�λ�ã�0��box������λ�ã�1��box�Ķ���λ��</param>
        /// <returns></returns>
        public static float GetObjGroundHeight(BoxCollider groundBox, int type = -1)
        {
            //��ȡ��ǰ��������ĸ߶�
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



        //����ģ��mesh�ϵĵ�����͵㣻ģ����Ҫ�����ɶ�
        [Obsolete("��ʱ����bounds�ķ�ʽ��ȡ��͵�")]
        public static Vector3 FindLowPositionLoop(Transform obj)
        {
            Transform[] allTrans = obj.GetComponentsInChildren<Transform>();
            Vector3 min = FindLowPosition(allTrans[0]);
            //�ҳ�ÿ�������Сλ��
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
            Vector3 minVector = obj.TransformPoint(objVertor[0]);//ģ�͵�λ����������λ��
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

