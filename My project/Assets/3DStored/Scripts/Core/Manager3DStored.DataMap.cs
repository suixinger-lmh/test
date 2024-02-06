using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

namespace Stored3D
{
    //数据管理部分
    public partial class Manager3DStored
    {
        public string _factoryFileName = "FactoryMap.json";
        public string _shelvesFileName = "ShelvesMap.json";
        public string _goodsFileName = "GoodsMap.json";
       // [HideInInspector]
        public List<Factory> _factories;
        public List<Shelves> _shelves;
        public List<Goods> _goods;

        //图片缩略图缓存资源
        public Dictionary<string, Texture> imageCache = new Dictionary<string, Texture>();

        /// <summary>
        /// 资源准备标记,标记完成后才能进入流程场景
        /// </summary>
        bool waitDataLoad = false;
        IEnumerator StartLoadRes()//异步顺序加载
        {
            waitDataLoad = true;
            yield return null;

            //仓库json加载和数据获取
            string filePath = PathExtensions.StreamingAssetsPath() + "/Json/" + _factoryFileName;
            yield return StartCoroutine(GetText(filePath, (jsonStr) => ReadJson2Obj<List<Factory>>(ref _factories, jsonStr)));

            filePath = PathExtensions.StreamingAssetsPath() + "/Json/" + _shelvesFileName;
            yield return StartCoroutine(GetText(filePath, (jsonStr) => ReadJson2Obj<List<Shelves>>(ref _shelves, jsonStr)));

            filePath = PathExtensions.StreamingAssetsPath() + "/Json/" + _goodsFileName;
            yield return StartCoroutine(GetText(filePath, (jsonStr) => ReadJson2Obj<List<Goods>>(ref _goods, jsonStr)));

            //整理图片
            foreach (var imageItem in _factories) 
            {
                if (!string.IsNullOrEmpty(imageItem.abRes.ImagePath) && !imageCache.ContainsKey(imageItem.abRes.ImagePath))
                    imageCache.Add(imageItem.abRes.ImagePath, null);
            }
            foreach (var imageItem in _shelves)
            {
                if (!string.IsNullOrEmpty(imageItem.abRes.ImagePath) && !imageCache.ContainsKey(imageItem.abRes.ImagePath))
                    imageCache.Add(imageItem.abRes.ImagePath, null);
            }
            foreach (var imageItem in _goods)
            {
                if (!string.IsNullOrEmpty(imageItem.abRes.ImagePath) && !imageCache.ContainsKey(imageItem.abRes.ImagePath))
                    imageCache.Add(imageItem.abRes.ImagePath, null);
            }

            List<string> keyList = new List<string>();
            keyList.AddRange(imageCache.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                string tempStr = keyList[i];
                string path = PathExtensions.StreamingAssetsPath() + "/" + tempStr;
                yield return StartCoroutine(GetTexture(path, (texture) =>
                {
                    imageCache[tempStr] = texture;
                }));
                Debug.Log("图片加载进度:"+i+"/"+keyList.Count);
            }

         
            //Debug.Log("加载结束。。");
            waitDataLoad = false;
        }



        T ReadJson2Obj<T>(ref T jsonList,string jsonText)
        {
            try
            {
                jsonList = JsonMapper.ToObject<T>(jsonText);
                //return jsonList;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            return jsonList;
        }



        IEnumerator GetText(string path, Action<string> at = null)
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                if (at != null)
                    at(www.downloadHandler.text);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }
        }
        
        IEnumerator GetTexture(string path, Action<Texture> at = null) 
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            www.downloadHandler = texDl;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Texture2D t = texDl.texture;
                //Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),Vector2.zero, 1f);
                if (at != null)
                    at(texDl.texture);

                // Or retrieve results as binary data
                //byte[] results = www.downloadHandler.data;
            }


        }
      

    }

}
