using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{

    //闲置状态：   linerender为空，点击确定 起始点
    //开始状态：   存在起始点 选择终结点  点击确定 终结点
    //结束状态：   存在绘制图像 不可再绘制(或者可修改点位)，可撤销，




    public Camera camera;
    public LineRenderer _line;

    [Range(0,2)]
    public float linewidth = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int num = 0;
    // Update is called once per frame
    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            //#region 正常画
            //if (Input.GetMouseButtonDown(1))
            //{
            //    _line.positionCount = 0;
            //    num = 0;
            //}

            //if (Input.GetMouseButton(0))
            //{
            //    num++;
            //    _line.positionCount = num;
            //    _line.SetPosition(num-1,hit.point);

            //}

            //#endregion


            #region 画方块

            if (Input.GetMouseButtonDown(1))
            {
                startVector = hit.point;
            }

            if (Input.GetMouseButton(1))
            {
                endVector = hit.point;

                //DrawCircle(startVector, endVector, 360);
                DrawRect(endVector,startVector);

            }

            if (Input.GetMouseButtonUp(1))
            {
                endVector = hit.point;
            }


            #endregion

            #region 画圆

            if (Input.GetMouseButtonDown(0))
            {
                startVector = hit.point;
            }

            if (Input.GetMouseButton(0))
            {
                endVector = hit.point;

                DrawCircle(startVector, endVector, 360);
                //DrawRect(endVector, startVector);

            }

            if (Input.GetMouseButtonUp(0))
            {
                endVector = hit.point;
            }


            #endregion


            #region 绘制线条

            //两条




            #endregion

        }
    }

    void DrawRect(Vector3 endP,Vector3 startP)
    {

        _line.positionCount = 5;
        _line.SetPosition(0, startP);
        _line.SetPosition(1, new Vector3(endP.x, startP.y, startP.z));
        _line.SetPosition(2, endP);
        _line.SetPosition(3, new Vector3(startP.x, startP.y, endP.z));
        _line.SetPosition(4, startP);
    
    }


    float angle2;
    //画圆 pointCount越高越圆
    void DrawCircle(Vector3 startP,Vector3 endP,int pointCount)
    {
        //起点和中点连线中心为圆心
        Vector3 center = startP + endP;
        center /= 2;

        float radius = Vector3.Distance(startP, endP) / 2;

        //以y值固定的平面上画圆

        //点量算出角度分辨率
        float angleScale = 360 / pointCount;



        //绘制
        _line.positionCount = pointCount;
        for(int i = 0; i < pointCount; i++)
        {
            //以0°在右侧计算

            float posx = radius * Mathf.Cos(angleScale * i * Mathf.Deg2Rad);
            float posz = radius * Mathf.Sin(angleScale * i * Mathf.Deg2Rad);

            _line.SetPosition(i, center+ new Vector3(posx, 0, posz));
        }

    }

    //画矩形参数
    Vector3 startVector;
    Vector3 endVector;
}
