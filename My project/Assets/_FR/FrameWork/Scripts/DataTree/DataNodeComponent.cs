using Stored3D;
using UnityEngine;

namespace FrameWork
{
    public sealed class DataNodeComponent : MonoBehaviour, IBaseComponent
    {
        private const string RootName = "<Root>";
        private DataNode m_Root;


        public void InitComponent()
        {
            m_Root = new DataNode(RootName, null);
           
        }
        //public override void Init()
        //{
        //    base.Init();
          

        //    FrameEntrance.Instance.DataNode.SetData<VarString>("Model", "练习");
        //}

        /// <summary>
        /// 获取根结点。
        /// </summary>
        public DataNode Root
        {
            get
            {
                return m_Root;
            }
        }

        /// <summary>
        /// 关闭并清理数据结点管理器。 
        /// </summary>
        public void Shutdown()
        {
            Clear();
            m_Root = null;
        }

        /// <summary>
        /// 根据类型获取数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要获取的数据类型。</typeparam>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <returns>指定类型的数据。</returns>
        public T GetData<T>(string path) where T : Variable
        {
            return GetData<T>(path, null);
        }

        /// <summary>
        /// 获取数据结点的数据。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <returns>数据结点的数据。</returns>
        public Variable GetData(string path)
        {
            return GetData(path, null);
        }

        /// <summary>
        /// 根据类型获取数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要获取的数据类型。</typeparam>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="node">查找起始结点。</param>
        /// <returns>指定类型的数据。</returns>
        public T GetData<T>(string path, DataNode node) where T : Variable
        {
            DataNode current = GetNode(path, node);
            if (current == null)
            {
                string errorString = string.Format("节点出界，路径：“{0}”，“{1}”.", path, (node != null ? node.FullName : string.Empty));
                Debug.LogError(errorString);
                return null;
            }

            return current.GetData<T>();
        }

        /// <summary>
        /// 获取数据结点的数据。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="node">查找起始结点。</param>
        /// <returns>数据结点的数据。</returns>
        public Variable GetData(string path, DataNode node)
        {
            DataNode current = GetNode(path, node);
            if (current == null)
            {
                string errorString = string.Format("节点出界，路径：“{0}”，“{1}。”", path, (node != null ? node.FullName : string.Empty));
                Debug.LogError(errorString);
                return null;
            }

            return current.GetData();
        }

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要设置的数据类型。</typeparam>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="data">要设置的数据。</param>
        public void SetData<T>(string path, T data) where T : Variable
        {
            SetData(path, data, null);
        }

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="data">要设置的数据。</param>
        public void SetData(string path, Variable data)
        {
            SetData(path, data, null);
        }

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <typeparam name="T">要设置的数据类型。</typeparam>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="data">要设置的数据。</param>
        /// <param name="node">查找起始结点。</param>
        public void SetData<T>(string path, T data, DataNode node) where T : Variable
        {
            DataNode current = GetOrAddNode(path, node);
            current.SetData(data);
        }

        /// <summary>
        /// 设置数据结点的数据。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="data">要设置的数据。</param>
        /// <param name="node">查找起始结点。</param>
        public void SetData(string path, Variable data, DataNode node)
        {
            DataNode current = GetOrAddNode(path, node);
            current.SetData(data);
        }

        /// <summary>
        /// 获取数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <returns>指定位置的数据结点，如果没有找到，则返回空。</returns>
        public DataNode GetNode(string path)
        {
            return GetNode(path, null);
        }

        /// <summary>
        /// 获取数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="node">查找起始结点。</param>
        /// <returns>指定位置的数据结点，如果没有找到，则返回空。</returns>
        public DataNode GetNode(string path, DataNode node)
        {
            DataNode current = (node ?? m_Root);
            string[] splitPath = GetSplitPath(path);
            foreach (string i in splitPath)
            {
                current = current.GetChild(i);
                if (current == null)
                {
                    return null;
                }
            }

            return current;
        }

        /// <summary>
        /// 获取或增加数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <returns>指定位置的数据结点，如果没有找到，则创建相应的数据结点。</returns>
        public DataNode GetOrAddNode(string path)
        {
            return GetOrAddNode(path, null);
        }

        /// <summary>
        /// 获取或增加数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="node">查找起始结点。</param>
        /// <returns>指定位置的数据结点，如果没有找到，则增加相应的数据结点。</returns>
        public DataNode GetOrAddNode(string path, DataNode node)
        {
            DataNode current = (node ?? m_Root);
            string[] splitPath = GetSplitPath(path);
            foreach (string i in splitPath)
            {
                current = current.GetOrAddChild(i);
            }

            return current;
        }

        /// <summary>
        /// 移除数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        public void RemoveNode(string path)
        {
            RemoveNode(path, null);
        }

        /// <summary>
        /// 移除数据结点。
        /// </summary>
        /// <param name="path">相对于 node 的查找路径。</param>
        /// <param name="node">查找起始结点。</param>
        public void RemoveNode(string path, DataNode node)
        {
            DataNode current = (node ?? m_Root);
            DataNode parent = current.Parent;
            string[] splitPath = GetSplitPath(path);
            foreach (string i in splitPath)
            {
                parent = current;
                current = current.GetChild(i);
                if (current == null)
                {
                    return;
                }
            }

            if (parent != null)
            {
                parent.RemoveChild(current.Name);
            }
        }

        /// <summary>
        /// 移除所有数据结点。
        /// </summary>
        public void Clear()
        {
            m_Root.Clear();
        }

        /// <summary>
        /// 数据结点路径切分工具函数。
        /// </summary>
        /// <param name="path">要切分的数据结点路径。</param>
        /// <returns>切分后的字符串数组。</returns>
        private static string[] GetSplitPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new string[] { };
            }
            return path.Split(DataNode.PathSplit, System.StringSplitOptions.RemoveEmptyEntries);
        }


    }
}
