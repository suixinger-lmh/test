using Custom.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredTreeViewItem : TreeViewItem
{
    public string ID;
    public StoredTreeViewItem(string itemName,string id, Sprite itemIcon = null) : base(itemName, itemIcon)
    {
        ID = id;
    }
}
