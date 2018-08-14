using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class StoreWindow : WindowBase
{
	public ContentStoreEquip contentEquip;//装备容器
	public ContentStoreProp contentProp;//物品容器
	public DelegatedynamicContent scrapContent;//碎片容器
	public UILabel storeSpceNum;
	public GameObject scrapPrefab;
	public SortCondition sc ;
	public TapContentBase tapBase;
	public ButtonBase siftButton;
	ArrayList equipList;
	ArrayList propList;
    ArrayList magicList;//秘宝列表
	public Card chooseCard;
	List<Exchange> scrapList = null;
	int startIndex;
	int propStorageVersion = -1;
	int equipStorageVersion = -1;
    int magicWeaponVersion = -1;

	public override void OnAwake ()
	{
		UiManager.Instance.storeWindow = this;
	}

	public void Initialize (int tap)
	{
		startIndex = tap;
	}

	protected override void begin ()
	{
		base.begin ();
		scrapContent.SetUpdateItemCallback (onUpdateItem);
		scrapContent.SetinitCallback (initItem);
		if (isAwakeformHide == false) {
			equipStorageVersion = StorageManagerment.Instance.EquipStorageVersion;
			propStorageVersion = StorageManagerment.Instance.PropStorageVersion;
			//新开窗口用策划的推荐排序,可使用排最上,然后品质高在上
			SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_PROPSTORE_WINDOW);
			SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_EQUIPSTORE_WINDOW);
			tapBase.changeTapPage (tapBase.tapButtonList [startIndex]);
		}
		//筛选改变,装备仓库,道具仓库改变,刷新
		if (SortManagerment.Instance.isStoreModifyed || StorageManagerment.Instance.EquipStorageVersion != equipStorageVersion || StorageManagerment.Instance.PropStorageVersion != propStorageVersion) {
			SortManagerment.Instance.isStoreModifyed = false;
			updateContent ();
		}
		GuideManager.Instance.doFriendlyGuideEvent ();
		MaskWindow.UnlockUI ();
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close") {
			GuideManager.Instance.doGuide ();
			if (equipList != null) {
				foreach(Equip equip in equipList)
					equip.isNew = false;
			}
			finishWindow ();
		}
		if (gameObj.name == "buttonSift") {
			if (contentEquip.gameObject.activeSelf) {
				UiManager.Instance.openWindow<SiftEquipWindow> ((win) => {
					win.Initialize (SiftEquipWindow.STOREWINDOW, SiftWindowType.SIFT_EQUIPSTORE_WINDOW);});
			} else {
				UiManager.Instance.openWindow<SiftPropWindow> ((win) => {
					win.Initialize ();
				});
			}
		}
		 
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		updateContent();
        if (UiManager.Instance.getWindow<ShenGeGroupWindow>() != null)
            UiManager.Instance.getWindow<ShenGeGroupWindow>().finishWindow();
	}

	void updateEquipContent ()
	{
		equipStorageVersion = StorageManagerment.Instance.EquipStorageVersion;
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_EQUIPSTORE_WINDOW);
		equipList = SortManagerment.Instance.equipSort (StorageManagerment.Instance.getAllEquip (), sc);
		equipList = SortManagerment.Instance.equipSplit(equipList,new int[4]{SortType.SPLIT_EQUIP_NEW,SortType.SPLIT_FREE_STATE,SortType.SPLIT_USING_STATE,SortType.SPLIT_EATEN});
		contentEquip.cleanAll ();
	    if (GuideManager.Instance.getOnTypp() ==30)
	    {
	        ArrayList tempList=new ArrayList();
	        string tempUid = "";
	        for (int i=0;i<equipList.Count;i++)
	        {
	            Equip eq = equipList[i] as Equip;
	            if (eq != null && eq.isCanReine())
	            {
	                tempUid = eq.uid;
	                tempList.Add(eq);
	                break;
	            }

	        }
	        for (int i=0;i<equipList.Count;i++)
	        {
	            Equip eq = equipList[i] as Equip;
	            if (eq != null && eq.uid != tempUid) tempList.Add(eq);
	        }
	        equipList = tempList;
	    }
		contentEquip.reLoad (equipList, ContentEquipChoose .INTENSIFY, chooseCard);
	    if (GuideManager.Instance.getOnTypp() == 30)
	    {
            StartCoroutine(Utils.DelayRun(() => {
                GuideManager.Instance.doFriendlyGuideEvent();
             
            }, 0.5f));
            
	    }
		storeSpceNum.text = equipList.Count + "/" + StorageManagerment.Instance.getEquipStorageMaxSpace ();
	}
	void updatePropContent ()
	{
		propStorageVersion = StorageManagerment.Instance.PropStorageVersion;
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_PROPSTORE_WINDOW);
		propList = SortManagerment.Instance.propSort (StorageManagerment.Instance.getAllPropExcludeScrap (), sc);
//		topGift ();
		contentProp.cleanAll ();
		contentProp.reLoad (propList);
		storeSpceNum.text = propList.Count + "/" + StorageManagerment.Instance.getPropStorageMaxSpace ();
	}

	public void updateContent ()
	{
		if (tapBase.getActiveIndex () == 1) {
			updateEquipContent ();
		} else if (tapBase.getActiveIndex () == 2) {
			if (!isAwakeformHide) {
				updateScrapContent ();
			}
		} else {
			updatePropContent ();
		}
	}

	public void updateScrapContent ()
	{
		scrapContent.cleanAll ();
		scrapList = EquipScrapManagerment.Instance.getEquipScrapList();
		scrapContent.reLoad(scrapList.Count);
	}

	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item== null){
			item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
		}
		
		EquipScrapView button = item.GetComponent<EquipScrapView> ();
		button.fawin = this;
		button.init(scrapList[i]);
		
		return item;
	}
	GameObject initItem ( int i)
	{
		GameObject	item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
		EquipScrapView button = item.GetComponent<EquipScrapView> ();
		button.fawin = this;
		button.init(scrapList[i]);
		
		return item;
	}
	private void finishSiftWindow ()
	{
		if (fatherWindow as SiftEquipWindow) {
			if (tapBase.getActiveIndex () == 1) {
				fatherWindow.finishWindow ();
			}
		} else if (fatherWindow as SiftPropWindow) {
			if (tapBase.getActiveIndex () == 0) {
				fatherWindow.finishWindow ();
			}
		}
	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase(gameObj, enable);
		if (gameObj.name == "buttonProp" && enable == false) {
			contentProp.cleanAll ();
		} 
		else if (gameObj.name == "buttonEquip" && enable == false) {
			contentEquip.cleanAll ();
		} 
		else if (gameObj.name == "buttonEquipScrap" && enable == false) {
            //GuideManager.Instance.doFriendlyGuideEvent();
			scrapContent.cleanAll ();
		} 
		else if (gameObj.name == "buttonProp" && enable == true) { 
			siftButton.gameObject.SetActive(true);
			storeSpceNum.gameObject.SetActive (true);
			finishSiftWindow ();

			if (contentProp.nodeList == null || contentProp.nodeList.Count < 1 || StorageManagerment.Instance.PropStorageVersion != propStorageVersion) {
				updatePropContent ();
			} else {
				storeSpceNum.text = propList.Count + "/" + StorageManagerment.Instance.getPropStorageMaxSpace ();
			}
		}  
		else if (gameObj.name == "buttonEquip" && enable == true) {
			siftButton.gameObject.SetActive(true);
			storeSpceNum.gameObject.SetActive (true);
			finishSiftWindow ();
			GuideManager.Instance.doGuide (); 
			if (contentEquip .nodeList == null || contentEquip.nodeList.Count < 1 || StorageManagerment.Instance.EquipStorageVersion != equipStorageVersion) {
				updateEquipContent ();
			} else {
				storeSpceNum.text = equipList.Count + "/" + StorageManagerment.Instance.getEquipStorageMaxSpace ();
			}
		}
		else if (gameObj.name == "buttonEquipScrap" && enable == true) {
			siftButton.gameObject.SetActive(false);
			storeSpceNum.gameObject.SetActive (false);
            GuideManager.Instance.doFriendlyGuideEvent();
			finishSiftWindow ();
			updateScrapContent();
		}
	}
	//置顶礼包
	void topGift ()
	{
		for (int i=0; i < propList.Count; i++) {

			if ((propList [i] as Prop).getType () == PropType.PROP_TYPE_CHEST) {
				Prop tmp = (Prop)propList [i];
				propList.Remove (propList [i]);

				propList.Insert (0, tmp);
			}

		}
		
	}

	public void cleanSortCondition ()
	{
		SortCondition sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_PROPSTORE_WINDOW);
		sc.clearSortCondition ();
		sc = SortConditionManagerment.Instance.getConditionsByKey (SiftWindowType.SIFT_EQUIPCHOOSE_WINDOW);
		sc.clearSortCondition ();
	}

	public void exchange(int sid)
	{
		ExchangeFPort exf = FPortManager.Instance.getFPort ("ExchangeFPort") as ExchangeFPort;
		exf.exchange (sid, 1, exchangeOK);
	}
	
	void exchangeOK(int sid,int num)
	{
		ExchangeManagerment.Instance.addExchange (sid, num);
		UiManager.Instance.createMessageLintWindow(LanguageConfigManager.Instance.getLanguage ("s0132_hecheng"));
		updateScrapContent();
		MaskWindow.UnlockUI();
	}
}
