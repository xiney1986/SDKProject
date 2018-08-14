using UnityEngine;
using System.Collections;

public class StarSoulArmySelectWindow : WindowBase {
	
	private CallBack callback;//回调
	private CallBack<int> callbackI;
	public ButtonBase armyOne;//队伍一按钮
	public ButtonBase armyTwo;//仓库按钮
	public ButtonBase armyThree;//队伍三按钮
	//    public UILabel[] labelButtonText;
	public UILabel[] labelActives; //冒险中字样
	
	public override void OnAwake ()
	{
		base.OnAwake ();
		//竞技场队伍
		if (UserManager.Instance.self.getUserLevel () < TeamEditWindow.TEAMTHREELV) {
			armyThree.disableButton(true);
			armyThree.textLabel.text = "[FF0000]" + LanguageConfigManager.Instance.getLanguage("s0068")
				+ " (" + LanguageConfigManager.Instance.getLanguage("warchoose04",TeamEditWindow.TEAMTHREELV.ToString ()) + ")";
		}
		
	}
	
	protected override void DoEnable ()
	{
		base.DoEnable ();
		if (ArmyManager.Instance.getlockArmyID()>0) {
			
			if (ArmyManager.Instance.getlockArmyID() == ArmyManager.Instance.getArmy(1).armyid) {
				labelActives [0].text = LanguageConfigManager.Instance.getLanguage ("s0409");
			} else if (ArmyManager.Instance.getlockArmyID() == ArmyManager.Instance.getArmy(2).armyid) {
				labelActives [1].text = LanguageConfigManager.Instance.getLanguage ("s0409");
			} else if (ArmyManager.Instance.getlockArmyID() == ArmyManager.Instance.getArmy(3).armyid) {
				labelActives [2].text = LanguageConfigManager.Instance.getLanguage ("s0409");
			}
		}
	}
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
		
		
	}
	
	//初始化窗口
	public void initWindow(CallBack callback,int index)
	{
		this.callback = callback;
	}
	public void initWindowI(CallBack<int> callbackI)
	{
		this.callbackI = callbackI;
	}
	
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 
		
		if(gameObj.name == "armyOne")
		{
			if(fatherWindow is StarSoulWindow || fatherWindow is SoulHuntWindow)
			{
				finishWindow ();
				if(callbackI != null)
				{
					callbackI(ArmyManager.PVE_TEAMID);
					callbackI = null;
				}
				return;
			}
		}
		else if(gameObj.name == "armyTwo")
		{
			if(fatherWindow is StarSoulWindow || fatherWindow is SoulHuntWindow)
			{
				finishWindow ();
				if(callbackI != null)
				{
					callbackI(10);
					callbackI = null;
					
				}
				return;
			}
		}
		else if(gameObj.name == "armyThree")
		{
			if(fatherWindow is StarSoulWindow || fatherWindow is SoulHuntWindow)
			{
				finishWindow ();
				if(callbackI != null)
				{
					callbackI(ArmyManager.PVP_TEAMID);
					callbackI = null;
				}
				return;
			}
		}
		
		finishWindow ();
		if(callback != null)
		{
			callback();
			callback = null;
		}
		else if(callbackI != null)
		{
			callbackI(1);
			callbackI = null;
		}
	}
}
