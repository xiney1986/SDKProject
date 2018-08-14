using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LotteryWindow : WindowBase
{
	public GameObject handSelectBtn;// 手动选号按钮obj//
	public GameObject randomSelectBtn;// 随机选号按钮obj//
	public GameObject shuoMingBtn;// 次数说明按钮obj//
	public GameObject selectBtn;// 选注按钮obj//
	public UILabel canSelectCount;// 可选注次数label//
	public GameObject labelTmp;// 选号显示模板//
	public UIGrid infoGrid;
	public UILabel costGoldLabel;
	public UILabel costRmbLabel;

	public GameObject tmp;
	public GameObject shuomingPanel;
	public UIGrid shuomingGrid;
	public GameObject shuomingClose;

	public BoxCollider drag;
	public BoxCollider buyBtnCollider;
	public BoxCollider randomBtnCollider;
	public BoxCollider handBtnCollider;

	protected override void begin ()
	{
		LotteryManagement.Instance.selectNumList.Clear();
		base.begin ();
		costGoldLabel.text = "0";
		costRmbLabel.text = "0";
		if(LotteryManagement.Instance.getLotteryCount() > 0)
		{
			selectBtn.GetComponent<ButtonBase>().disableButton(false);
		}
		else
		{
			selectBtn.GetComponent<ButtonBase>().disableButton(true);
		}
		canSelectCount.text = LotteryManagement.Instance.getLotteryCount().ToString();
		updateBtnState();
		initShuomingPanel();

		MaskWindow.UnlockUI();
	}

	public void initShuomingPanel()
	{
		GameObject obj;
		for(int i=0;i<LotteryBuyCountConfigManager.Instance.getSample().Count;i++)
		{
			obj = GameObject.Instantiate(tmp) as GameObject;
			obj.transform.parent = tmp.transform.parent;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			//obj.transform.FindChild("Sprite").gameObject.GetComponent<UISprite>().spriteName = "vip" + LotteryBuyCountConfigManager.Instance.getSample()[i].vipLv;
			obj.transform.FindChild("vipLv").gameObject.GetComponent<UILabel>().text = LotteryBuyCountConfigManager.Instance.getSample()[i].vipLv.ToString();
			obj.transform.FindChild("goldCount").gameObject.GetComponent<UILabel>().text = LotteryBuyCountConfigManager.Instance.getSample()[i].goldCount.ToString();
			obj.transform.FindChild("rmbCount").gameObject.GetComponent<UILabel>().text = LotteryBuyCountConfigManager.Instance.getSample()[i].rmbCount.ToString();
			obj.SetActive(true);
		}
		shuomingGrid.repositionNow = true;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);

		if(gameObj == handSelectBtn)// 手动选号//
		{
			// 可买次数超了//
			if(LotteryManagement.Instance.getLotteryCount() <= 0)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_buyCountNotEough"));
				});
			}
			else 
			{
				UiManager.Instance.openDialogWindow<lotteryInputWindow>();
//				if(LotteryManagement.Instance.enoughToBuy(UserManager.Instance.self.getVipLevel()))
//				{
//					UiManager.Instance.openDialogWindow<lotteryInputWindow>();
//				}
			}
		}
		else if(gameObj == randomSelectBtn)// 随机选号//
		{
			// 可买次数超了//
			if(LotteryManagement.Instance.getLotteryCount() <= 0)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_buyCountNotEough"));
				});
			}
			else 
			{
				randomSelectNum();
//				if(LotteryManagement.Instance.enoughToBuy(UserManager.Instance.self.getVipLevel()))
//				{
//					randomSelectNum();
//				}
			}
		}
		else if(gameObj == shuoMingBtn)// 次数说明//
		{
			shuomingPanel.SetActive(true);
			MaskWindow.UnlockUI();
		}
		else if(gameObj == selectBtn)// 选注//
		{
			if(LotteryManagement.Instance.selectNumList != null && LotteryManagement.Instance.selectNumList.Count > 0)
			{
				if(LotteryManagement.Instance.enoughToBuy(UserManager.Instance.self.getVipLevel()))
				{
					sendBuyMsg(LotteryManagement.Instance.selectNumList);
				}
			}
			else 
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_selectNumFirst"));
				});
			}

		}
		else if(gameObj.name == "close" && gameObj != shuomingClose)
		{
			LotteryManagement.Instance.selectNumList.Clear();
			LotteryInfoFPort fPort = FPortManager.Instance.getFPort ("LotteryInfoFPort") as LotteryInfoFPort;
			fPort.lotteryInfoAccess(()=>{
				LotteryManagement.Instance.canGetInitFPort = false;
				finishWindow();
				UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().reSetMoneyTimeCount();
				UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initMainPanel();
				UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initRadioLabels();
				UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setTimer();
				UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setBoolTrue();
			});

		}
		else if(gameObj == shuomingClose)
		{
			shuomingPanel.SetActive(false);
		}
	}

	void sendBuyMsg(List<string> numList)
	{
		string numStrs = "";
		for(int i=0;i<numList.Count;i++)
		{
			if(i != numList.Count - 1)
			{
				numStrs += numList[i] + ",";
			}
			else 
			{
				numStrs += numList[i];
			}
		}
		LotteryBuyFPort fPort = FPortManager.Instance.getFPort("LotteryBuyFPort") as LotteryBuyFPort;
		fPort.lotteryBuyFPortAccess(CommandConfigManager.Instance.getLotteryData().sid,numStrs,updateAfterSelect);
	}
	// 选注成功后刷新//
	void updateAfterSelect()
	{
		UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
			win.Initialize (LanguageConfigManager.Instance.getLanguage ("Lottery_selectSuccess"));
		});
		updateBtnState();
		LotteryManagement.Instance.selectNumList.Clear();
		MaskWindow.UnlockUI();
		LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
		fPort.lotteryInfoAccess(()=>{
			LotteryManagement.Instance.canGetInitFPort = false;
			finishWindow();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().reSetMoneyTimeCount();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initMainPanel();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initRadioLabels();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setTimer();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setBoolTrue();
		});
	}
	// 在已选号码列表里生成号码//
	public void creatSelectedNumList(string numStr)
	{
		if(!drag.enabled)
		{
			drag.enabled = true;
		}
		GameObject numObj = GameObject.Instantiate(labelTmp) as GameObject;
		numObj.transform.parent = labelTmp.transform.parent;
		numObj.transform.localPosition = Vector3.zero;
		numObj.transform.localScale = Vector3.one;
		//numObj.GetComponent<UILabel>().text = numStr;
		LotteryManagement.Instance.setNumSprite(numObj.transform,numStr);
		numObj.SetActive(true);

		infoGrid.repositionNow = true;
	}
	// 随机选号//
	void randomSelectNum()
	{
		string str = "";
		for(int i=0;i<4;i++)
		{
			str += Random.Range(0,9).ToString();
		}
		setCostLabel(UserManager.Instance.self.getVipLevel());
		LotteryManagement.Instance.selectNumList.Add(str);
		LotteryManagement.Instance.currentDayBuyCount++;
		canSelectCount.text = LotteryManagement.Instance.getLotteryCount().ToString();
		updateBtnState();
		creatSelectedNumList(str);
		MaskWindow.UnlockUI();
	}
	// 更新按钮状态//
	public void updateBtnState()
	{
		if(LotteryManagement.Instance.getLotteryCount() > 0)
		{
			randomSelectBtn.GetComponent<ButtonBase>().disableButton(false);
			handSelectBtn.GetComponent<ButtonBase>().disableButton(false);
		}
		else
		{
			randomSelectBtn.GetComponent<ButtonBase>().disableButton(true);
			handSelectBtn.GetComponent<ButtonBase>().disableButton(true);
		}
	}

	public void setCostLabel(int vipLv)
	{
		// 花费金币//
		if(LotteryManagement.Instance.currentDayBuyCount < LotteryBuyCountConfigManager.Instance.getCountSample(vipLv).goldCount)
		{
			costGoldLabel.text = (StringKit.toInt(costGoldLabel.text) + CommandConfigManager.Instance.getLotteryData().costGold).ToString();
		}
		// 花费钻石//
		else 
		{
			costRmbLabel.text = (StringKit.toInt(costRmbLabel.text) + CommandConfigManager.Instance.getLotteryData().costRmb).ToString();
		}		
	}

	public override void OnNetResume ()
	{
		base.OnNetResume ();
		LotteryInfoFPort fPort = FPortManager.Instance.getFPort("LotteryInfoFPort") as LotteryInfoFPort;
		fPort.lotteryInfoAccess(()=>{
			LotteryManagement.Instance.canGetInitFPort = false;
			finishWindow();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().reSetMoneyTimeCount();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initMainPanel();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().initRadioLabels();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setTimer();
			UiManager.Instance.getWindow<NoticeWindow>().show.GetComponent<LotteryContent>().setBoolTrue();
		});
	}
}
