using FrameWork;
using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPanel : RightUIPanel
{
    public Text TypeText;
    public Text NameText;

    public InputField nameInput;
    public Button changeNameBtn;

    public Button backBtn;

    protected override void Start()
    {
        base.Start();

        if (Manager3DStored.Instance != null)
        {
            Manager3DStored.Instance.GetStoredComponent<EventComponent>().AddEventListener(CoreEventId.EditorPanel, OpenEditorPanel);
        }
        changeNameBtn.onClick.AddListener(() =>
        {
            nameInput.text = NameText.text;
            nameInput.gameObject.SetActive(true);
            NameText.gameObject.SetActive(false);

        });

        nameInput.onEndEdit.AddListener((str) => {
            nowStoredItem.SetName(str);
            NameText.text = str;
            NameText.gameObject.SetActive(true);
            nameInput.gameObject.SetActive(false);
            nameInput.text = string.Empty;


            Manager3DStored.Instance.GetStoredComponent<EventComponent>().DispatchCoreEvent(new CoreEvent( CoreEventId.RefreshTreeData, str));
        });

        backBtn.onClick.AddListener(CloseUIPanel);

    }

    StoredItemBase nowStoredItem;
    void OpenEditorPanel(CoreEvent ce)
    {
        nowStoredItem = (StoredItemBase)ce.EventParam;

        if(nowStoredItem == null)
        {
            CloseUIPanel();
            return;
        }


        ShowInf(nowStoredItem);

        OpenUIPanel();
    }

    void ShowInf(StoredItemBase item)
    {
        switch (item._type)
        {
            case StoredItemBase.StoredItemType.Factory:
                TypeText.text = "仓库";
                break;

            case StoredItemBase.StoredItemType.Area:
                TypeText.text = "库位";
                break;

            case StoredItemBase.StoredItemType.Shelves:
                TypeText.text = "货架";
                break;
            case StoredItemBase.StoredItemType.Goods:
                TypeText.text = "货物";
                break;
        }

        NameText.text = item.GetName();

    }


}
