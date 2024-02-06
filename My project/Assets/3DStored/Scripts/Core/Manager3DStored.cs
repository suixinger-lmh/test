using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stored3D{

    public partial class Manager3DStored : MonoBehaviour
    {
        List<IBaseComponent> baseComponents;

        public Image maskImage;

        private void Awake()
        {
            //单例实体赋值
            SetInstance();

            //配置文件加载

            //数据文件加载(异步加载，未加载完成不进入场景)
            StartCoroutine(StartLoadRes());

            //框架初始化


            //组件获取&初始化
            GetAllComponent();


            //场景进入(异步等待，所有准备完成后进入)
            StartCoroutine(WaitEnterScene());
        }


        //获取物体下所有组件
        void GetAllComponent()
        {
            baseComponents = new List<IBaseComponent>(transform.GetComponentsInChildren<IBaseComponent>());
            //Debug.Log(baseComponents.Count);
            foreach (var x in baseComponents)
            {
                //Debug.Log(x.GetType());
                x.InitComponent();//组件功能初始化设置
            }
                
        }

        public T GetStoredComponent<T>()
        {
            return (T)baseComponents.Find(p => p.GetType() == typeof(T));
        }

        public void DoFadeIn(float time = 1)
        {
            if (maskImage != null)
            {
                maskImage.raycastTarget = true;
                maskImage.DOFade(1, time);
            }
        }

        public void DoFadeOut(float time = 1)
        {
            if (maskImage != null)
                maskImage.DOFade(0, time).OnComplete(() => {
                    maskImage.raycastTarget = false;
                });
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
}

