using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/**
 * 激活码邀请主窗口
 * @authro 陈世惟  
 * */

public class InviteCodeWindow : WindowBase {

	public GameObject invitationTitle;//邀请标题
	public UILabel myCode;//我的邀请码
	public UILabel invitationPlayerNum;//已邀请数目

	public ContentInviteCode contentInviteCode;
	private List<InviteCode> inCodes;//前台数据

	public GameObject inviteCodeButtonPrefab;
	public ButtonBase buttonSend;
	public GameObject shareButton;

	protected override void begin ()
	{
		base.begin ();
		isGet(InviteCodeManagerment.Instance.inviteType);
		if(isAwakeformHide==false)
		{
			getMsgFport();
			myCode.text = StringKit.serverIdToFrontId(UserManager.Instance.self.uid);
		}
		MaskWindow.UnlockUI ();
	}
	
	public override void DoDisable ()
	{
		base.DoDisable ();
	}
	
	
	public void reLoadInviteContent()
	{
		isGet(InviteCodeManagerment.Instance.inviteType);
		inCodes = InviteCodeManagerment.Instance.getAllInviteCodeByTapType(1);
		invitationPlayerNum.text = InviteCodeManagerment.Instance.inviteNum;
		contentInviteCode.Initialize(inCodes);
	}
	
	//获取后台进度等信息

	public void getMsgFport()
	{
		InviteCodeInfoFPort fport = FPortManager.Instance.getFPort("InviteCodeInfoFPort") as InviteCodeInfoFPort;
		fport.access(reLoadContent);
	}
	
	//激活邀请码领奖

	public void invtiteCodeFport(string playerCode)
	{
		if(playerCode.Replace(" ","") == "")
			return;
		if(playerCode == null)
			return;
		//只能纯数字

		if(!StringKit.isNum(playerCode))
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0335"),null);
			});
			return;
		}
		//不能激活自己
		if(playerCode == myCode.text)
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0336"),null);
			});
			return;
		}
		//已激活

		if(InviteCodeManagerment.Instance.inviteType == 1)
		{
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0333"),null);
			});
			return;
		}
		InviteCodeInviteFPort fport = FPortManager.Instance.getFPort("InviteCodeInviteFPort") as InviteCodeInviteFPort;
		string code=StringKit.frontIdToServerId(playerCode);

		if(code=="error"){
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow(1,LanguageConfigManager.Instance.getLanguage("s0093"),null,LanguageConfigManager.Instance.getLanguage("s0335"),null);
			});
			return;
		}else{
			fport.access(code,changeInviteType,hideWin,windowBack);
		}

	}

	//标签切换方法

	public void Initialize(int index)
	{

	}
	
	//切换页面

	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.tapButtonEventBase (gameObj, enable);
		
		if (gameObj.name == "buttonInvite" && enable == true) 
		{
			GuideManager.Instance.onceGuideEvent(GuideGlobal.ONCEGUIDE_INVITE1);
			invitationTitle.SetActive (true);
		}
		if (gameObj.name == "buttonHufen" && enable == true) 
		{
			invitationTitle.SetActive (false);
		}
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		
		if(gameObj.name == "close")
		{
			finishWindow();
		}else if(gameObj.name == "buttonSend")
		{
			UiManager.Instance.openDialogWindow<InviteSendCodeWindow>((win)=>{
				win.initWindow(InviteCodeManagerment.Instance.inviteType);
			});
		}
	}
	
	private void reLoadContent ()
	{
		reLoadInviteContent();
	}

	private void windowBack()
	{
		UiManager.Instance.openWindow<InviteCodeWindow>((win)=>{ });
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>((win)=>{});
	}


	//成功领取奖励后前台临时改变状态，无需重新向后台要数据

	private void changeInviteType()
	{
		InviteCodeManagerment.Instance.inviteType = 1;
		isGet(InviteCodeManagerment.Instance.inviteType);
	}
	
	//查看奖励物品时调用

	private void hideWin()
	{
		hideWindow();
	}
	
	//是否领取激活奖励

	public void isGet(int _type)
	{
		if(_type == 0)
		{
			buttonSend.disableButton(false);
			buttonSend.textLabel.text=LanguageConfigManager.Instance.getLanguage("invitcode03");
		}
		if(_type == 1)
		{
			buttonSend.textLabel.text=LanguageConfigManager.Instance.getLanguage("goddess11");
			buttonSend.disableButton(true);
		}
	}

}
