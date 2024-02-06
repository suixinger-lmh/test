using UnityEngine;

public class ExFuncPanelBase : MonoBehaviour
{
    public class ExFuncParam
    {
        public GameObject _factory;//仓库对象
        public GameObject _area;//区域对象
        public GameObject _shelves;//货架对象

        public Transform TempShelvesParent;//临时生成的货架位置
        public Transform FacShelvesParent;//仓库保存下的货架位置
    }



    protected virtual void Start()
    {
        //自身初始化，注意执行时机

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
