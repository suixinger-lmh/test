using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stored3D;
using UnityEngine.Networking;
using System;
using UnityEngine.Events;

public class AssetBundleManager : MonoBehaviour,IBaseComponent
{
    public void InitComponent()
    {
        
    }

    AssetBundle _assetBundle;

    //public List<GameObject> assetsCache = new List<GameObject>();

    public Dictionary<string, GameObject> assetsCache = new Dictionary<string, GameObject>();


    public void GetObjInstance()
    {

    }

    //public void DoLoadOnce(string assetBundleName,string assetName,UnityAction<GameObject> ac)
    //{
    //    //已经在缓存中
    //    if (assetsCache.ContainsKey(assetBundleName + assetName))
    //    {
    //        ac(assetsCache[assetBundleName + assetName]);
    //        return;
    //    }
    //    //else
    //    //{
    //    //    assetsCache.Add(assetBundleName + assetName,null);
    //    //}

    //    //if (_assetBundle!=null && _assetBundle.name == assetBundleName)
    //    //    return;
        

    //    //Debug.Log(assetBundleName);

    //    string abPath = PathExtensions.StreamingAssetsPath() + "/AssetBundle/Factory/" + assetBundleName + ".unity3d";
    //    StartCoroutine(GetAssetBundle(abPath, assetName, (go) => {
    //        assetsCache.Add(assetBundleName + assetName, go);
    //        if (ac != null)
    //            ac(go);
    //    }));

    //}

    public void DoLoadOnce(string folder, string assetBundleName, string assetName, UnityAction<GameObject> ac)
    {
        StartCoroutine(DoLoadOnce_Sync(folder,assetBundleName,assetName,ac));
    }

    IEnumerator DoLoadOnce_Sync(string folder,string assetBundleName, string assetName, UnityAction<GameObject> ac)
    {
        string cacheName = assetBundleName + assetName;
        //已经在缓存中
        if (assetsCache.ContainsKey(cacheName))
        {
            //异步等待缓存物体加载完成【加载失败退出】
            //避免加载失败异常
            while (assetsCache.ContainsKey(cacheName) && assetsCache[cacheName] == null)
            {
                yield return null;
            }

            //加载完成
            if (assetsCache.ContainsKey(cacheName))
                ac(assetsCache[cacheName]);
        }
        else//初次加载
        {
            //缓存记录，空物体占位
            assetsCache.Add(cacheName, null);
            string abPath = PathExtensions.StreamingAssetsPath() + folder + assetBundleName + ".unity3d";
            StartCoroutine(GetAssetBundle(abPath, assetName, (go) =>
            {
                //加载失败,清除缓存
                if (go == null)
                    assetsCache.Remove(cacheName);
                else
                {
                    //加载成功，缓存物体刷新
                    assetsCache[cacheName] = go;
                    if (ac != null)
                        ac(go);
                }
          
            }));
        }
    }



    IEnumerator GetAssetBundle(string abPath,string assetName,UnityAction<GameObject> ac)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(abPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            ac(null);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            if (bundle != null)
            {
                GameObject assetObj = bundle.LoadAsset<GameObject>(assetName);
                //assetObj.name = abPath + assetName;
                if (ac != null)
                    ac(assetObj);

                bundle.Unload(false);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
