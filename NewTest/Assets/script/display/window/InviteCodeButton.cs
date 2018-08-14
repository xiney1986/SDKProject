using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InviteCodeButton : ButtonBase {

	public UILabel labelName;//奖励名称
	public UILabel labelNeed;//描述
	public UILabel labelJindu;//进度
	public UISprite spriteIsDone;//达成未达成图标
	public PrizesModule prizeIcon;//奖励图标

	private int index;
	private InviteCode ic;//前台邀请任务奖励信息
	private int currentNum;
	private InviteCodeWindow win;
	private string namelabel;
	
	public void initUI (InviteCode _ic,int _index)
	{
		if(labelJindu.gameObject.activeSelf)
			labelJindu.gameObject.SetActive (false);
		win = fatherWindow as InviteCodeWindow;
 
		if (_ic == null)
			return;
		ic = _ic;
		index = _index;
		ShowAward();
		ShowJindu();
		isReceived();
	}
	
	public void ShowAward()
	{
		prizeIcon.initPrize (getPrize (), windowBack, win);
		labelName.text = namelabel; 
		labelNeed.text = LanguageConfigManager.Instance.getLanguage ("s0329",ic.inviteNeedNum,ic.needLevel);
	}
	
	public void ShowJindu()
	{
		if(!labelJindu.gameObject.activeSelf)
			labelJindu.gameObject.SetActive (true);
		labelJindu.text = LanguageConfigManager.Instance.getLanguage ("s0328") + ic.jindu + "/" + ic.inviteNeedNum;
	}
	
	public override void DoClickEvent ()
	{
		MaskWindow.LockUI ();

		if(ic.awardType == 0) {
			if(ic.jindu < StringKit.toInt(ic.inviteNeedNum)) {
				MaskWindow.UnlockUI();
				return;
			}
		}
		else {
			MaskWindow.UnlockUI();
			return;
		}

		string str="";
		if (StorageManagerment.Instance. checkStoreFull (ic.prizes.ToArray(),out str)) {
			UiManager.Instance.openDialogWindow<MessageWindow>((win)=>{
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), "", str + "," + LanguageConfigManager.Instance.getLanguage("s0203"), null);
			});
			MaskWindow.UnlockUI();
			return;
		}else{
		getAwardFPort();
		}
	}

	public void showWin()
	{
		UiManager.Instance.openWindow<InviteCodeWindow>();
		UiManager.Instance.openDialogWindow<AllAwardViewWindow>();
	}
	//领取奖励端口,传入奖励ID
	public void getAwardFPort()
	{

		InviteGetAwardFPort fport = FPortManager.Instance.getFPort("InviteGetAwardFPort") as InviteGetAwardFPort;
		fport.access(StringKit.toInt(ic.uid),getAward);
	}
	
	//成功领取奖励后调用
	public void getAward()
	{
		UiManager.Instance.openDialogWindow<AllAwardViewWindow> ((win)=>{
			win.Initialize(ic.prizes,LanguageConfigManager.Instance.getLanguage("s0120"));
		});
		ic.awardType = 1;
		isReceived();
	}
	public void isReceived()
	{
		if(ic.awardType == 0)
		{
			if(ic.jindu >= StringKit.toInt(ic.inviteNeedNum))
			{
				spriteIsDone.spriteName = "text_canReceive";
			}
			else
			{
				spriteIsDone.spriteName = "text_doing";
			}
		}
		else
		{
			spriteIsDone.spriteName = "text_received";
		}
	}


	private void windowBack ()
	{
		UiManager.Instance.openWindow<InviteCodeWindow>();
	}
	
	private PrizeSample getPrize ()
	{
		PrizeSample ps = ic.prizes [0];
		currentNum = ps.getPrizeNumByInt ();
		namelabel = AllAwardViewManagerment.Instance.getNameByType(ps);
		return ps;
	}
}
