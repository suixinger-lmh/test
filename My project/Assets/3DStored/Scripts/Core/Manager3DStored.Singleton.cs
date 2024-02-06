using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stored3D
{
    public partial class Manager3DStored
    {
        private static Manager3DStored instance;

        public static Manager3DStored Instance
        {
            //get {
            //    if (instance == null) {
            //        GameObject frame = Resources.Load("MainProcess") as GameObject;
            //        Instantiate(frame);
            //        Debug.Log("�������");
            //    }



            //    return instance;
            //}


            get => instance;
            //set => instance = value;
        }

        void SetInstance()
        {
            if (instance != null)
            {
                Debug.LogError("�Ѿ����ڵ���ʵ�壡");

                return;
            }
            instance = this;
            DontDestroyOnLoad(this);
        }

    }
}

