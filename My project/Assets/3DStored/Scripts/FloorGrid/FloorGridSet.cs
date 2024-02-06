using UnityEngine;

namespace Stored3D
{
    public class FloorGridSet : MonoBehaviour, IBaseComponent
    {
        public GameObject _Floor;

        //TODO:顶点对齐无法使用，因为模型没有每个单位一个顶点，可以代码生成
        //floor用的plane网格生成，scale=1时为10米
        public float floor_x = 2500, floor_z=2500;//单位(米)

        public Material _GridMaterial;

        

        public void CreateFloorGrid()
        {
            if(_Floor == null)
            {
                GameObject  tempFloor = Resources.Load("Floor") as GameObject;
                _Floor = Instantiate(tempFloor,GameObject.Find(CommonHelper.Root).transform);
                _Floor.name = "Floor";
               
            }
          
            _Floor.transform.position = new Vector3(0, CommonHelper.GRIDHEIGHT, 0);
            //_Floor.tag = "floor";//编辑器tag无法保存

            float scaleX = floor_x / 10;
            float scaleZ = floor_z / 10;
            float scaleY = 1;
            _Floor.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);

            _GridMaterial = _Floor.GetComponent<MeshRenderer>().sharedMaterial;//直接改变原shader参数
            //缩放一个网格为一个单位
            _GridMaterial.SetTextureScale("_MainTex",new Vector2( floor_x / 2, floor_z / 2));
        }

        public void CreateFloorGrid(float x,float z)
        {
            floor_x = x;
            floor_z = z;
            CreateFloorGrid();
        }

        //private void Awake()
        //{
        //    CreateFloorGrid();
        //}








        public void InitComponent()
        {

        }
    }

}
