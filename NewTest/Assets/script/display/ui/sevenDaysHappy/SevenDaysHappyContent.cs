using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SevenDaysHappyContent : MonoBase 
{
	public GameObject detailButtonTmp;// 内容页签模板//
	public GameObject detail_right;// 内容页签右边箭头//
	public GameObject detail_left;// 内容页签左边箭头//
	public UILabel timeTittle;// 时间描述//
	public UILabel timeValue;// 活动时间//
	public Transform detailButtonsPanel;
	private GameObject[] detailButtons;
	private SevenDaysHappySample sample;
	public SevenDaysHappyDetailBtn selectedDetailBtn;// 选中的详情按钮//
	public SevenDaysHappyMissonContent missonContent;// 任务列表// 
	public GameObject banjiaPanel;// 购买半价界面//
	public GoodsView banjia_good;// 半价购买商品//
	public UILabel banjia_goodName;// 半价商品名称 //
	public UILabel banjia_goodsPriceBefore;// 半价商品原价//
	public UILabel banjia_goodsPriceNow;// 半价商品现价//
	public ButtonBase banjia_goumaiBtn;// 半价商品购买按钮//
	public WindowBase fatherWin;
	SevenDaysHappyMisson misson;
	// 活动结束文字位置//
	private Vector3 timeEndPos = new Vector3(-60,159,0);


	public void initContent(SevenDaysHappySample sample,DayTopButton topBtn)
	{
		this.sample = sample;
		initDetailButtons(topBtn);
	}

	//  初始化半价购买界面//
	public void initBanjiaPanel(SevenDaysHappyMisson misson)
	{
		this.misson = misson;

		banjia_good.init(misson.prizes[0]);
		banjia_good.fatherWindow = fatherWin;

		banjia_goodName.text = banjia_good.showName;
		banjia_goodsPriceBefore.text = (misson.price * 2).ToString();
		banjia_goodsPriceNow.text = misson.price.ToString();

		if(banjia_goumaiBtn.onClickEvent == null)
		{
			banjia_goumaiBtn.onClickEvent = clickGouMai;
		}
		if(misson.missonState == SevenDaysHappyMissonState.Recevied)
		{
			banjia_goumaiBtn.disableButton(true);
		}
		else
		{
			if(SevenDaysHappyManagement.Instance.getActiveMissonEndTime() - ServerTimeKit.getSecondTime() > 0)
			{
				banjia_goumaiBtn.disableButton(false);
			}
			else
			{
				banjia_goumaiBtn.disableButton(true);
			}
		}
	}
	public void clickGouMai(GameObject obj)
	{
		if(!isPropStorageFull(misson.prizes[0]))// 仓库未满//
		{
			if(UserManager.Instance.self.getRMB () >= misson.price)
			{
				BuyGoodsFPort fport = FPortManager.Instance.getFPort ("BuyGoodsFPort") as BuyGoodsFPort;
				fport.buyGoods(misson.goodsID,1,updateBanjiaBtn);
			}
			else
			{
				string str = LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_notEnoughRMB");
				MessageWindow.ShowRecharge(str);
			}
		}
		else
		{
			// 飘字提示，仓库已满请清理//
			UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
				win.Initialize (LanguageConfigManager.Instance.getLanguage ("storeFull"));
			});
		}
	}
	// 更新半价购买按钮//
	public void updateBanjiaBtn(int i,int j)
	{
		if(misson != null)
		{
			UiManager.Instance.createPrizeMessageLintWindow(misson.prizes[0]);
			SevenDaysHappyMisson _misson = SevenDaysHappyManagement.Instance.getMissonByMissonID(misson.missonID);
			if(_misson != null)
			{
				_misson.missonState = SevenDaysHappyMissonState.Recevied;
				_misson.missonProgress[0] = 1;
				ShopManagerment.Instance.updateBanJiaGood(misson.goodsID,1);
			}
		}

		banjia_goumaiBtn.disableButton(true);
	}

	public void initDetailButtons(DayTopButton topBtn)
	{
		int detailCount = 0;
		SevenDaysHappyDetailBtn btn;
		GameObject obj;
		detailButtons = new GameObject[sample.detailsDic.Count];
		foreach (KeyValuePair<int,SevenDaysHappyDetail> item in sample.detailsDic)
		{
			obj = Instantiate(detailButtonTmp) as GameObject;
			obj.transform.parent = detailButtonsPanel;
			obj.SetActive(true);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			detailButtons[detailCount] = obj;
			btn = obj.GetComponent<SevenDaysHappyDetailBtn>();
			if(btn != null)
			{
				if(detailCount == 0)
				{
					selectedDetailBtn = btn;
				}
				btn.initDetailBtn(item.Value,this,topBtn);
				if(detailCount == 0)
				{
					missonContent.destroyMissons();
					// 显示任务列表//
					//btn.showMisson();
					if(btn.detail.type == SevenDaysHappyDetailType.banjiaqianggou)// 当初始化的是半价购买时//
					{
						btn.content.initBanjiaPanel(btn.detail.missonList[0]);
						btn.content.banjiaPanel.SetActive(true);
					}
					else
					{
						btn.showMisson();
					}
					topBtn.detail = btn.detail;
				}
			}
			detailCount++;
		}
		detailButtonsPanel.gameObject.GetComponent<UIGrid>().repositionNow = true;
		if(detailCount > 4)
		{
			detail_right.SetActive(true);
			detail_left.SetActive(true);
		}
	}

	public void destroyDetailButtons()
	{
		if(detailButtons != null)
		{
			for(int i=0;i<detailButtons.Length;i++)
			{
				Destroy(detailButtons[i]);
			}
		}
	}

	//验证相关仓库是否满
	private bool isPropStorageFull (PrizeSample prop)
	{
		bool isfull = false;
		if (prop == null)
			return false;
		switch (prop.type) {
		case PrizeType.PRIZE_CARD:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllRole ().Count > StorageManagerment.Instance.getRoleStorageMaxSpace ()) {
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_BEAST:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllBeast ().Count > StorageManagerment.Instance.getBeastStorageMaxSpace ()) {
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_EQUIPMENT:
			if (prop.getPrizeNumByInt () + StorageManagerment.Instance.getAllEquip ().Count > StorageManagerment.Instance.getEquipStorageMaxSpace ()) {
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_MAGIC_WEAPON:
			if (prop.getPrizeNumByInt() + StorageManagerment.Instance.getAllMagicWeapon().Count > StorageManagerment.Instance.getMagicWeaponStorageMaxSpace()) {
				isfull = true;
			} else {
				isfull = false;
			}
			break;
		case PrizeType.PRIZE_PROP:
			if (StorageManagerment.Instance.getProp (prop.pSid) != null) {
				isfull = false;
			} else {
				if (1 + StorageManagerment.Instance.getAllProp ().Count > StorageManagerment.Instance.getPropStorageMaxSpace ()) {
					isfull = true;
				} else {
					isfull = false;
				}
			}
			break;
		}
		return isfull;		
	}

	void Update()
	{
		// 在任务时间期间//
		if(SevenDaysHappyManagement.Instance.getActiveMissonEndTime() - ServerTimeKit.getSecondTime() > 0)
		{
			timeTittle.text = LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_missonEnd");
			timeValue.text = TimeKit.timeTransformDHMS(SevenDaysHappyManagement.Instance.getActiveMissonEndTime() - ServerTimeKit.getSecondTime());
		}
		else
		{
			// 在领奖时间期间//
			if(SevenDaysHappyManagement.Instance.getEndTime() - ServerTimeKit.getSecondTime() > 0)
			{
				timeTittle.text = LanguageConfigManager.Instance.getLanguage("sevenDaysHappy_awardEnd");
				timeValue.text = TimeKit.timeTransformDHMS(SevenDaysHappyManagement.Instance.getEndTime() - ServerTimeKit.getSecondTime());
			}
			else// 整个活动结束//
			{
				timeValue.gameObject.SetActive(false);
				timeTittle.text = LanguageConfigManager.Instance.getLanguage("s0211");
				timeTittle.transform.localPosition = timeEndPos;
			}
			// 处理半价抢购//
			if(banjiaPanel.activeSelf)
			{
				banjia_goumaiBtn.disableButton(true);
			}
		}
	}

}
