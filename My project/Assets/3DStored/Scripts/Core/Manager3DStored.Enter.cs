using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stored3D
{
    public partial class Manager3DStored
    {
        public string sceneName;


        //�ȴ�������н�����ɺ󣬽��볡��
        IEnumerator WaitEnterScene()
        {
            while (waitDataLoad)
            {
                yield return null;
            }
            //���볡��
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
        }



    }

}
