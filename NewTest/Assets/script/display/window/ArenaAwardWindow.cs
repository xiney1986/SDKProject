using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 public class ArenaAwardWindow : WindowBase {
    
    public const int TYPE_FINAL = 1; //决赛
    public const int TYPE_GUESS = 2; //竞猜
    public const int TYPE_INTEGRAL = 3; //积分

    public GameObject roleViewPrefab;
    public GameObject goodsViewPrefab;
	/** 积分,竞猜,决赛奖励用prefab */
    public GameObject itemPrefab;
    public DelegatedynamicContent[] contents;
	public GameObject[] dragObjs;
    public GameObject tapRoot;
    public GameObject[] tapButtons;
    public GameObject rewards;
    public UILabel lblRank;
    public ButtonBase buttonRewards;
    public UISprite[] contentBgs;
    public ArenaAwardNum numFinal;
    public ArenaAwardNum numGuess;
    public UILabel lblMyIntergal;

    int type;
    List<ArenaAwardInfo> list;

	protected override void begin()
    {
        if (isAwakeformHide) {
			MaskWindow.UnlockUI();
			return;
		}

        foreach (DelegatedynamicContent dc in contents)
        {
            dc.SetUpdateItemCallback(OnContentUpdateItem);
			dc.SetinitCallback(initItem);
        }

        loadData();
		MaskWindow.UnlockUI();
    }

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround("ChouJiang_BeiJing");
		//UiManager.Instance.backGroundWindow.switchToDark();
	}

    public void init(int type)
    {
        this.type = type;
        contents [0].gameObject.SetActive(type == 1);
        contents [1].gameObject.SetActive(type == 2);
        contents [2].gameObject.SetActive(type == 3);
        contentBgs [0].gameObject.SetActive(type == 1);
        contentBgs [1].gameObject.SetActive(type == 2);
        contentBgs [2].gameObject.SetActive(type == 3);
		dragObjs [0].gameObject.SetActive(type == 1);
		dragObjs [1].gameObject.SetActive(type == 2);
		dragObjs [2].gameObject.SetActive(type == 3);

        rewards.SetActive(type == TYPE_FINAL);
//        tapRoot.SetActive(type != TYPE_INTEGRAL);

		//决赛
        if (type == TYPE_FINAL)
        {
			setTitle(LanguageConfigManager.Instance.getLanguage("Arena26"));
            ArenaFinalWindow win = fatherWindow as ArenaFinalWindow;
            if (win != null)
            {
                lblRank.text = LanguageConfigManager.Instance.getLanguage("Arena45") + " : " + LanguageConfigManager.Instance.getLanguage("Arena14_" + win.getMyRank() + "_2");
            } else
            {
                lblRank.text = LanguageConfigManager.Instance.getLanguage("Arena47");
            }

            if (ArenaManager.instance.state != ArenaManager.STATE_RESET || win.getMyRank() == 0)
            {
                buttonRewards.disableButton(true);
            }
		} else if (type == TYPE_GUESS) {//竞猜
			setTitle(LanguageConfigManager.Instance.getLanguage("Arena53"));
		} else if (type == TYPE_INTEGRAL) {//竞猜
			setTitle(LanguageConfigManager.Instance.getLanguage("Arena27"));
		}
    }

    private DelegatedynamicContent Content
    {
        get
        {
            return contents[type - 1];
        }
    }

    void loadData()
    {
        if (type == TYPE_INTEGRAL)
            FPortManager.Instance.getFPort<ArenaGetAwardInfoIntegralFPort>().access(OnDataLoaded);
        else if (type == TYPE_GUESS)
            FPortManager.Instance.getFPort<ArenaGetAwardInfoGuessFPort>().access(OnDataLoaded);
        else
            FPortManager.Instance.getFPort<ArenaGetAwardInfoFinalFPort>().access(OnDataLoaded);
    }

    void OnDataLoaded(List<ArenaAwardInfo> list)
    {

        this.list = list;

		for (int i = 0; i < list.Count; i++) {
			if (list[i].received) {
				list.RemoveAt(i);
				i--;
			}
		}

        if (list != null && list.Count > 0)
        {
            Content.reLoad(list.Count);
        }

        if (type == TYPE_GUESS)
        {
            numGuess.OnDataLoaded(list);
        }
        if (type == TYPE_INTEGRAL)
        {
            lblMyIntergal.transform.parent.gameObject.SetActive(true);
            lblMyIntergal.text = LanguageConfigManager.Instance.getLanguage("Arena51") + ArenaManager.instance.getMyIntergal();
        }
    }

    void OnDataLoaded(List<ArenaAwardInfo> list,bool received)
    {
        if (received)
        {
            buttonRewards.disableButton(true);
        }
        numFinal.loadData();
        OnDataLoaded(list);
        buttonRewards.gameObject.SetActive(true);
    }

    GameObject OnContentUpdateItem(GameObject item,int index)
    {
        if (item == null)
        {
            item = NGUITools.AddChild(Content.gameObject,itemPrefab);
            item.SetActive(true);
        }

        ArenaAwardItem sc = item.GetComponent<ArenaAwardItem>();
        sc.window = this;
        sc.init(list[index]);
        return item;
    }
	GameObject initItem(int index)
	{

		GameObject	item = NGUITools.AddChild(Content.gameObject,itemPrefab);
			item.SetActive(true);

		
		ArenaAwardItem sc = item.GetComponent<ArenaAwardItem>();
		sc.window = this;
		sc.init(list[index]);
		return item;
	}
	/*
    public override void tapButtonEventBase(GameObject gameObj, bool enable)
    {
        if (!enable)
            return;

     //   Utils.DestoryChilds(Content.gameObject);

        if (gameObj.name == "buttonFinals")
        {
            type  = TYPE_FINAL;
        } else
        {
            type = TYPE_GUESS;
        }
        init(type);
        loadData();
    }
    */

    public override void buttonEventBase(GameObject gameObj)
    {
		base.buttonEventBase (gameObj);
        if (gameObj.name == "close")
        {
            finishWindow();
        } else if (gameObj.name == "buttonreaward")
        {
            FPortManager.Instance.getFPort<ArenaReceiveAwardFinalFPort>().access(OnReceiveBack);
        } 
    }

    private void OnReceiveBack(bool result)
    {
        TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage(result ? "Arena30" : "Arena31"));
        if (result)
        {
            numFinal.SetShow(false);
            buttonRewards.disableButton(true);
        }
    }


}
