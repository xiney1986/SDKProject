using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 卡片仓库
 * */
public class CardStoreWindow : WindowBase
{
	public TapContentBase tapBase;//分页按钮
	public UILabel numbers;
	public ContentCardChoose cardContent;
	public DelegatedynamicContent scrapContent;
	public GameObject scrapPrefab;
	public SortCondition sc;
	public ButtonBase siftButton;
	ArrayList list = null;
	List<Exchange> scrapList = null;
	int storageVersion = -1;
    private bool intoFromGuide = false;

    public void setIntoTyoe(bool bo)
    {
        intoFromGuide = bo;
    }
	public void updateCardContent ()
	{
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_CARDSTORE_WINDOW);
		list = SortManagerment.Instance.cardSort (StorageManagerment.Instance.getAllRole (), sc, CardStateType.STATE_USING);
		if (list!=null&&list.Count>0&&intoFromGuide&&IncreaseManagerment.Instance.geCommendCardEvolutionList().Count>0)
	    {
			intoFromGuide=false;
	        Card tempList = IncreaseManagerment.Instance.geCommendCardEvolutionList()[0];
            ArrayList reList = new ArrayList();
            reList.Add(tempList);
	        for (int i=0;i<list.Count;i++)
	        {
	            if ((list[i] as Card).uid != tempList.uid) reList.Add(list[i]);
	        }
	        list = reList;
	    }
		storageVersion = StorageManagerment.Instance.RoleStorageVersion;
		cardContent.reLoad (list);
		numbers.text = list.Count + "/" + StorageManagerment.Instance.getRoleStorageMaxSpace ();
        //if (GuideManager.Instance.doingEvoLution)
        //{
        //    GuideManager.Instance.doFriendlyGuideEvent();
        //}
	}

	public void updateScrapContent ()
	{
		scrapContent.cleanAll ();
		scrapList = CardScrapManagerment.Instance.getCardScrapList();
		scrapContent.reLoad(scrapList.Count);
		if (GuideManager.Instance.isEqualStep (135004000)) {
			GuideManager.Instance.doGuide ();
			StartCoroutine (Utils.DelayRun (()=>{
				GuideManager.Instance.guideEvent ();
			},0.5f));
		}
	}

	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item== null){
			item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
		}
		
		CardScrapView button = item.GetComponent<CardScrapView> ();
		button.fawin =this;
		button.init(scrapList[i]);
		
		return item;
	}

	GameObject initItem (int i){
		GameObject	item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
		CardScrapView button = item.GetComponent<CardScrapView> ();
		button.fawin =this;
		button.init(scrapList[i]);
		return item;
	}
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		if (tapBase.getActiveIndex () == 1) {
			updateScrapContent ();
		} else {
			updateCardContent ();
		}

	}

	protected override void begin ()
	{ 
		base.begin ();
		scrapContent.SetUpdateItemCallback (onUpdateItem);
		scrapContent.SetinitCallback (initItem);
		if (!isAwakeformHide) {
			SortConditionManagerment.Instance.initDefaultSort (SiftWindowType.SIFT_CARDSTORE_WINDOW);
			tapBase.changeTapPage (tapBase.tapButtonList [0]);
		}
		if (StorageManagerment.Instance.RoleStorageVersion != storageVersion || !isAwakeformHide 
			|| cardContent.nodeList == null || SortManagerment.Instance.isCardStoreModifyed) {
			if (SortManagerment.Instance.isCardStoreModifyed)
				SortManagerment.Instance.isCardStoreModifyed = false;
			if (tapBase.getActiveIndex () == 0) {
				updateCardContent ();
			}
		}
		if (GuideManager.Instance.isEqualStep (135003000)) {
			GuideManager.Instance.doGuide ();
			GuideManager.Instance.guideEvent ();
		}
        StartCoroutine(Utils.DelayRun(() => {
            GuideManager.Instance.doFriendlyGuideEvent();
            MaskWindow.UnlockUI();
        }, 1f));
		
		
	}

	protected override void DoEnable ()
	{
		//base.DoEnable ();
		//看不到的时候先清掉容器好看些
		if (StorageManagerment.Instance.RoleStorageVersion != storageVersion) {
			cardContent.cleanAll ();
			list = null;
		}

	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			if (GuideManager.Instance.isEqualStep (135006000)) {
				GuideManager.Instance.doGuide ();
			}
			finishWindow ();
		} else if (gameObj.name == "buttonfilter") {
			UiManager.Instance.openWindow<SiftCardWindow> ((win) => {
				win.Initialize (null, SiftCardWindow.CARDSTORE, SiftWindowType.SIFT_CARDSTORE_WINDOW);
			});
		}
	}

	//页面按钮
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonCard" && enable == false) {
			cardContent.cleanAll ();
		} 
		else if (gameObj.name == "buttonScrap" && enable == false) {
			scrapContent.cleanAll ();
		} 
		else if (gameObj.name == "buttonCard" && enable == true) {
			numbers.gameObject.SetActive (true);
			siftButton.gameObject.SetActive (true);
			updateCardContent();
		}
		else if (gameObj.name == "buttonScrap" && enable == true) {
			numbers.gameObject.SetActive (false);
			siftButton.gameObject.SetActive (false);
			GuideManager.Instance.doFriendlyGuideEvent();
			updateScrapContent();
		}
	}

	//初始化信息
	public void Initialize(int index)
	{
		tapBase.changeTapPage(tapBase.tapButtonList[index]);
	}

	//置顶上阵角色
	void topTeamRole ()
	{
		for (int i=0; i<list.Count; i++) {
		
			if (ArmyManager.Instance.getTeamCardUidList ().Contains ((list [i] as Card).uid)) {
				object tmp = list [i];
				list.Remove (list [i]);
				list.Insert (0, tmp);
			}
		}
	}

	Card c = null;

	public void exchange(int sid,Card _c)
	{
		this.c = _c;
		ExchangeFPort exf = FPortManager.Instance.getFPort ("ExchangeFPort") as ExchangeFPort;
		exf.exchange (sid, 1, exchangeOK);
	}

	void exchangeOK(int sid,int num)
	{
		if (GuideManager.Instance.isEqualStep (135006000)) {
			GuideManager.Instance.guideEvent ();
		}
		ExchangeManagerment.Instance.addExchange (sid, num);
		if (c != null && HeroRoadManagerment.Instance.activeHeroRoadIfNeed (c)) {
			TextTipWindow.Show(LanguageConfigManager.Instance.getLanguage("s0418"));
			c = null;
		}
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("s0132_hecheng"));
		updateScrapContent();
		MaskWindow.UnlockUI();
	}
}
