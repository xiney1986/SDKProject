using UnityEngine;
using System.Collections;

public class LastBattleDonateFPort : BaseFPort
{
	private CallBack callBack;

	// cost：当捐献的物品为卡片或者装备时，cost为"card,1,2,3,4" 或者 "equipment,5,6,7,8"，调该方法//
	public void lastBattleDonateAccess(CallBack _callBack,int index,string cost)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEDONATE);
		message.addValue ("index", new ErlInt (index));//捐献条目下标
		message.addValue ("cost", new ErlString (cost));//捐献物品id
		access (message);
	}
	// //
	public void lastBattleDonateAccess2(CallBack _callBack,int index)
	{
		this.callBack = _callBack;
		
		ErlKVMessage message = new ErlKVMessage (FrontPort.LASTBATTLEDONATE);
		message.addValue ("index", new ErlInt (index));//捐献条目下标
		access (message);
	}

	public override void read (ErlKVMessage message)
	{
		ErlType type = message.getValue ("msg") as ErlType;
		string result = type.getValueString();
		if(result == "ok")// 捐献成功成功//
		{
			if(callBack != null)
			{
				callBack();
				callBack = null;
			}
		}
		else
		{
			result = LanguageConfigManager.Instance.getLanguage("LastBattle_" + result);
			UiManager.Instance.openDialogWindow<MessageWindow> ((win) => {
				win.initWindow (1, LanguageConfigManager.Instance.getLanguage ("s0093"), null, result, null);
			});
		}
	}

}
