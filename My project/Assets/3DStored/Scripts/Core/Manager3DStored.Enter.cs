using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stored3D
{
    public partial class Manager3DStored
    {
        public string sceneName;


        //等待框架所有进度完成后，进入场景
        IEnumerator WaitEnterScene()
        {
            while (waitDataLoad)
            {
                yield return null;
            }
            //进入场景
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }



    }

}
