using UnityEngine;
using System.Collections;

public class LastBattleDonationWindow : WindowBase
{
	public UITexture tex1;
	public UITexture tex2;
	public UIGrid donationsGrid;
	public Transform donationsContent;
	public GameObject donateItemTmp;// 捐献物资模板//
	public GameObject btnClose;
	public UILabel juGongValue;// 持有军功个数//
	private LastBattleDonationSample sample;
	private LastBattleDonateItem donateItem;
	public GameObject goodsInfoPanel;
	public ButtonBase okBtn;
	public ButtonBase cancelBtn;
	public UILabel haveCount;// 拥有goods的数量//
	public UILabel donateCount;// 捐献goods的数量//
	private GameObject[] itemObjs;

	public GoodsView goodsTmp;
	public Transform goodsPos;

	protected override void begin ()
	{
		base.begin ();
		initWin();

		MaskWindow.UnlockUI();
	}
	public override void DoDisable ()
	{
		base.DoDisable ();
		if(itemObjs != null)
		{
			for(int i=0;i<itemObjs.Length;i++)
			{
				GameObject.Destroy(itemObjs[i]);
			}
		}
	}
	public void initWin()
	{
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SHOP_CHUANGLIAN, tex1);
		ResourcesManager.Instance.LoadAssetBundleTexture (ResourcesManager.SHOP_CHUANGLIAN, tex2);
		setJuGongValue();
		initDonationItems();
	}

	public void initDonationItems()
	{
		itemObjs = null;
		if(LastBattleManagement.Instance.currentDonationList != null)
		{
			GameObject obj;
			LastBattleDonateItem item;
			itemObjs = new GameObject[LastBattleManagement.Instance.currentDonationList.Count];
			for(int i=0;i<LastBattleManagement.Instance.currentDonationList.Count;i++)
			{
				obj = GameObject.Instantiate(donateItemTmp) as GameObject;
				obj.transform.parent = donationsContent;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;
				item = obj.GetComponent<LastBattleDonateItem>();
				item.fartherWin = this;
				item.initItem(LastBattleManagement.Instance.currentDonationList[i]);
				obj.SetActive(true);
				itemObjs[i] = obj;
			}
			donationsGrid.repositionNow = true;
		}
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if(gameObj == btnClose)
		{
			finishWindow();
		}
		else if(gameObj == okBtn.gameObject)
		{
			clickOkBtn();
		}
		else if(gameObj == cancelBtn.gameObject)
		{
			closeGoodsInfoPanel();
			sample = null;
			MaskWindow.UnlockUI();
		}

	}
	void setJuGongValue()
	{
		if (StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid) == null)
		{
			juGongValue.text = "0/" + CommandConfigManager.Instance.lastBattleData.junGongMaxNum;
		}
		else
		{
			juGongValue.text = StorageManagerment.Instance.getProp(CommandConfigManager.Instance.lastBattleData.junGongSid).getNum() + "/" + CommandConfigManager.Instance.lastBattleData.junGongMaxNum;
		}
	}
	GameObject obj;
	GoodsView view;
	Transform trans;
	public void initGoodsInfoPanel(LastBattleDonationSample itemData,LastBattleDonateItem item)
	{
		if(itemData != null)
		{
			donateItem = item;
			sample = itemData;
			obj = GameObject.Instantiate(goodsTmp.gameObject) as GameObject;
			view = obj.GetComponent<GoodsView>();
			trans = obj.transform;

			view.init(itemData.donation);
			view.rightBottomText.gameObject.SetActive(false);
			view.fatherWindow = this;
			trans.parent = goodsPos.parent;
			trans.localPosition = goodsPos.localPosition;
			trans.localScale = Vector3.one;

			goodsInfoPanel.SetActive(true);
			haveCount.text = LastBattleManagement.Instance.getHaveGoodsCount(itemData);
			donateCount.text = itemData.donation.num;
		}

	}
	void clickOkBtn()
	{
		if(sample == null)
		{
			MaskWindow.UnlockUI();
			return;
		}

		// 军功超上限提示//
		if(LastBattleManagement.Instance.creatJunGongMaxTipByGetCount(sample.junGong))
			return;

		// 发送捐献消息//
		LastBattleDonateFPort fPort=FPortManager.Instance.getFPort ("LastBattleDonateFPort") as LastBattleDonateFPort;
		if(sample.donation.type == PrizeType.PRIZE_CARD || sample.donation.type == PrizeType.PRIZE_EQUIPMENT)
		{
			string msg = "";
			ArrayList list;
			if(sample.donation.type == PrizeType.PRIZE_EQUIPMENT)
			{
				msg = "equipment,";
				Equip equip = StorageManagerment.Instance.getEquipBySid(sample.donation.pSid);
				list = StorageManagerment.Instance.getAllEquipByEatByQuiltyID(equip.getQualityId());
				for(int i=0;i<StringKit.toInt(sample.donation.num);i++)
				{
					if(i != StringKit.toInt(sample.donation.num) - 1)
					{
						msg += (list[i] as Equip).uid + ",";
					}
					else 
					{
						msg += (list[i] as Equip).uid;
					}
				}
			}
			else if(sample.donation.type == PrizeType.PRIZE_CARD)
			{
				msg = "card,";
				Card card = StorageManagerment.Instance.getCardBySid(sample.donation.pSid);
				list = StorageManagerment.Instance.getAllRoleByEatByQuiltyID(card.getQualityId());
				for(int i=0;i<StringKit.toInt(sample.donation.num);i++)
				{
					if(i != StringKit.toInt(sample.donation.num) - 1)
					{
						msg += (list[i] as Card).uid + ",";
					}
					else 
					{
						msg += (list[i] as Card).uid;
					}
				}
			}
			fPort.lastBattleDonateAccess(donateCallBack,sample.index,msg); 
		}
		else
		{ 
			fPort.lastBattleDonateAccess2(donateCallBack,sample.index); 
		}

	}
	void donateCallBack()
	{
		if(sample != null)
		{
			showPrize(sample);
			sample.state = LastBattleDonationState.YES_DONATE;
			updateItem();
			setJuGongValue();
			closeGoodsInfoPanel();
			sample = null;
			MaskWindow.UnlockUI();
		}
	}
	void updateItem()
	{
		LastBattleDonateItem item;
		if(LastBattleManagement.Instance.currentDonationList != null && LastBattleManagement.Instance.currentDonationList.Count > 0)
		{
			for(int i=0;i<LastBattleManagement.Instance.currentDonationList.Count;i++)
			{
				item = donationsContent.GetChild(i).gameObject.GetComponent<LastBattleDonateItem>();
				if(item != null)
				{
					item.updateDonateButtonState(item.getItemData());
				}
			}
		}
	}
	// 飘字显示获得的军功 积分 赐福等级 进度//
	string[] prizeStr = new string[4];
	void showPrize(LastBattleDonationSample sample)
	{
		prizeStr[0] = string.Format(LanguageConfigManager.Instance.getLanguage("junGongAdd"),"X" + sample.junGong);
		prizeStr[1] = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_RankScoreAdd"),sample.scores);
		prizeStr[2] = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_NvShenAddLV"),sample.nvShenBlessLV);
		prizeStr[3] = string.Format(LanguageConfigManager.Instance.getLanguage("LastBattle_ProcessAdd"),sample.process);
		UiManager.Instance.createMessageLintWindow(prizeStr);
	}

	// 断线重连//
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		closeGoodsInfoPanel();
		if(itemObjs != null)
		{
			for(int i=0;i<itemObjs.Length;i++)
			{
				GameObject.Destroy(itemObjs[i]);
			}
		}
		LastBattleInitFPort init = FPortManager.Instance.getFPort ("LastBattleInitFPort") as LastBattleInitFPort;
		init.lastBattleInitAccess(initWin);
	}
	void closeGoodsInfoPanel()
	{
		if(obj != null)
		{
			GameObject.Destroy(obj);
		}
		goodsInfoPanel.SetActive(false);
	}
}
