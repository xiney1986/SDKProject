using UnityEngine;
using System;

/**
 * 卡片技能升级接口
 * @author 汤琦
 * */
public class CardAdvanceSkillFPort : BaseFPort
{
	private CallBack<string> callback;
	
	public CardAdvanceSkillFPort()
	{
		
	}
	//副卡id（foodUID）以逗号分隔的字符串
	public void access (string mainUID,string foodUID,CallBack<string> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CARD_SKILL_ADVANCE); 
		message.addValue ("mainuid", new ErlString (mainUID));
		message.addValue ("fooduid", new ErlString (foodUID));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value; 
		if(str == FPortGlobal.INTENSIFY_SUCCESS)
		{
			GuideManager.Instance.doGuide(); 
			string uid = (message.getValue ("main_uid") as ErlType).getValueString();
			//刷新卡片仓库
//			SerializeStorageFPort serialize = FPortManager.Instance.getFPort("SerializeStorageFPort") as SerializeStorageFPort;
//			serialize.access("card_storage",callback,uid);
			if(callback != null){
				callback(uid);
				callback = null;
			}
		}
		else if(str == FPortGlobal.INTENSIFY_LIMIT)//卡片状态限制
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify10"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_MAINLOSE)//主角卡不能提升
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify11"),null);
		}
		else if(str == FPortGlobal.ARMY_NO_CARD)//没有此卡
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify12"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_OUTCASH)//缺钱
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify13"),null);
		}
		else if(str == FPortGlobal.INTENSIFY_NOTEATANYMORE)//满级不能吃任何卡
		{
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify14"),null);
		}
		else {
			UiManager.Instance.createMessageWindowByOneButton(LanguageConfigManager.Instance.getLanguage("Intensify15"),null);
		}
	}
}
