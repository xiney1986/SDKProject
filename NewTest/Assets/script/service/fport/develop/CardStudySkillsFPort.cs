using UnityEngine;
using System;

/**
 * 卡片技能学习接口
 * @author 汤琦
 * */
public class CardStudySkillsFPort : BaseFPort 
{
	private CallBack<string> callback;
	
	public CardStudySkillsFPort()
	{
		
	}
	//mainUID 主卡uid
	//fooduid 副卡uid
	//type 学习技能类型1开场2主动3被动
	//sid1 学习的技能sid
	//sid2 主卡替换的技能sid,0为学习,有则为替换
	public void access (string mainUID,string foodUID,int type,int mainSkillId,int foodSkillId,CallBack<string> back)
	{ 
		this.callback = back;
		ErlKVMessage message = new ErlKVMessage (FrontPort.CARD_SKILL_STUDE); 
		message.addValue ("mainuid", new ErlString (mainUID));
		message.addValue ("fooduid", new ErlString (foodUID));
		message.addValue ("type", new ErlInt (type));
		message.addValue ("sid1", new ErlInt (foodSkillId));
		message.addValue ("sid2", new ErlInt (mainSkillId));
		access (message);
	}
	
	public override void read (ErlKVMessage message)
	{
		string str = (message.getValue ("msg") as ErlAtom).Value;
		
		if (str == FPortGlobal.INTENSIFY_SUCCESS) {
			if(callback != null)
				callback(null);
		} else {
			MessageWindow.ShowAlert (str);
			if (callback != null)
				callback = null;
		}
		/**


		else if(str == FPortGlobal.INTENSIFY_LIMIT)//卡片状态限制
		{
			
		}
		else if(str == FPortGlobal.INTENSIFY_MAINLOSE)//主角卡不能提升
		{
			
		}
		else if(str == FPortGlobal.INTENSIFY_PLENGTHLIMIT)//被动技能长度限制
		{
			
		}
		else if(str == FPortGlobal.INTENSIFY_ALENGTHLIMIT)//主动技能长度限制
		{
			
		}
		else if(str == FPortGlobal.INTENSIFY_OUTCASH)//缺钱
		{
			
		}
		else if(str == FPortGlobal.INTENSIFY_ERROR)//其他信息错误
		{
			
		}
		else if(str == FPortGlobal.ARMY_NO_CARD)//没有此卡
		{
			
		}
		*/
	}
}
