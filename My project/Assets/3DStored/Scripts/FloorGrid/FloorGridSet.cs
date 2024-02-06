using UnityEngine;

namespace Stored3D
{
    public class FloorGridSet : MonoBehaviour, IBaseComponent
    {
        public GameObject _Floor;

        //TODO:��������޷�ʹ�ã���Ϊģ��û��ÿ����λһ�����㣬���Դ�������
        //floor�õ�plane�������ɣ�scale=1ʱΪ10��
        public float floor_x = 2500, floor_z=2500;//��λ(��)

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
            //_Floor.tag = "floor";//�༭��tag�޷�����

            float scaleX = floor_x / 10;
            float scaleZ = floor_z / 10;
            float scaleY = 1;
            _Floor.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);

            _GridMaterial = _Floor.GetComponent<MeshRenderer>().sharedMaterial;//ֱ�Ӹı�ԭshader����
            //����һ������Ϊһ����λ
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
