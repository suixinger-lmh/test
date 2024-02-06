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
            //����ʵ�帳ֵ
            SetInstance();

            //�����ļ�����

            //�����ļ�����(�첽���أ�δ������ɲ����볡��)
            StartCoroutine(StartLoadRes());

            //��ܳ�ʼ��


            //�����ȡ&��ʼ��
            GetAllComponent();


            //��������(�첽�ȴ�������׼����ɺ����)
            StartCoroutine(WaitEnterScene());
        }


        //��ȡ�������������
        void GetAllComponent()
        {
            baseComponents = new List<IBaseComponent>(transform.GetComponentsInChildren<IBaseComponent>());
            //Debug.Log(baseComponents.Count);
            foreach (var x in baseComponents)
            {
                //Debug.Log(x.GetType());
                x.InitComponent();//������ܳ�ʼ������
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

