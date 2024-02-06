using System;
using System.Collections;

namespace UnityEngine
{
    public static class PathExtensions
    {
        public static string StreamingAssetsPath()
        {
            string path = null;
            //Debug.Log(Application.platform.ToString());
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    path =/* "file://" +*/ Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.OSXEditor:
                    path = "file://" + Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    path = "file://" + Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.OSXPlayer:
                    path = "file://" + Application.streamingAssetsPath;
                    break;
                case RuntimePlatform.WebGLPlayer:
                    //if (CustomStatic.m_InitConfig != null && !string.IsNullOrEmpty(CustomStatic.m_InitConfig.webRelativePath))
                    //{
                    //    path = CustomStatic.m_InitConfig.webRelativePath + "/StreamingAssets";
                    //}
                    //else
                    {
                        path = "StreamingAssets";
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw";
                    break;
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
            }
            return path;
        }
    }
}