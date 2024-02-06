using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LookForwardCamera : MonoBehaviour
{
    public Transform labelTarget;
    public Transform people;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //朝向车
        labelTarget.forward =  labelTarget.position - people.position;


    }

    //public Vector3 pos = new Vector3(-41.31f, 3.32f, 118.81f);
    //public Vector3 size = new Vector3(0.002768f, 0.002768f, 0.002768f);
    //private void OnTriggerEnter(Collider other)
    //{
    //    //触发，面板从商店移动到路边，打开
    //    labelTarget.DOMove(pos,2f);
    //    labelTarget.DOScale(size,2f);
    //}

}
