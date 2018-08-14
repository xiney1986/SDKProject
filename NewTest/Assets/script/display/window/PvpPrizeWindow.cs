using UnityEngine;
using System.Collections;

/**
 * PVP奖励窗口
 * @author 汤琦
 * */
public class PvpPrizeWindow : WindowBase
{
	public TapContentBase tapBase;//分页按钮
	public WinStreakContent winStreak;//连胜奖励页
	public WinStreakMaxContent winStreakMax;//最高连胜奖页
	public UISprite showIcon;//超过一页的标示图

	protected override void DoEnable ()
	{
		base.DoEnable ();
		UiManager.Instance.backGround.switchBackGround ("ChouJiang_BeiJing");
		if (MissionManager.instance != null)
			MissionManager.instance.hideAll ();
	}

	protected override void begin ()
	{
		base.begin ();
		if (!isAwakeformHide)
		{
			tapBase.changeTapPage (tapBase.tapButtonList [0]);
			isWinStreakShowIcon();
			isWinStreakMaxShowIcon();
		}
		MaskWindow.UnlockUI ();
	}
	


	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "close")
		{
			finishWindow();
			if (MissionManager.instance != null)
			{
				MissionManager.instance.showAll ();
				MissionManager.instance.setBackGround();
			}
			if(fatherWindow is MissionMainWindow)
			{
				EventDelegate.Add(OnHide,()=>{
					PvpInfoManagerment.Instance.setPvpType(PvpInfo.TYPE_PVP_FB);
					UiManager.Instance.openDialogWindow<PvpInfoWindow>();
					
				});
			}
		}



	}
	
	public override void tapButtonEventBase (GameObject gameObj, bool enable)
	{
		base.buttonEventBase (gameObj); 
		if (gameObj.name == "winStreak" && enable == true)
		{
			winStreak.Initialize(PvpPrizeSampleManager.Instance.getWinStreak());
		}
		if (gameObj.name == "winStreakMax" && enable == true)
		{
			winStreakMax.Initialize(PvpPrizeSampleManager.Instance.getWinStreakMax());
		}
	}
	
	//连胜奖励页是否显示超过图标
	private void isWinStreakShowIcon()
	{
		if(PvpPrizeSampleManager.Instance.getWinStreak().Length > 2)
		{
			showIcon.gameObject.SetActive(true);
		}
		else
		{
			showIcon.gameObject.SetActive(false);
		}
	}
	
	//最高连胜奖页是否显示超过图标
	private void isWinStreakMaxShowIcon()
	{
		if(PvpPrizeSampleManager.Instance.getWinStreakMax().Length > 2)
		{
			showIcon.gameObject.SetActive(true);
		}
		else
		{
			showIcon.gameObject.SetActive(false);
		}
	}

}
