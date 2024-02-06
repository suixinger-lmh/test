using LitJson;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredItemBase : MonoBehaviour
{
    public enum StoredItemType
    {
        Null,
        Factory,
        Area,
        Shelves,
        Goods,
    }

    public StoredItemType _type = StoredItemType.Null;

    public bool canUse = false;

    public string ID;
    public virtual void BindComponent<T>(T data)
    {

    }

    public virtual string GetName()
    {
        return gameObject.name;
    }

    public virtual string GetID()
    {
        return ID;
    }
    public virtual void SetID(string id)
    {
        ID = id;
    }
    public virtual void SetName(string name)
    {
        gameObject.name = name;
    }


    public virtual float GetRelativeHeightValue()
    {
        return 0;
    }




    public string _saveData;


    public void SetData<T>(T data)
    {
        _saveData = JsonMapper.ToJson(data);
    }



    public string GetData()
    {
        return _saveData;
    }


    public T GetData<T>()
    {
        return JsonMapper.ToObject<T>(_saveData);
    }


    public string GetPositionStr()
    {
        //string.Format("{0}/{1}/{2}", transform.position.x, transform.position.y, transform.position.z);
        return CommonHelper.Vector3toString(transform.position);
    }

}
