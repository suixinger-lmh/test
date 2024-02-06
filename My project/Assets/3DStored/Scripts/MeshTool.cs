using UnityEngine;

public class MeshTool
{
    public static Bounds SpownCollider(Transform target)
    {
        Vector3 pMax = Vector3.zero;
        Vector3 pMin = Vector3.zero;
        Vector3 center = Vector3.zero;

        //移到原点，避免在生成collider时，自身影响参数
        Vector3 oldPos = target.transform.position;
        Quaternion oldQua = target.transform.rotation;
        target.transform.position = Vector3.zero;
        target.transform.rotation = Quaternion.identity;
        //

        Bounds bounds = ClacBounds(target, ref pMax, ref pMin, ref center);

        BoxCollider collider = target.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = target.gameObject.AddComponent<BoxCollider>();
        }
        collider.center = bounds.center;
        collider.size = bounds.size;

        target.transform.position = oldPos;
        target.transform.rotation = oldQua;

        return bounds;
    }

    public static Bounds OnlyGetBounds(Transform target)
    {
        Vector3 pMax = Vector3.zero;
        Vector3 pMin = Vector3.zero;
        Vector3 center = Vector3.zero;

        //设置box参数不附带物体信息，移动到原点
        //Vector3 oldPos = target.transform.position;
        //Quaternion oldQua = target.transform.rotation;

        //target.transform.position = Vector3.zero;
        //target.transform.rotation = Quaternion.identity;

        return ClacBounds(target, ref pMax, ref pMin, ref center);
    }

    /// <summary>
    /// 计算目标包围盒
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static Bounds ClacBounds(Transform obj, ref Vector3 pMax, ref Vector3 pMin, ref Vector3 center)
    {
        Renderer mesh = obj.GetComponent<Renderer>();

        if (mesh != null)
        {
            Bounds b = mesh.bounds;
            pMax = b.max;
            pMin = b.min;
            center = b.center;
        }


        RecursionClacBounds(obj.transform, ref pMax, ref pMin);

        ClacCenter(pMax, pMin, out center, ref pMax, ref pMin);

        Vector3 size = new Vector3(pMax.x - pMin.x, pMax.y - pMin.y, pMax.z - pMin.z);
        Bounds bound = new Bounds(center, size);
        bound.size = size;

//#if UNITY_EDITOR
//        Debug.Log("size>" + size);
//#endif
        bound.extents = size / 2f;

        return bound;
    }
    /// <summary>
    /// 计算包围盒中心坐标
    /// </summary>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <param name="center"></param>
    private static void ClacCenter(Vector3 max, Vector3 min, out Vector3 center, ref Vector3 pMax, ref Vector3 pMin)
    {
        float xc = (pMax.x + pMin.x) / 2f;
        float yc = (pMax.y + pMin.y) / 2f;
        float zc = (pMax.z + pMin.z) / 2f;

        center = new Vector3(xc, yc, zc);
//#if UNITY_EDITOR
//        Debug.Log("center>" + center);
//#endif
    }
    /// <summary>
    /// 计算包围盒顶点
    /// </summary>
    /// <param name="obj"></param>
    private static void RecursionClacBounds(Transform obj, ref Vector3 pMax, ref Vector3 pMin)
    {
        if (obj.transform.childCount <= 0)
        {
            return;
        }

        foreach (Transform item in obj)
        {
            Renderer m = item.GetComponent<Renderer>();



            if (m != null)
            {
                Bounds b = m.bounds;
                if (pMax.Equals(Vector3.zero) && pMin.Equals(Vector3.zero))
                {
                    pMax = b.max;
                    pMin = b.min;
                }

                if (b.max.x > pMax.x)
                {
                    pMax.x = b.max.x;
                }

                if (b.max.y > pMax.y)
                {
                    pMax.y = b.max.y;
                }
                if (b.max.z > pMax.z)
                {
                    pMax.z = b.max.z;
                }
                if (b.min.x < pMin.x)
                {
                    pMin.x = b.min.x;
                }

                if (b.min.y < pMin.y)
                {
                    pMin.y = b.min.y;
                }
                if (b.min.z < pMin.z)
                {
                    pMin.z = b.min.z;
                }
            }
            RecursionClacBounds(item, ref pMax, ref pMin);
        }
    }
}