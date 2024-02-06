using Stored3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSortPanel : ExFuncPanelBase
{
    [Header("�ֲ�")]
    public InputField input_level;
    public Button btn_levelCreate;//�ֲ�
    public Button btn_backSet;//�ֲ㻹ԭ
    public Button btn_split;//��ÿһ�㶼��������

    GameObject shelves;
    ShelvesLevel levelComponent;


    protected override void Start()
    {
        base.Start();

        if (input_level == null)
            input_level = GetComponentInChildren<InputField>();

        //�ֲ�
        if (btn_levelCreate == null)
            btn_levelCreate = transform.Find("create").GetComponent<Button>();
        btn_levelCreate.onClick.RemoveAllListeners();
        btn_levelCreate.onClick.AddListener(CreateLevel);
        //��ԭ
        if (btn_backSet == null)
            btn_backSet = transform.Find("back").GetComponent<Button>();
        btn_backSet.onClick.RemoveAllListeners();
        btn_backSet.onClick.AddListener(BackSet);
        //�зֶ���
        if (btn_split == null)
            btn_split = transform.Find("split").GetComponent<Button>();
        btn_split.onClick.RemoveAllListeners();
        btn_split.onClick.AddListener(CreateLevel);
    }


    public override void InitExFunc(ExFuncParam param)
    {
        //��ȡ��������
        shelves = param._shelves;
        //��������ӷֲ�ű�
        levelComponent = shelves.GetComponent<ShelvesLevel>();
        if (levelComponent == null)
        {
            //��һ�μ�¼ԭʼ��Ϣ
            levelComponent = shelves.AddComponent<ShelvesLevel>();
            levelComponent.SaveOrigin(shelves);
        }

        input_level.text = levelComponent.level.ToString();

        base.InitExFunc(param);
    }


    public override void ExitExPanel()
    {
        input_level.text = string.Empty;
        shelves = null;
        levelComponent = null;

        base.ExitExPanel();
    }



    void RefreshInputText()
    {
        input_level.text = levelComponent.level.ToString();
    }






    void CreateLevel()
    {
        int count;
        if(int.TryParse(input_level.text, out count))
        {
            if(count>0)
                levelComponent.SetLevel_new(count);
        }
        RefreshInputText();
    }
    void BackSet()
    {
        levelComponent.BackState();
        //levelComponent.BackBoxInf();
        RefreshInputText();
    }
    void SplitLevel()//��ÿ�����
    {

    }
}
