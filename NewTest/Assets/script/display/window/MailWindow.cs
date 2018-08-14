using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 邮件和临时仓库窗口
 * @author 汤琦
 * */
public class MailWindow : WindowBase
{
	public TapContentBase tapBase;//分页按钮
	public TempItemContent tempContent;//临时仓库页面
	public MailItemContent mailContent;//邮件页面
	private bool isMailPage;//是否在邮件页面
	private string oneKeyString;//一键删除提示框内容
	public ButtonBase oneKeyDeleteButton;
	public ButtonBase oneKeyExtractButton;
	public GameObject mailItemPrefab;
	public GameObject templtemPrefab;

    public GameObject mailNum;
    public GameObject tempNum;
    public UILabel mailCount;
    public UILabel tempCount;
	
	protected override void begin ()
	{
		base.begin ();
        GuideManager.Instance.doFriendlyGuideEvent();
        if (UiManager.Instance.getWindow<AllAwardViewWindow>() != null) {
            UiManager.Instance.getWindow<AllAwardViewWindow>().gameObject.SetActive(true);
        }
		MaskWindow.UnlockUI();
	}

	//断线重连
	public override void OnNetResume ()
	{
		base.OnNetResume ();
		Initialize(0);
	}

    public void showNum()
    {
        if (MailManagerment.Instance.getUnReadMailNum () > 0)
        {
            mailCount.text = MailManagerment.Instance.getUnReadMailNum().ToString();
            mailNum.SetActive(true);
        }else
            mailNum.SetActive(false);
        if (StorageManagerment.Instance.getValidAllTemp(StorageManagerment.Instance.getAllTemp()).Count > 0)
        {
            tempCount.text = StorageManagerment.Instance.getValidAllTemp(StorageManagerment.Instance.getAllTemp()).Count.ToString();
            tempNum.SetActive(true);
        }
        else
            tempNum.SetActive(false);

    }
	public void changeButton(bool isMailPage)
	{
		if(isMailPage)
		{
			if(MailManagerment.Instance.getAllMail().Count > 0)
			{
				oneKeyDeleteButton.disableButton(false);
			}
			else
			{
				oneKeyDeleteButton.disableButton(true);
			}
			if(MailManagerment.Instance.isHaveAnnex())
			{
				oneKeyExtractButton.disableButton(false);
			}
			else
			{
				oneKeyExtractButton.disableButton(true);
			}
		}
		else
		{
			if(StorageManagerment.Instance.getAllTemp() == null||StorageManagerment.Instance.getAllTemp().Count > 0)
			{
				oneKeyDeleteButton.disableButton(false);
				oneKeyExtractButton.disableButton(false);
			}
			else
			{
				oneKeyDeleteButton.disableButton(true);
				oneKeyExtractButton.disableButton(true);
			}
		}
	}

	protected override void DoEnable ()
	{
		//base.DoEnable ();
		if(MailManagerment.isUpdateMailInfo) {
			Initialize(1);
			Initialize(0);
			MailManagerment.isUpdateMailInfo = false;
		}


	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
		if(mailContent!=null&&mailContent.timer!=null)
		{
			mailContent.timer.stop();
		}
        if (UiManager.Instance.getWindow<AllAwardViewWindow>() != null) {
            UiManager.Instance.getWindow<AllAwardViewWindow>().gameObject.SetActive(false);
        }
	}
	//临时仓库筛选
	private ArrayList tempSift(ArrayList list)
	{
		ArrayList displayList = StorageManagerment.Instance.getValidAllTemp (list);
		//按时间排序
		for (int i = 0; i < displayList.Count-1; i++) {
			for (int j = 0; j < displayList.Count-1-i; j++) {
				if((displayList[j] as TempProp).time>(displayList[j+1] as TempProp).time)
				{
					object temp = displayList[j];
					displayList[j] = displayList[j+1];
					displayList[j+1] = temp;
				}
			}
		}
		return displayList;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		//一键提取
		if(gameObj.name == "oneKeyExtract")
		{
			//邮件
			if(isMailPage)
			{
				if(!MailManagerment.Instance.isOneKeyMailExtract()) {
					UiManager.Instance.createMessageWindowByOneButton(MailManagerment.Instance.getStr(),null);
					return;
				}
				if(MailManagerment.Instance.isHaveAnnex()) {
					UiManager.Instance.applyMask();
					sendOneKeyExtractMailFPort();
				}
			}
			//临时仓库
			else
			{
				if(isFull())
				{
					UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
						win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0040"),null,LanguageConfigManager.Instance.getLanguage("TempGetFull"),null);
					});
					return;
				}
				else
				{
					sendOneKeyExtractFPort();
					return;
				}
			}
			
			
		}
		//一键删除
		if(gameObj.name == "oneKeyDelete")
		{
			//邮件
			if(isMailPage)
			{
				oneKeyString = LanguageConfigManager.Instance.getLanguage("s0110");
			}
			//临时仓库
			else
			{
				oneKeyString = LanguageConfigManager.Instance.getLanguage("s0110");
			}
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(2,LanguageConfigManager.Instance.getLanguage("s0040"),LanguageConfigManager.Instance.getLanguage("s0093"),oneKeyString,sendOneKeyDeleteFPort);
			});

		}
		if(gameObj.name == "close")
		{
			finishWindow();
		}
	}
	//一键提取邮件通信
	private void sendOneKeyExtractMailFPort()
	{
		OneKeyExtractMailFPort fport = FPortManager.Instance.getFPort("OneKeyExtractMailFPort") as OneKeyExtractMailFPort;
		fport.access(oneKeyExtractMailBack);
	}
	//一键提取邮件回调
	private void oneKeyExtractMailBack(bool isOK ,bool totalExtract)
	{
		if(isOK) {
			mailContent.reLoad (MailManagerment.Instance.getSortAllMail());
			/*
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0040"),null,LanguageConfigManager.Instance.getLanguage("s0115"),oneKeyExtractMailResult);
			});
			*/

			// 如果没有全部领完  提示信息//
			if(!totalExtract)
			{
				UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
					win.Initialize (LanguageConfigManager.Instance.getLanguage ("mail05"));
				});
			}
		}
		else {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("s0204"),oneKeyExtractMailResult);
		}
	}
	
	
	private void oneKeyExtractMailResult (MessageHandle msg)
	{
		mailContent.reLoad (MailManagerment.Instance.getSortAllMail());
		changeButton(true); 
		UiManager.Instance.cancelMask();
	}
	//一键删除邮件回调
	private void oneKeyDeleteMailBack()
	{
		MailManagerment.Instance.clearMail(false);
        UiManager.Instance.openDialogWindow<MessageLineWindow>((win) =>
        {
            win.Initialize(LanguageConfigManager.Instance.getLanguage("s0118"));
        });
		tapBase.resetTap();
		Initialize(0);
	}
	//一键删除通信
	private void sendOneKeyDeleteFPort(MessageHandle msg)
	{
		if(msg.buttonID == MessageHandle.BUTTON_LEFT)
			return;
		//邮件
		if(isMailPage)
		{
			OneKeyDeleteMailFPort fport = FPortManager.Instance.getFPort("OneKeyDeleteMailFPort") as OneKeyDeleteMailFPort;
			fport.access(oneKeyDeleteMailBack);
		}
		//临时仓库
		else
		{
			TempStorageOneKeyDeleteFPort fport = FPortManager.Instance.getFPort("TempStorageOneKeyDeleteFPort") as TempStorageOneKeyDeleteFPort;
			fport.access(tempStorageOneKeyDeleteBack);
			
		}
	}
	private void tempStorageOneKeyDeleteBack()
	{
		StorageManagerment.Instance.getTempStorage ().clearStorgae ();
		tapBase.resetTap();
		Initialize(1);
	}
	//一键提取临时仓库通信
	private void sendOneKeyExtractFPort()
	{
		TempStorageOneKeyExtractFPort fport = FPortManager.Instance.getFPort("TempStorageOneKeyExtractFPort") as TempStorageOneKeyExtractFPort;
		fport.access(oneKeyExtractBack);
	}
	//一键提取临时仓库回调
	private void oneKeyExtractBack()
	{
		UiManager.Instance.cancelMask ();
		UiManager.Instance.openDialogWindow<MessageLineWindow>((win)=>{
			win.Initialize(LanguageConfigManager.Instance.getLanguage("s0115"));
		});
		
		tapBase.resetTap();
		Initialize(1);
	}
	
	//初始化信息
	public void Initialize(int index)
	{
        showNum();
		tapBase.changeTapPage(tapBase.tapButtonList[index]);
	}
	//页面按钮
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "buttonTemp" && enable == true) 
		{ 
			GuideManager.Instance.onceGuideEvent(GuideGlobal.ONCEGUIDE2);
			MailManagerment.Instance.runDeleteUids();
			isMailPage = false;
			if(StorageManagerment.Instance.getAllTemp()==null)
			{
				tempContent.reLoad (0);
			}
			else
			{
				tempContent.reLoad (tempSift(StorageManagerment.Instance.getAllTemp()));
			}
			changeButton(isMailPage);
			//mailContent.cleanAll();
		}
		if (gameObj.name == "buttonMail" && enable == true)
		{
			GuideManager.Instance.onceGuideEvent(GuideGlobal.ONCEGUIDE1);
			MailManagerment.Instance.runDeleteUids();
			isMailPage = true;
			if(MailManagerment.Instance.getAllMail() != null)
				mailContent.reLoad (MailManagerment.Instance.getSortAllMail());
			changeButton(isMailPage);
			//tempContent.cleanAll();
		}
	}
	//验证相关仓库是否满
	private bool isFull()
	{
		int tempEquip = 0;
		int tempProp = 0;
		int tempCard = 0;
		int tempBeast = 0;
		ArrayList list = StorageManagerment.Instance.getAllTemp();
		for (int i = 0; i < list.Count; i++) {
			if((list[i] as TempProp).type == TempPropType.BEAST )
			{

				tempBeast += (list[i] as TempProp).getNum();
			}
			else if((list[i] as TempProp).type == TempPropType.CARD)
			{
				tempCard += (list[i] as TempProp).getNum();
			}
			else if((list[i] as TempProp).type == TempPropType.EQUIPMENT)
			{
				tempEquip += (list[i] as TempProp).getNum();
			}
			else if((list[i] as TempProp).type == TempPropType.GOODS)
			{
				tempProp += (list[i] as TempProp).getNum();
			}
		}
		if(tempBeast + StorageManagerment.Instance.getAllBeast().Count>StorageManagerment.Instance.getBeastStorageMaxSpace())
		{
			return true;
		}
		else if(tempEquip + StorageManagerment.Instance.getAllEquip().Count>StorageManagerment.Instance.getEquipStorageMaxSpace())
		{
			return true;
		}
		else if(tempProp + StorageManagerment.Instance.getAllProp().Count>StorageManagerment.Instance.getPropStorageMaxSpace())
		{
			return true;
		}
		else if(tempCard + StorageManagerment.Instance.getAllRole().Count>StorageManagerment.Instance.getRoleStorageMaxSpace())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
