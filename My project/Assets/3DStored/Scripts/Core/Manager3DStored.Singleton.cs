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
            //        Debug.Log("加载完成");
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
                Debug.LogError("已经存在单例实体！");

                return;
            }
            instance = this;
            DontDestroyOnLoad(this);
        }

    }
}

