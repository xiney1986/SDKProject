using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 公会战行军值提示窗口
/// </summary>
public class GuildFightPowerTipWindow : WindowBase
{
	public UIToggle neverChoose;
	public CallBack callback;
	public UILabel tipLabel;

	
	protected override void begin ()
	{
		base.begin ();
		MaskWindow.UnlockUI ();
	}
	/// <summary>
	/// 初始化提示
	/// </summary>
	public void initWindow (int currentPower,int power,CallBack callback)
	{
		this.callback = callback;
		tipLabel.text = LanguageConfigManager.Instance.getLanguage("GuildArea_55",currentPower.ToString(),power.ToString());
	}
	/// <summary>
	/// 点击事件
	/// </summary>
	public override void buttonEventBase (GameObject gameObj)
	{
		base.buttonEventBase (gameObj);
		if (gameObj.name == "confirm") 
		{
			if (neverChoose.value)
			{
				TaskManagerment.Instance.isTips = true;
			}
			callback ();
		} 
		else if (gameObj.name == "cancel") 
		{
		}
		finishWindow ();
	}
}
