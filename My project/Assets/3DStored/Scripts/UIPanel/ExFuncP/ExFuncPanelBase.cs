using UnityEngine;

public class ExFuncPanelBase : MonoBehaviour
{
    public class ExFuncParam
    {
        public GameObject _factory;//�ֿ����
        public GameObject _area;//�������
        public GameObject _shelves;//���ܶ���

        public Transform TempShelvesParent;//��ʱ���ɵĻ���λ��
        public Transform FacShelvesParent;//�ֿⱣ���µĻ���λ��
    }



    protected virtual void Start()
    {
        //�����ʼ����ע��ִ��ʱ��

    }

    public virtual void InitExFunc(ExFuncParam param)
    {
        gameObject.SetActive(true);
    }

    public virtual void ExitExPanel()
    {
        gameObject.SetActive(false);
    }

}
