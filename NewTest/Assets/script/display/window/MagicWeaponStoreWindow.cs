using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicWeaponStoreWindow : WindowBase
{
	public DelegatedynamicContent scrapContent;//碎片容器
    public MagicWeaponContent contentMagicWeapon;//神器容器
	public UILabel storeSpceNum;
	public GameObject scrapPrefab;
	public SortCondition sc ;
	public TapContentBase tapBase;
	public ButtonBase siftButton;
    public GameObject magicWeaponDraw;
    public GameObject scrapDraw;
	ArrayList equipList;
	ArrayList propList;
    ArrayList magicList;//秘宝列表
    List<Exchange> magicScrapList=null;//秘宝碎片列表
	public Card chooseCard;
	List<Exchange> scrapList = null;
	int startIndex;
	int propStorageVersion = -1;
	int equipStorageVersion = -1;
    int magicWeaponVersion = -1;
    public PrizeSample ps;//组成奖励

	public GameObject scrapNum;
	public UILabel canGetWeaponCount;

	public override void OnAwake ()
	{
        UiManager.Instance.magicWeaponStoreWindow = this;
	}

	public void Initialize (int tap)
	{
		startIndex = tap;
	}

	protected override void begin ()
	{
		base.begin ();
		scrapList = MagicWeaponScrapManagerment.Instance.getMagicWeaponScrapList() ;
		if(scrapList.Count >= 0)
		{
			Debug.Log("....." + MagicWeaponScrapManagerment.Instance.canGetMagicWeaponCount(scrapList));
			updateCanGetMagicWeaponCount();
		}

		scrapContent.SetUpdateItemCallback (onUpdateItem);
		scrapContent.SetinitCallback (initItem);
		if (!isAwakeformHide) {
            magicWeaponVersion = StorageManagerment.Instance.magicWeaponVersion;
			//新开窗口用策划的推荐排序,可使用排最上,然后品质高在上
			//SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_PROPSTORE_WINDOW);
			//SortConditionManagerment.Instance.initDefaultSort(SiftWindowType.SIFT_EQUIPSTORE_WINDOW);
			tapBase.changeTapPage (tapBase.tapButtonList [startIndex]);
		}
		//筛选改变,装备仓库,道具仓库改变,刷新
		if (SortManagerment.Instance.isStoreModifyed ||StorageManagerment.Instance.magicWeaponVersion!=magicWeaponVersion) {
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
			 if (contentMagicWeapon.gameObject.activeSelf) {
                UiManager.Instance.openWindow<SiftMagicWeaponWindow>((win) => {
                    win.Initialize(SiftMagicWeaponWindow.STOREWINDOW, SiftWindowType.SIFT_MAGICWEAPON_WINDOW);
                });
            }
            else {
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
	}

    /// <summary>
    /// 更新秘宝
    /// </summary>
    void updateMagicWeaponContent() {
        magicWeaponDraw.SetActive(true);
        scrapDraw.SetActive(false);
        magicWeaponVersion = StorageManagerment.Instance.magicWeaponVersion;
        sc = SortConditionManagerment.Instance.getConditionsByKey(SiftWindowType.SIFT_MAGICWEAPON_WINDOW);
        //这里不排序了 无聊
        //测试先用这个 注释掉下面
       // magicList = MagicWeaponManagerment.Instance.getMagicWeaponList();
        magicList = SortManagerment.Instance.magicWeaponSort(StorageManagerment.Instance.getAllMagicWeapon(), sc);
        //magicList = SortManagerment.Instance.magicWeaponSort(magicList, new int[2] { SortType.SPLIT_EQUIP_NEW, SortType.SPLIT_FREE_STATE });
       // magicList = StorageManagerment.Instance.getAllMagicWeapon();//得到仓库里所有的秘宝
        contentMagicWeapon.cleanAll();//这里直接清理秘宝容器
        contentMagicWeapon.reLoad(magicList,MagicWeaponType.STRENG);
        storeSpceNum.text = magicList.Count + "/" + StorageManagerment.Instance.getMagicWeaponStorageMaxSpace();

    }

	public void updateContent ()
	{
       if (tapBase.getActiveIndex () == 1) {//更新装碎片
            updateScrapContent();
		} else if (tapBase.getActiveIndex() == 0) {//更新神器
            updateMagicWeaponContent();
        }
	}

	public void updateScrapContent ()
	{
        magicWeaponDraw.SetActive(false);
        scrapDraw.SetActive(true);
		scrapContent.cleanAll ();
        scrapList = MagicWeaponScrapManagerment.Instance.getMagicWeaponScrapList() ;
		scrapContent.reLoad(scrapList.Count);
	}

	GameObject onUpdateItem (GameObject item, int i)
	{
		if (item== null){
			item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
		}

        MagicWeaponScrapView button = item.GetComponent<MagicWeaponScrapView>();
		button.fawin = this;
		button.init(scrapList[i]);
		
		return item;
	}
    public void resolveBack() {
        UiManager.Instance.createPrizeMessageLintWindow(ps);
        ps = null;
        updateContent();
    }
	GameObject initItem ( int i)
	{
		GameObject	item= NGUITools.AddChild (scrapContent.gameObject,scrapPrefab);
        MagicWeaponScrapView button = item.GetComponent<MagicWeaponScrapView>();
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
        if (gameObj.name == "buttonMagicWeapon" && enable == false) {
            contentMagicWeapon.cleanAll();
        } 
		else if (gameObj.name == "buttonEquipScrap" && enable == true) {
			siftButton.gameObject.SetActive(false);
			storeSpceNum.gameObject.SetActive (false);
			finishSiftWindow ();
			updateScrapContent();
        } else if (gameObj.name == "buttonMagicWeapon" && enable==true) {
            siftButton.gameObject.SetActive(true);
            storeSpceNum.gameObject.SetActive(true);
            GuideManager.Instance.doGuide();
            if (contentMagicWeapon.nodeList == null || contentMagicWeapon.nodeList.Count < 1 || StorageManagerment.Instance.magicWeaponVersion != magicWeaponVersion) {
                updateMagicWeaponContent();//更新秘宝容器
            }

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

	public void updateCanGetMagicWeaponCount()
	{
		if(scrapList.Count >= 0)
		{
			if(MagicWeaponScrapManagerment.Instance.canGetMagicWeaponCount(scrapList) > 0)
			{
				scrapNum.SetActive(true);
				canGetWeaponCount.text = MagicWeaponScrapManagerment.Instance.canGetMagicWeaponCount(scrapList) + "";
			}
			else
			{
				scrapNum.SetActive(false);
			}
		}
	}
}
