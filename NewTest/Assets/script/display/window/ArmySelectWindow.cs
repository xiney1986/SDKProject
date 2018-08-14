using UnityEngine;
using System.Collections;

/**
 * 队伍选择窗口
 * @author 汤琦
 * */
public class ArmySelectWindow : WindowBase
{
	private CallBack callback;//回调
	private CallBack<int> callbackI;
	public ButtonBase armyOne;//队伍一按钮
	public ButtonBase armyTwo;//队伍二按钮
	public ButtonBase armyThree;//队伍三按钮
//    public UILabel[] labelButtonText;
	public UILabel[] labelActives; //冒险中字样
	private int armyIndex = 0;

	public override void OnAwake ()
	{
		base.OnAwake ();
		//队伍二 40级开放
		if (UserManager.Instance.self.getUserLevel () < TeamEditWindow.TEAMTWOLV) {
			armyTwo.disableButton(true);
			armyTwo.textLabel.text = "[FF0000]" + LanguageConfigManager.Instance.getLanguage("s0067") + " (" + LanguageConfigManager.Instance.getLanguage("warchoose04","40") + ")";
		}
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
		this.armyIndex = index;
		this.callback = callback;
	}
	public void initWindowI(CallBack<int> callbackI,int index)
	{
		this.callbackI = callbackI;
		this.armyIndex = index;
	}

	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj); 

		if(gameObj.name == "armyOne")
		{
			if(fatherWindow is TeamEditWindow)
				ArmyManager.Instance.ActiveEditArmy=ArmyManager.Instance.EditArmy1;
			else
				ArmyManager.Instance.setActive(1); 
			armyIndex = 1;
//			ArmyManager.Instance.updateArmy (2, ArmyManager.Instance.EditArmy2);
//			ArmyManager.Instance.updateArmy (3, ArmyManager.Instance.EditArmy3);
//			ArmyManager.Instance.recalculateAllArmyIds();
		}
		else if(gameObj.name == "armyTwo")
		{
			if(fatherWindow is TeamEditWindow)
				ArmyManager.Instance.ActiveEditArmy=ArmyManager.Instance.EditArmy2;
			else
				ArmyManager.Instance.setActive(2); 
			armyIndex = 2;
//			ArmyManager.Instance.updateArmy (1, ArmyManager.Instance.EditArmy1);
//			ArmyManager.Instance.updateArmy (3, ArmyManager.Instance.EditArmy3);
//			ArmyManager.Instance.recalculateAllArmyIds();
		}
		else if(gameObj.name == "armyThree")
		{
			if(fatherWindow is TeamEditWindow)
				ArmyManager.Instance.ActiveEditArmy=ArmyManager.Instance.EditArmy3;
			else
				ArmyManager.Instance.setActive(3); 
			armyIndex = 3;
//			ArmyManager.Instance.updateArmy (1, ArmyManager.Instance.EditArmy1);
//			ArmyManager.Instance.updateArmy (2, ArmyManager.Instance.EditArmy2);
//			ArmyManager.Instance.recalculateAllArmyIds();
		}
		ArmyManager.Instance.saveArmy ();
		finishWindow ();

		if(callback != null)
		{
			callback();
			callback = null;
		}
		else if(callbackI != null)
		{
			callbackI(armyIndex);
			callbackI = null;
			armyIndex = 0;
		}
	}

}
