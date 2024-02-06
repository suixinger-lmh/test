using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stored3D
{
    [System.Serializable]
    public class StoredABItem
    {
        public string LabelName;//��ʾ����
        public string ABPath;//ab������
        public string AssetName;//ab������Դ����
        public string ImagePath;//����ͼ��ַ
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




    //�ֿ�ab��Դ��ʽ��
    //1.��Ҫ����collider
    ////1.����һ��collider(���߼���ƶ�����)
    ////2.����һ�����(���ò���)
    [System.Serializable]
    public class Factory: ABItemBase
    {
       // public StoredABItem abRes = new StoredABItem();


        public string Scale;

    }
    [System.Serializable]
    public class Area
    {
        public string AreaShape;//������״
        public string Usage;//��λ��;�����ܣ�Ͳ�֣��ѷš�

        public void SetShape(DrawTool.DrawStyle style)
        {
            switch (style)
            {
                case DrawTool.DrawStyle.rect:
                    AreaShape = "����";
                    break;
                case DrawTool.DrawStyle.line:
                    AreaShape = "����ͼ��";
                    break;
                case DrawTool.DrawStyle.cicle:
                    AreaShape = "Բ��";
                    break;

            }
        }
        public void SetUsage(DrawTool.DrawStyle style)
        {
            switch (style)
            {
                case DrawTool.DrawStyle.rect:
                    Usage = "����";
                    break;
                case DrawTool.DrawStyle.line:
                    Usage = "����ͼ��";
                    break;
                case DrawTool.DrawStyle.cicle:
                    Usage = "Բ��";
                    break;

            }
        }
    }

    //����item��ʽ��
    //1.һ����������Ϊ���ڵ㣬���ڹ���������
    //2.boxcollider�����ڸ��ڵ��ϣ����²�Ҫ��collider����collider���ϵ����ڵ�һ��box�ϣ�

    //3.������Ҫ�� ����boxcollider  GD�ҵ�(����������cube���϶�������С��תΪGD�ҵ㣬������Ϣ�����ڹҵ���)
    [System.Serializable]
    public class Shelves : ABItemBase
    {
       // public StoredABItem abRes = new StoredABItem();

        public string Usage;//������;��������ܣ�Ͳ�� ����ͬ��;�в�ͬ���ܡ�



    }


    [System.Serializable]
    public class Goods : ABItemBase
    {
        //public StoredABItem abRes = new StoredABItem();
    }



}
